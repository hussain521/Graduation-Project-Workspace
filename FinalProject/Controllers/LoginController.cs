namespace FinalProject.Controllers
{
    public class LoginController : Controller
    {
        static Dictionary<string,int> Referers = new Dictionary<string, int>();
        public IActionResult Logout()
        {
            //return View();
            var x = Request;
            if (!string.IsNullOrWhiteSpace(Request.Headers.Referer))
            {
                if (!Referers.ContainsKey(Request.Headers.Referer))
                    Referers.Add(Request.Headers.Referer,0);
                Referers[Request.Headers.Referer]++;
            }
            return Ok();
        }
    }
}
