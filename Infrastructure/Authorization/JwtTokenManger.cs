using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text; // تم إضافة هذا السطر لحل مشكلة Encoding
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Authorization
{
    public class JwtTokenManger : IJwtTokenManger
    {
        private readonly IConfiguration _configuration;
        protected readonly ApplicationDbContext _context;
        PasswordHasher<User> _passwordHasher;

        public JwtTokenManger(IConfiguration configuration, ApplicationDbContext context)
        {
            _context = context;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
        }

        public bool VerifyPassword(User user, string enteredPassword)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, enteredPassword);
            return result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded;
        }

        public string Authenticate(User login)
        {
            var QueryResult = _context.Users.Where(u => u.Email == login.Email).ToList();                      
            if (QueryResult == null || QueryResult.Count == 0 || QueryResult.Count > 1)
                return null;
            else
            {
                try
                {
                    var checkPassword = this.VerifyPassword(QueryResult.FirstOrDefault(), login.Password);
                    if (checkPassword == true)
                    {
                        var first = QueryResult.FirstOrDefault();
                        var userinfo = new User
                        {
                            Id = first.Id,
                            Email = first.Email,
                            UserName = first.UserName,
                            OrganizationId = first.OrganizationId
                        };

                        var key = _configuration["JwtConfig:Key"];
                        var keyBytes = Encoding.UTF8.GetBytes(key);
                        var tokenHandler = new JwtSecurityTokenHandler();

                        JsonSerializerSettings settings = new JsonSerializerSettings();
                        settings.NullValueHandling = NullValueHandling.Ignore;
                        settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        var Json = JsonConvert.SerializeObject(userinfo, settings);

                        // تم تصحيح الشرط الخاص بـ ur.Value هنا
                        var userRoles = _context.UserRoles
                            .Where(ur => ur.UserId == first.Id && ur.OrganizationId == first.OrganizationId && ur.IsActive == true && ur.Value == true)
                            .Select(ur => ur.RoleId.ToString())
                            .ToList();

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, userinfo.UserName.ToString()),
                            new Claim("Name", userinfo.UserName.ToString()),
                            new Claim("Id", userinfo.Id.ToString()),
                            new Claim("LoginInfo", Json)
                        };

                        foreach (var roleId in userRoles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, roleId));
                        }

                        var tokenDescriptor = new SecurityTokenDescriptor()
                        {
                            Subject = new ClaimsIdentity(claims),
                            Expires = DateTime.UtcNow.AddDays(15),
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                        };

                        var token = tokenHandler.CreateToken(tokenDescriptor);
                        return tokenHandler.WriteToken(token);
                    }
                    else return null;
                }
                catch (Exception ex)
                {
                    return null;
                }                
            }
        }
    }
}