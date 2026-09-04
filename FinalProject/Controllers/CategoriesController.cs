namespace FinalProject.Controllers
{
    public class CategoriesController : BaseController<Category>
    {
        CategoriesApiClient _categoriesApiClient;
        public CategoriesController(CategoriesApiClient categoriesApiClient) : base(categoriesApiClient)
        {
            _categoriesApiClient = categoriesApiClient;
        }

        // GET: Category
        public IActionResult Index()
        {
            return View();
        }

        // GET: Category/GetAll
        public async Task<JsonResult> GetAll()
        {
            var result = await _categoriesApiClient.GetAll();
            return Json(result);
        }

        // GET: Category/GetList
        public async Task<JsonResult> GetList()
        {
            var result = await _categoriesApiClient.GetList();
            return Json(result);
        }

        // GET: Category/FindById/5
        [HttpGet]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _categoriesApiClient.FindById(id);
            return Json(result);
        }

        // POST: Category/Create
        [HttpPost]
        public async Task<JsonResult> Add(Category category)
        {
            var result = await _categoriesApiClient.Add(category);
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
        public async Task<JsonResult> Update(Category category)
        {
            var result = await _categoriesApiClient.Update(category.Id, category);
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

        // POST: Category/Delete/5
        [HttpPost]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _categoriesApiClient.Delete(id);
            if (result != null && result.IsSuccess)
            {
                SuccessMessage("تمت الحذف بنجاح");
            }
            else
            {
                ErrorMessage("لم يتم الحذف");
            }
            return Json(new { success = true, message = "تم حذف العنصر بنجاح" });
        }
    }
}