using DevExpress.XtraReports;

namespace FinalProject.Controllers
{
    public class ReportsController : Controller
    {
        DocumentsApiClient client;
        public ReportsController(DocumentsApiClient client)
        {
            this.client = client;
        }
        public IActionResult AccountsReport()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AccountStateReport(Document document)
        {
            var Result =await client.GetAccountStatement(document);
            RL.AccountReport Rpt = new RL.AccountReport(Result.Data);
            return PartialView("~/Views/Home/ShowReport.cshtml", Rpt);
        }
    }
}
