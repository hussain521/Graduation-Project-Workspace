using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace AccountingERP.Services
{
    public interface ITenantService
    {
        Guid OrganizationId { get; }
        Guid BranchId { get; }
        Guid UserId { get; }
        string UserName { get; }
        bool IsAdmin { get; }
        void SetOrganization(Guid orgId);
    }

    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private Guid? _overrideOrganizationId;

        public TenantService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid OrganizationId
        {
            get
            {
                if (_overrideOrganizationId.HasValue && _overrideOrganizationId.Value != Guid.Empty)
                    return _overrideOrganizationId.Value;

                var user = _httpContextAccessor.HttpContext?.User;
                var claim = user?.FindFirst("OrganizationId")?.Value;
                if (Guid.TryParse(claim, out var orgId))
                    return orgId;

                var headerClaim = _httpContextAccessor.HttpContext?.Request.Headers["X-OrganizationId"].ToString();
                if (Guid.TryParse(headerClaim, out var headerOrgId))
                    return headerOrgId;

                return Guid.Empty;
            }
        }

        public Guid BranchId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var claim = user?.FindFirst("BranchId")?.Value;
                if (Guid.TryParse(claim, out var branchId))
                    return branchId;

                return Guid.Empty;
            }
        }

        public Guid UserId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                var claim = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (Guid.TryParse(claim, out var userId))
                    return userId;

                return Guid.Empty;
            }
        }

        public string UserName => _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

        public bool IsAdmin => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value == "Admin" ||
                               _httpContextAccessor.HttpContext?.User?.FindFirst("IsAdmin")?.Value == "True";

        public void SetOrganization(Guid orgId)
        {
            _overrideOrganizationId = orgId;
        }
    }
}