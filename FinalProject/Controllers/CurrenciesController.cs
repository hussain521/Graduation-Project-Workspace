namespace FinalProject.Controllers
{
    public class CurrenciesController : BaseController<Currency>
    {
        CurrenciesApiClient _currenciesApiClient;
        public CurrenciesController(CurrenciesApiClient currenciesApiClient) : base(currenciesApiClient)
        {
            _currenciesApiClient = currenciesApiClient;
        }

        // GET: Currency
        [Authorize(Roles = Shared.Constant.Roles.Currencies.Page)]
        public IActionResult Index()
        {
            return View();
        }

        // GET: Currency/GetAll
        [Authorize(Roles = Shared.Constant.Roles.Currencies.Page)]
        public async Task<JsonResult> GetAll()
        {
            var result = await _currenciesApiClient.GetAll();
            return Json(result);
        }

        // GET: Currency/GetList
        [Authorize]
        public async Task<JsonResult> GetList()
        {
            var result = await _currenciesApiClient.GetList();
            return Json(result);
        }

        // GET: Currency/FindById/5
        [HttpGet]
        [Authorize]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _currenciesApiClient.FindById(id);
            return Json(result);
        }

        // POST: Currency/Create
        [HttpPost]
        [Authorize(Roles = Shared.Constant.Roles.Currencies.Page)]
        public async Task<JsonResult> Add(Currency currency)
        {
            var result = await _currenciesApiClient.Add(currency);
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
        [Authorize(Roles = Shared.Constant.Roles.Currencies.Page)]
        public async Task<JsonResult> Update(Currency currency)
        {
            var result = await _currenciesApiClient.Update(currency.Id, currency);
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

        // POST: Currency/Delete/5
        [HttpPost]
        [Authorize(Roles = Shared.Constant.Roles.Currencies.Page)]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _currenciesApiClient.Delete(id);
            if (result != null && result.IsSuccess)
            {
                SuccessMessage("تمت الحذف بنجاح");
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