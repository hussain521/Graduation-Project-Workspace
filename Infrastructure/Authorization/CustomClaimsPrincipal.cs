using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Security.Principal;

namespace Infrastructure.Authorization
{
    public class CustomClaimsPrincipal : ClaimsPrincipal
    {
        ApplicationDbContext _context;
        public CustomClaimsPrincipal(IPrincipal principal, ApplicationDbContext context) : base(principal)
        {
            _context = context;
        }

        public override bool IsInRole(string Role)
        {
            ClaimsIdentity Claims = Identity as ClaimsIdentity;
            if (Identity.IsAuthenticated && Claims.Claims.Any(claim => claim.Type == "Id"))
            {
                var LoginId = Claims.Claims.First(claim => claim.Type == "Id").Value;
                var QueryResult = _context.UserRoles.Where(r => r.UserId == new Guid(LoginId) && r.RoleId == new Guid(Role) && r.Value==true).ToList();
                if (QueryResult != null && QueryResult.Count > 0)
                {
                    var Value = QueryResult.FirstOrDefault().Value;
                    if (!Value)
                    {

                    }
                    return Value;
                }
                else return false;
            }
            else return false;
            //return base.IsInRole(role);
        }
    }

    public class ClaimsTransformer : IClaimsTransformation
    {
        ApplicationDbContext _context;
        public ClaimsTransformer(ApplicationDbContext context)
        {
            _context = context;       
        }

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var customPrincipal = new CustomClaimsPrincipal(principal,_context) as ClaimsPrincipal;
            return Task.FromResult(customPrincipal);
        }
    }
}
