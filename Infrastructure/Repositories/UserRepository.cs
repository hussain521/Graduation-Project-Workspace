using Infrastructure.Authorization;
using Microsoft.AspNetCore.Identity;
using Shared.Constant.Roles;

namespace Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>
    {
        private readonly IJwtTokenManger _tokenManger;
        PasswordHasher<User> _passwordHasher;
        public UserRepository(IJwtTokenManger tokenManger, ApplicationDbContext context) : base(context)
        {
            _passwordHasher = new PasswordHasher<User>();
            _tokenManger = tokenManger;
        }

        public async Task<bool> CheckEmailExists(string Email)
        {
            var users =await _context.Users.Where(a => a.Email == Email || a.UserName==Email).ToListAsync();
            if (users != null && users.Count>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Result<string> LoginAsync(User user)
        {
            var token=this._tokenManger.Authenticate(user);
            if (!string.IsNullOrWhiteSpace(token))
            {
                return Result.Success(token);
            }
            else
            {
                return Result.Failure<string>(new Error("Invalid Email or Password"));
            }
        }

        public async Task<Result<bool>> RegisterAsync(User user)
        {
            var exists = await this.CheckEmailExists(user.Email);
            if (!exists)
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    Organization organization = user.Organization ?? new Organization { Name = user.Email };
                    if (organization.Id == Guid.Empty)
                    {
                        organization.Id = Guid.NewGuid();
                    }
                    await this._context.Organizations.AddAsync(organization);

                    User NewUser = new User
                    {
                        Id = Guid.NewGuid(),
                        Email = user.Email,
                        UserName = user.Email,
                        Password = _passwordHasher.HashPassword(user, user.Password),
                        ConfirmPassword = _passwordHasher.HashPassword(user, user.ConfirmPassword),
                        OrganizationId = organization.Id,
                    };

                    await this._context.AddAsync(NewUser);
                    var AllRoles = RolesManager.GetAllRolesIds();
                    foreach (var role in AllRoles)
                    {
                        var userRole = new UserRole
                        {
                            Id = Guid.NewGuid(),
                            UserId = NewUser.Id,
                            RoleId = new Guid(role),
                            IsActive = true,
                            Value = true,
                            OrganizationId = organization.Id,
                        };
                        await this._context.AddAsync(userRole);
                    }

                    Currency YR = new Currency
                    {
                        Id = Guid.NewGuid(),
                        Name = "YER",
                        Symbol = "YER",
                        IsLocal = true,
                        CurrentExchangeRate = 1,
                        UserId = NewUser.Id,
                        OrganizationId = organization.Id,
                    };
                    await this._context.Currencies.AddAsync(YR);
                    await this._context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Result.Success<bool>(true);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return Result.Failure<bool>(new Error(ex.Message));
                }
            }
            else
            {
                return Result.Failure<bool>(new Error("Email Already Exists"));
            }
        }
    }
}
