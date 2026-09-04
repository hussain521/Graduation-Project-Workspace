namespace FinalProject.Controllers
{
    public class AccountsController : BaseController<Account>
    {
        AccountsApiClient _accountsApiClient;
        public AccountsController(AccountsApiClient accountsApiClient) : base(accountsApiClient)
        {
            _accountsApiClient = accountsApiClient;
        }

        // GET: Account
        [Authorize(Roles = Shared.Constant.Roles.Accounts.Page)]
        public IActionResult Index()
        {
            return View();
        }

        // GET: Account/GetAll
        [Authorize(Roles = Shared.Constant.Roles.Accounts.Page)]
        public async Task<JsonResult> GetAll()
        {
            var result = await _accountsApiClient.GetAll();
            return Json(result);
        }

        // GET: Account/GetList
        [Authorize]
        public async Task<JsonResult> GetList()
        {
            var result = await _accountsApiClient.GetList();
            return Json(result);
        }

        // GET: Account/FindById/5
        [HttpGet]
        [Authorize]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _accountsApiClient.FindById(id);            
            return Json(result);
        }

        // POST: Account/Create
        [HttpPost]
        [Authorize(Roles = Shared.Constant.Roles.Accounts.Add)]
        public async Task<JsonResult> Add(Account account)
        {
            var result = await _accountsApiClient.Add(account);
            if (result != null && result.IsSuccess)
            {
                SuccessMessage("تمت الاضافة بنجاح");
            }
            else
            {
                ErrorMessage("لم تتم الاضافة");
            }
            return Json(result);
        }

        // POST: Items/Edit
        [HttpPost]
        [Authorize(Roles = Shared.Constant.Roles.Accounts.Update)]
        public async Task<JsonResult> Update(Account account)
        {
            var result = await _accountsApiClient.Update(account.Id, account);
            if (result != null && result.IsSuccess)
            {
                SuccessMessage("تم التعديل بنجاح");
            }
            else
            {
                ErrorMessage("لم يتم التعديل");
            }
            return Json(result);
        }

        // POST: Account/Delete/5
        [HttpPost]
        [Authorize(Roles = Shared.Constant.Roles.Accounts.Delete)]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _accountsApiClient.Delete(id);
            if (result != null && result.IsSuccess)
            {
                SuccessMessage("تم الحذف بنجاح");
            }
            else
            {
                ErrorMessage("لم يتم الحذف");
            }
            //return Json(new { success = true, message = "تم حذف العنصر بنجاح" });
            return Json(result);
        }
    }
}