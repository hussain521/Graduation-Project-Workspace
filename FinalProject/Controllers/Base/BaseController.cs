using AspNetCoreHero.ToastNotification.Abstractions;

namespace FinalProject.Controllers.Base
{
    public class BaseController<T> : Controller where T : BaseEntity
    {
        GenericApiClient<T> _apiClient;
        public BaseController(GenericApiClient<T> apiClient)
        {                
            this._apiClient = apiClient;
        }

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

        #region Messages

        public INotyfService Notify
        {
            get
            {
                var sf = (INotyfService?)HttpContext.RequestServices.GetService(typeof(INotyfService));
                return sf;
            }
        }
        // Warning & Notification helpers
        protected void SuccessMessage(string Message)
        {
            TempData["SuccessMessage"] = Message;
            this.Notify?.Success(Message);
        }

        protected void SuccessMessage(string Message, int Seconds)
        {
            TempData["SuccessMessage"] = Message;
            this.Notify?.Success(Message, Seconds);
        }

        protected void WarningMessage(string Message)
        {
            TempData["WarningMessage"] = Message;
            this.Notify?.Warning(Message);
        }

        protected void WarningMessage(string Message, int Seconds)
        {
            TempData["WarningMessage"] = Message;
            this.Notify?.Warning(Message, Seconds);
        }

        // Error
        protected void ErrorMessage(string Message)
        {
            TempData["ErrorMessage"] = Message;
            this.Notify?.Error(Message);
        }

        protected void ErrorMessage(string Message, int Seconds)
        {
            TempData["ErrorMessage"] = Message;
            this.Notify?.Error(Message, Seconds);
        }

        // Information
        protected void InformationMessage(string Message)
        {
            TempData["InfoMessage"] = Message;
            this.Notify?.Information(Message);
        }

        protected void InformationMessage(string Message, int Seconds)
        {
            TempData["InfoMessage"] = Message;
            this.Notify?.Information(Message, Seconds);
        }
       
        [HttpPost]
        [ActionName("ShowMessage")]
        public void ShowMessage(MessageModel message)
        {
            if (message.Type == MessageType.Success)
                this.SuccessMessage(message.Text, message.Duration);
            else if (message.Type == MessageType.Information)
                this.InformationMessage(message.Text, message.Duration);
            else if (message.Type == MessageType.Warning)
                this.WarningMessage(message.Text, message.Duration);
            else if (message.Type == MessageType.Error)
                this.ErrorMessage(message.Text, message.Duration);
        }
        #endregion
    }
}
