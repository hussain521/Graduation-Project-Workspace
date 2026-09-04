namespace FinalProject.Plugins
{
    public class MyService
    {
        private HttpClient _httpClient;
        public MyService()
        {

        }

        public MyService(HttpClient httpClient)
        {
            /*var handler = new HttpClientHandler()
            {
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls
            };
             httpClient=new HttpClient(handler);*/
            _httpClient = httpClient;
            _httpClient.Timeout = new TimeSpan(0, 10, 0);
        }

        public HttpClient GetHttpClient()
        {
            return _httpClient;
        }
    }
}
