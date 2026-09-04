using System.Globalization;
using System.Net.Http.Headers;
using System.Net;

namespace FinalProject.Plugins
{
    public class TokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenHandler(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            try
            {
                /*if(_httpContextAccessor!=null &&
                    _httpContextAccessor.HttpContext!=null &&
                    _httpContextAccessor.HttpContext.User!=null &&
                    _httpContextAccessor.HttpContext.User.Claims!=null &&                    )
                {

                }*/

                var idClm = _httpContextAccessor.HttpContext.User.FindFirst("Id");
                if (idClm != null)
                {
                    var userId = idClm.Value;
                }
                else
                {

                }
            }
            catch (Exception)
            {
                //_httpContextAccessor.HttpContext.SignOutAsync().Wait();
                //_httpContextAccessor.HttpContext.Request.Path= "/Authentication/login";

            }

            var token = _httpContextAccessor.HttpContext.User.Claims.Where(c => c.Type == "JWToken")?.FirstOrDefault()?.Value;//.GetTokenAsync("JWToken");
            if (string.IsNullOrWhiteSpace(token))
            {

            }
            else
            {
                //string sub = token.Substring(0, 4);
                //token = token.Replace(sub, "syJk");
            }

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
            var tok = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _httpContextAccessor.HttpContext.Response.Redirect("../Users/Logout", true);
            }

            //if (!response.IsSuccessStatusCode)
            //{
            //    _writer.WriteLine("{0}\t{1}\t{2}", request.RequestUri,
            //        (int)response.StatusCode, response.Headers.Date);
            //}
            return response;
        }
    }
}
