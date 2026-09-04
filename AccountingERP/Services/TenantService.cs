using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace AccountingERP.Services
{
    public interface ITenantService
    {
        Guid OrganizationId { get; }
        Guid? UserId { get; }
        string UserName { get; }
        bool IsAdmin { get; }
        void SetTenant(Guid organizationId);
    }

    public class TenantService : ITenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private Guid _explicitTenantId = Guid.Empty;

        public TenantService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid OrganizationId
        {
            get
            {
                if (_explicitTenantId != Guid.Empty)
                    return _explicitTenantId;

                var context = _httpContextAccessor.HttpContext;
                if (context != null)
                {
                    // 1. Check Header
                    if (context.Request.Headers.TryGetValue("X-Organization-Id", out var orgHeader) &&
                        Guid.TryParse(orgHeader, out var headerOrgId))
                    {
                        return headerOrgId;
                    }

                    // 2. Check Claims
                    var claimOrg = context.User?.FindFirst("OrganizationId")?.Value;
                    if (!string.IsNullOrEmpty(claimOrg) && Guid.TryParse(claimOrg, out var orgId))
                    {
                        return orgId;
                    }

                    // 3. Check Session / Cookie
                    if (context.Items.TryGetValue("CurrentOrganizationId", out var itemOrg) && itemOrg is Guid itemOrgId)
                    {
                        return itemOrgId;
                    }
                }

                return Guid.Empty;
            }
        }

        public Guid? UserId
        {
            get
            {
                var val = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return Guid.TryParse(val, out var uid) ? uid : null;
            }
        }

        public string UserName => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? "System";

        public bool IsAdmin => _httpContextAccessor.HttpContext?.User?.IsInRole("Admin") == true ||
                               _httpContextAccessor.HttpContext?.User?.FindFirst("IsAdmin")?.Value == "True";

        public void SetTenant(Guid organizationId)
        {
            _explicitTenantId = organizationId;
        }
    }
}