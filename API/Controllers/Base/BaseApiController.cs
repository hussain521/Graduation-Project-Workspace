namespace API.Controllers.Base
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BaseController<T> : ControllerBase where T : BaseEntity
    {
        protected User GetUserInfo()
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Request.Headers.Authorization))
            {
                //var jwt = HttpContext.Request.Headers.Authorization;
                //jwt = jwt.ToString().Replace("Bearer ",string.Empty).Replace("{",string.Empty).Replace("}",string.Empty);
                var jwt = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer", string.Empty).Replace(" ", string.Empty);
                var handler = new JwtSecurityTokenHandler();
                var tkn = handler.ReadJwtToken(jwt);
                string usrInfo = tkn.Claims.Where(c => c.Type == "LoginInfo").FirstOrDefault().Value;
                if (!string.IsNullOrWhiteSpace(usrInfo))
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.NullValueHandling = NullValueHandling.Ignore;
                    settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    var Result = JsonConvert.DeserializeObject<User>(usrInfo, settings);                                        
                    return Result;
                }
                else return null;
            }
            else return null;
        }

        protected string GetUserIdString()
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Request.Headers.Authorization))
            {
                //var jwt = HttpContext.Request.Headers.Authorization;
                //jwt = jwt.ToString().Replace("Bearer ",string.Empty).Replace("{",string.Empty).Replace("}",string.Empty);
                var jwt = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer", string.Empty).Replace(" ", string.Empty);
                var handler = new JwtSecurityTokenHandler();
                var tkn = handler.ReadJwtToken(jwt);
                string usrInfo = tkn.Claims.Where(c => c.Type == "Id").FirstOrDefault().Value;
                var Result = usrInfo;
                return Result;
            }
            else return string.Empty;
        }

        protected Guid? GetUserId()
        {
            if (!string.IsNullOrWhiteSpace(HttpContext.Request.Headers.Authorization))
            {
                //var jwt = HttpContext.Request.Headers.Authorization;
                //jwt = jwt.ToString().Replace("Bearer ",string.Empty).Replace("{",string.Empty).Replace("}",string.Empty);
                var jwt = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer", string.Empty).Replace(" ", string.Empty);
                var handler = new JwtSecurityTokenHandler();
                var tkn = handler.ReadJwtToken(jwt);
                string usrInfo = tkn.Claims.Where(c => c.Type == "Id").FirstOrDefault().Value;
                var Result = new Guid(usrInfo);
                return Result;
            }
            else return null;
        }

        protected virtual T AddBaseInfo(T Entity)
        {
            var LoginInfo = this.GetUserInfo();
            Entity.UserId = LoginInfo.Id;
            Entity.OrganizationId = LoginInfo.OrganizationId;
            return Entity;
        }       
    }
}
