namespace FinalProject.Controllers
{
    public class ItemsController : BaseController<Item>
    {
        ItemsApiClient _itemsApiClient;
        public ItemsController(ItemsApiClient itemsApiClient) : base(itemsApiClient)
        {
            _itemsApiClient = itemsApiClient;
        }

        // GET: Item
        public IActionResult Index()
        {
            return View();
        }

        // GET: Item/GetAll
        public async Task<JsonResult> GetAll()
        {
            var result = await _itemsApiClient.GetAll();
            return Json(result);
        }

        // GET: Item/GetList
        public async Task<JsonResult> GetList()
        {
            var result = await _itemsApiClient.GetList();
            return Json(result);
        }

        // GET: Item/FindById/5
        [HttpGet]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _itemsApiClient.FindById(id);
            return Json(result);
        }

        // POST: Item/Create
        [HttpPost]
        public async Task<JsonResult> Add(Item item)
        {
            var result = await _itemsApiClient.Add(item);
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
        public async Task<JsonResult> Update(Item item)
        {
            var result = await _itemsApiClient.Update(item.Id, item);
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

        // POST: Item/Delete/5
        [HttpPost]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _itemsApiClient.Delete(id);
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