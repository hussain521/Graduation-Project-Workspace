using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Security.Principal;

namespace FinalProject.Authorization
{
    public class CustomClaimsPrincipal : ClaimsPrincipal
    {
        public CustomClaimsPrincipal(IPrincipal principal) : base(principal)
        {

        }

        public override bool IsInRole(string Role)
        {
            return true;
            /*ClaimsIdentity Claims = Identity as ClaimsIdentity;
            if (Identity.IsAuthenticated && Claims.Claims.Any(claim => claim.Type == "Id"))
            {
                var LoginId = Claims.Claims.First(claim => claim.Type == "Id").Value.ToInt64();
                if (!GlobalUsersStore.UserSessionsMap.ContainsKey(LoginId))
                    return false;
                else
                {
                    //var role=GlobalUsersStore.UserSessionsMap[LoginId].UserPermissionsMap.Any(p => p.Id == Role);
                    var val = GlobalUsersStore.UserSessionsMap[LoginId].UserPermissionsMap.Any(p => p.RoleId == Role);
                    return val;
                }
                //return GlobalUsersStore.Authorize(LoginId, Role);                
            }
            else return false;*/
        }
    }

    public class ClaimsTransformer : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var customPrincipal = new CustomClaimsPrincipal(principal) as ClaimsPrincipal;
            return Task.FromResult(customPrincipal);
        }
    }
}