using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AccountingERP.Data;
using AccountingERP.Models;

namespace AccountingERP.Services
{
    public interface IUserService
    {
        Task<List<UserViewModel>> GetUsersAsync();
        Task<UserViewModel?> GetUserByIdAsync(Guid id);
        Task<ApplicationUser?> AuthenticateAsync(string username, string password);
        Task<ApiResponse<UserViewModel>> CreateUserAsync(UserViewModel model);
        Task<ApiResponse<UserViewModel>> UpdateUserAsync(UserViewModel model);
        Task<ApiResponse<bool>> DeleteUserAsync(Guid id);
        Task<List<ApplicationRole>> GetRolesAsync();
        Task<List<Organization>> GetOrganizationsAsync();
        Task<ApiResponse<ApplicationRole>> SaveRoleWithPermissionsAsync(ApplicationRole role, List<RolePermission> permissions);
    }

    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly ITenantService _tenantService;

        public UserService(AppDbContext context, ITenantService tenantService)
        {
            _context = context;
            _tenantService = tenantService;
        }

        public async Task<List<UserViewModel>> GetUsersAsync()
        {
            var users = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Organization)
                .ToListAsync();

            return users.Select(u => new UserViewModel
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                Email = u.Email,
                Phone = u.Phone,
                RoleId = u.RoleId,
                RoleName = u.Role?.NameAr ?? (u.IsAdmin ? "مدير النظام" : "مستخدم"),
                OrganizationId = u.OrganizationId,
                OrganizationName = u.Organization?.NameAr ?? string.Empty,
                IsAdmin = u.IsAdmin,
                IsActive = u.IsActive
            }).ToList();
        }

        public async Task<UserViewModel?> GetUserByIdAsync(Guid id)
        {
            var u = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Organization)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (u == null) return null;

            return new UserViewModel
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                Email = u.Email,
                Phone = u.Phone,
                RoleId = u.RoleId,
                RoleName = u.Role?.NameAr ?? (u.IsAdmin ? "مدير النظام" : "مستخدم"),
                OrganizationId = u.OrganizationId,
                OrganizationName = u.Organization?.NameAr ?? string.Empty,
                IsAdmin = u.IsAdmin,
                IsActive = u.IsActive
            };
        }

        public async Task<ApplicationUser?> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users
                .IgnoreQueryFilters()
                .Include(u => u.Organization)
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower() && u.IsActive);

            if (user == null) return null;

            if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return user;
            }

            return null;
        }

        public async Task<ApiResponse<UserViewModel>> CreateUserAsync(UserViewModel model)
        {
            var res = new ApiResponse<UserViewModel>();

            var exists = await _context.Users
                .IgnoreQueryFilters()
                .AnyAsync(u => u.Username.ToLower() == model.Username.ToLower());

            if (exists)
            {
                res.Success = false;
                res.Message = "اسم المستخدم مستخدم من قبل";
                return res;
            }

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                res.Success = false;
                res.Message = "كلمة المرور مطلوبة للإنشاء";
                return res;
            }

            var orgId = _tenantService.OrganizationId != Guid.Empty ? _tenantService.OrganizationId : model.OrganizationId;

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                OrganizationId = orgId,
                Username = model.Username.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                FullName = model.FullName.Trim(),
                Email = model.Email?.Trim() ?? string.Empty,
                Phone = model.Phone?.Trim() ?? string.Empty,
                RoleId = model.RoleId,
                IsAdmin = model.IsAdmin,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            res.Success = true;
            res.Message = "تم إضافة المستخدم بنجاح";
            res.Data = await GetUserByIdAsync(user.Id);
            return res;
        }

        public async Task<ApiResponse<UserViewModel>> UpdateUserAsync(UserViewModel model)
        {
            var res = new ApiResponse<UserViewModel>();

            if (!model.Id.HasValue)
            {
                res.Success = false;
                res.Message = "معرف المستخدم غير صحيح";
                return res;
            }

            var user = await _context.Users.FindAsync(model.Id.Value);
            if (user == null)
            {
                res.Success = false;
                res.Message = "المستخدم غير موجود";
                return res;
            }

            user.FullName = model.FullName.Trim();
            user.Email = model.Email?.Trim() ?? string.Empty;
            user.Phone = model.Phone?.Trim() ?? string.Empty;
            user.RoleId = model.RoleId;
            user.IsAdmin = model.IsAdmin;
            user.IsActive = model.IsActive;

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            }

            await _context.SaveChangesAsync();

            res.Success = true;
            res.Message = "تم تحديث بيانات المستخدم بنجاح";
            res.Data = await GetUserByIdAsync(user.Id);
            return res;
        }

        public async Task<ApiResponse<bool>> DeleteUserAsync(Guid id)
        {
            var res = new ApiResponse<bool>();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                res.Success = false;
                res.Message = "المستخدم غير موجود";
                return res;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            res.Success = true;
            res.Data = true;
            res.Message = "تم حذف المستخدم بنجاح";
            return res;
        }

        public async Task<List<ApplicationRole>> GetRolesAsync()
        {
            return await _context.Roles.Include(r => r.Permissions).ToListAsync();
        }

        public async Task<List<Organization>> GetOrganizationsAsync()
        {
            return await _context.Organizations.IgnoreQueryFilters().Where(o => o.IsActive).ToListAsync();
        }

        public async Task<ApiResponse<ApplicationRole>> SaveRoleWithPermissionsAsync(ApplicationRole role, List<RolePermission> permissions)
        {
            var res = new ApiResponse<ApplicationRole>();

            if (role.Id == Guid.Empty)
            {
                role.Id = Guid.NewGuid();
                role.OrganizationId = _tenantService.OrganizationId;
                _context.Roles.Add(role);
            }
            else
            {
                var existing = await _context.Roles.Include(r => r.Permissions).FirstOrDefaultAsync(r => r.Id == role.Id);
                if (existing != null)
                {
                    existing.NameAr = role.NameAr;
                    existing.NameEn = role.NameEn;
                    existing.Description = role.Description;
                    _context.RolePermissions.RemoveRange(existing.Permissions);
                }
            }

            foreach (var perm in permissions)
            {
                perm.Id = Guid.NewGuid();
                perm.RoleId = role.Id;
                _context.RolePermissions.Add(perm);
            }

            await _context.SaveChangesAsync();

            res.Success = true;
            res.Message = "تم حفظ الدور والصلاحيات بنجاح";
            res.Data = role;
            return res;
        }
    }
}