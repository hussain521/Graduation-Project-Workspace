using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AccountingERP.Models;
using AccountingERP.Services;

namespace AccountingERP.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetUsersAsync();
            var roles = await _userService.GetRolesAsync();
            var orgs = await _userService.GetOrganizationsAsync();

            ViewBag.Roles = roles;
            ViewBag.Organizations = orgs;

            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "بيانات المستخدم غير مكتملة" });
            }

            var res = await _userService.CreateUserAsync(model);
            if (!res.Success)
            {
                return BadRequest(res);
            }

            return Json(res);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromBody] UserViewModel model)
        {
            var res = await _userService.UpdateUserAsync(model);
            if (!res.Success)
            {
                return BadRequest(res);
            }

            return Json(res);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var res = await _userService.DeleteUserAsync(id);
            if (!res.Success)
            {
                return BadRequest(res);
            }

            return Json(res);
        }
    }
}