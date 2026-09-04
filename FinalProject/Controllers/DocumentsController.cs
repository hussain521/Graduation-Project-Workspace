namespace FinalProject.Controllers
{
    public class DocumentsController : BaseController<Document>
    {
        DocumentsApiClient _documentsApiClient;
        public DocumentsController(DocumentsApiClient DocumentsApiClient) : base(DocumentsApiClient)
        {
            _documentsApiClient = DocumentsApiClient;
        }

        // GET: Document
        [Authorize(Roles = Shared.Constant.Roles.Documents.Page)]
        public IActionResult DebititNofication()
        {
            return View();
        }

        [Authorize(Roles = Shared.Constant.Roles.Documents.Page)]
        public IActionResult CreditNofication()
        {
            return View();
        }

        // GET: Document/GetAll
        [Authorize(Roles = Shared.Constant.Roles.Documents.Page)]
        public async Task<JsonResult> GetAll()
        {
            var result = await _documentsApiClient.GetAll();
            return Json(result);
        }

        // GET: Document/GetList
        [Authorize]
        public async Task<JsonResult> GetList()
        {
            var result = await _documentsApiClient.GetList();
            return Json(result);
        }

        // GET: Document/FindById/5
        [HttpGet]
        [Authorize]
        public async Task<JsonResult> FindById(Guid id)
        {
            var result = await _documentsApiClient.FindById(id);
            return Json(result);
        }

        // POST: Document/Create
        [HttpPost]
        //[Authorize(Roles = Shared.Constant.Roles.Documents.Add)]
        public async Task<JsonResult> Add(Document document)//DocumentDTO documentDTO)
        {
            //Document document = new Document();
            var result = await _documentsApiClient.Add(document);
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
        [Authorize(Roles = Shared.Constant.Roles.Documents.Update)]
        public async Task<JsonResult> Update(Document document)
        {
            var result = await _documentsApiClient.Update(document.Id, document);
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

        // POST: Document/Delete/5
        [HttpPost]
        [Authorize(Roles = Shared.Constant.Roles.Documents.Delete)]
        public async Task<JsonResult> Delete(Guid id)
        {
            var result = await _documentsApiClient.Delete(id);
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

        [HttpPost]
        public async Task<JsonResult> RefreshSerialNum(DocumentDTO documentDTO)
        {
            Document document = new Document
            {
                TypeId= documentDTO.TypeId,                
            };
            var result = await _documentsApiClient.RefreshSerialNum(document);
            if (result != null && result.IsSuccess)
            {
                //SuccessMessage("تمت الاضافة بنجاح");
            }
            else
            {
                ErrorMessage("غير قادر على استرجاع رقم السند");
            }
            return Json(result);
        }

        //عملي جديد

        [Authorize]
        public IActionResult AccountStatement() 
        {
            return View();
        }
        //عملي جديد
        
        [HttpPost]
        [Authorize]
        public async Task<JsonResult> GetAccountStatement(Document document)
        {
            var result = await _documentsApiClient.GetAccountStatement(document);
            return Json(result);
        }
    }
}
