namespace FinalProject.RL
{
    public partial class AccountReport : DevExpress.XtraReports.UI.XtraReport
    {
        public AccountReport(List<AccountStatementViewModel> Source)
        {
            InitializeComponent();

            LblDocDate.DataBindings.Add("Text", Source, "Date", "{0:dd/MM/yyyy}");                        
            LblDescTxt.DataBindings.Add("Text", Source, "Description");

            LblDebit.DataBindings.Add("Text", Source, "Debit");
            LblCredit.DataBindings.Add("Text", Source, "Credit");
            LblBalance.DataBindings.Add("Text", Source, "Balance");            
        }
    }
}
