namespace AccountingERP.Models
{
    public enum AccountType
    {
        Asset = 1,       // أصول
        Liability = 2,   // خصوم / التزامات
        Equity = 3,      // حقوق ملكية
        Revenue = 4,     // إيرادات
        Expense = 5      // مصروفات
    }

    public enum AccountNature
    {
        Debit = 1,  // مدين
        Credit = 2  // دائن
    }

    public enum DocumentType
    {
        JournalEntry = 1,      // قيد يومية عام
        ReceiptVoucher = 2,    // سند قبض
        PaymentVoucher = 3,    // سند صرف
        OpeningEntry = 4       // قيد افتتاحي
    }

    public enum DocumentStatus
    {
        Draft = 0,     // مسودة
        Posted = 1,    // مرحل
        Cancelled = 2  // ملغي
    }

    public static class SystemPermissions
    {
        public const string ViewAccounts = "Accounts.View";
        public const string ManageAccounts = "Accounts.Manage";
        public const string ViewDocuments = "Documents.View";
        public const string CreateDocuments = "Documents.Create";
        public const string EditDocuments = "Documents.Edit";
        public const string PostDocuments = "Documents.Post";
        public const string DeleteDocuments = "Documents.Delete";
        public const string ViewReports = "Reports.View";
        public const string ManageUsers = "Users.Manage";
        public const string ManageSettings = "Settings.Manage";
    }
}