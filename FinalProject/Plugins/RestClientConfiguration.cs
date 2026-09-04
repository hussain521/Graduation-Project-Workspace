namespace FinalProject.Plugins
{
    public class RestClientConfiguration
    {
        public string BaseUrl { get; set; }
        public RestClientConfiguration()
        {
            #region Mazen
            this.BaseUrl = "https://localhost:7291/api/";            
            //this.BaseUrl = "http://daftarapi.litesoftit.com/api/";
            #endregion
        }
    }
}
