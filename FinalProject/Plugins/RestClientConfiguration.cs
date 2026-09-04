namespace FinalProject.Plugins
{
    public class RestClientConfiguration
    {
        public string BaseUrl { get; set; }
        public RestClientConfiguration()
        {
            #region Mazen
            this.BaseUrl = "http://localhost:5207/api/";            
            //this.BaseUrl = "http://daftarapi.litesoftit.com/api/";
            #endregion
        }
    }
}
