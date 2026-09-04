using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Domain.DTOs;

namespace FinalProject.Controllers
{
    public class UsersController : BaseController<User>
    {
        UsersApiClient _usersApiClient;
        public UsersController(UsersApiClient usersApiClient):base(usersApiClient)
        {
            _usersApiClient = usersApiClient;
        }


        //[HttpPost]
        //public async Task<IActionResult> Logout()
        public IActionResult Logout()
        {
            /*var x = Request;
            if (!string.IsNullOrWhiteSpace(Request.Headers.Referer))
            {
            }
            //HttpContext.Session.Clear();
            // مسح الكوكيز الخاصة بالمصادقة
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Users");*/
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> LogOff()
        {
            //HttpContext.Session.Clear();
            // مسح الكوكيز الخاصة بالمصادقة
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Users");
        }

        // GET: UsersController/Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(
           //IFormCollection collection
           [FromForm] User user, string? ReturnUrl
           )
        {
            try
            {
                user.UserName = user.Email;
                user.ConfirmPassword = user.Password;
                var Result = await _usersApiClient.Login(user);
                if (Result != null && Result.IsSuccess)
                {
                    //return RedirectToAction(nameof(Index));
                    var token = Result.Data;
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        var jwt = token;
                        var handler = new JwtSecurityTokenHandler();
                        var tkn = handler.ReadJwtToken(jwt);
                        HttpContext.Request.Headers.Authorization = token;
                        //HttpContext.Session.SetString("JWToken", token);
                        // Create the identity
                        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                        foreach (var claim in tkn.Claims)
                        {
                            identity.AddClaim(claim);
                        }
                        identity.AddClaim(new Claim("JWToken", token));
                        // Sign in
                        var principal = new ClaimsPrincipal(identity);
                        AuthenticationProperties prop = new AuthenticationProperties();
                        prop.IsPersistent = true;

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, prop);
                        //await this.SharedFunctions.LoadUserRoles(this.GetUserId());
                        if (!string.IsNullOrWhiteSpace(ReturnUrl))
                        {
                            //return Redirect(ReturnUrl);
                            return RedirectToAction("Index", "Home");
                        }
                        else return RedirectToAction("Index", "Home");
                    }
                    else return RedirectToAction("Login", "Users");
                }
                else
                {
                    var errorMessage = Result?.Error?.Messages?.FirstOrDefault() ?? "اسم المستخدم أو كلمة المرور غير صحيحة";
                    ModelState.AddModelError(string.Empty, errorMessage);
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(user);
            }
        }


        // GET: UsersController/Create
        public ActionResult Register()
        {
            return View();
        }

        // POST: UsersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(
            [FromForm] User user
            )
        {
            try
            {
                user.UserName = user.Email;
                if (user.Organization == null)
                {
                    user.Organization = new Organization
                    {
                        Name = "Org1"
                    };
                }

                if (string.IsNullOrWhiteSpace(user.Organization.Name))
                {
                    user.Organization.Name = "Org1";
                }

                var Result = await _usersApiClient.Register(user);
                if (Result != null && Result.IsSuccess)
                {
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    var errorMessage = Result?.Error?.Messages?.FirstOrDefault() ?? "فشل إنشاء الحساب";
                    ModelState.AddModelError(string.Empty, errorMessage);
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(user);
            }
        }

        [HttpGet]
        [Authorize]
        public bool IsInRole(string IsInRole)
        {
            return User.IsInRole(IsInRole);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
    }
}
