using System.Net.Http.Headers;

namespace FinalProject.Plugins
{
    public class RestClient<T>:IRestClient<T> where T : BaseEntity
    {
        private MyService _myService;
        private readonly RestClientConfiguration _restClientConfiguration;

        public RestClient(MyService myService, RestClientConfiguration restClientConfiguration)
        {
            _myService = myService;
            _restClientConfiguration = restClientConfiguration;

        }

        public HttpClient GetClient()
        {
            //var httpClient = new HttpClient();
            var httpClient = _myService.GetHttpClient();
            /*if (!string.IsNullOrWhiteSpace(Program.Token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Program.Token);
            }*/

            return httpClient;
        }

        public async Task<Result<List<T>>> GetAsync(string serviceUrl)
        {
            Result<List<T>> Result = null;
            string Url = _restClientConfiguration.BaseUrl + serviceUrl;
            var httpClient = this.GetClient();

            //var json = await httpClient.GetStringAsync(Url);
            var result = await httpClient.GetAsync(Url);
            if (result.IsSuccessStatusCode)
            //if(result.StatusCode==System.Net.HttpStatusCode.OK)
            {
                //return result.IsSuccessStatusCode;
                var x = await result.Content.ReadAsStringAsync();
                Result = JsonConvert.DeserializeObject<Result<List<T>>>(x);
            }
            else
            {
                var code = result.StatusCode;
                var msg = result.Content.ReadAsStringAsync();
            }

            return Result;
        }

        public async Task<Result<T>> GetSingleAsync(string serviceUrl, Guid id)
        {
            Result<T> Result = default;

            string Url = _restClientConfiguration.BaseUrl + serviceUrl + id;
            var httpClient = this.GetClient();

            var result = await httpClient.GetAsync(Url);

            if (result.IsSuccessStatusCode)
            {
                var x = await result.Content.ReadAsStringAsync();
                Result = JsonConvert.DeserializeObject<Result<T>>(x);
            }
            else
            {
                var code = result.StatusCode;
                var msg = result.Content.ReadAsStringAsync();
            }
            return Result;
        }

        public async Task<Result<T>> PostAsync(string serviceUrl, T t)
        {
            Result<T> Result = default;
            string Url = _restClientConfiguration.BaseUrl + serviceUrl;
            var httpClient = this.GetClient();

            var json = JsonConvert.SerializeObject(t);

            HttpContent httpContent = new StringContent(json);

            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var result = await httpClient.PostAsync(Url, httpContent);
            if (result.IsSuccessStatusCode)
            //if(result.StatusCode==System.Net.HttpStatusCode.OK)
            {
                //return result.IsSuccessStatusCode;
                var x = await result.Content.ReadAsStringAsync();
                Result = JsonConvert.DeserializeObject<Result<T>>(x);
            }
            else
            {
                var code = result.StatusCode;
                var msg = result.Content.ReadAsStringAsync();
            }
            return Result;
        }

        public async Task<Result<TDest>> PostAsync<TDest>(string serviceUrl, T t)
        {
            Result<TDest> Result = default;
            string Url = _restClientConfiguration.BaseUrl + serviceUrl;
            var httpClient = this.GetClient();

            var json = JsonConvert.SerializeObject(t);

            HttpContent httpContent = new StringContent(json);

            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var result = await httpClient.PostAsync(Url, httpContent);
            if (result.IsSuccessStatusCode)
            //if(result.StatusCode==System.Net.HttpStatusCode.OK)
            {
                //return result.IsSuccessStatusCode;
                var x = await result.Content.ReadAsStringAsync();
                Result = JsonConvert.DeserializeObject<Result<TDest>>(x);
            }
            else
            {
                var code = result.StatusCode;
                var msg = result.Content.ReadAsStringAsync();
            }
            return Result;
        }

        public async Task<Result<T>> PutAsync(string serviceUrl, Guid id, T t)
        {
            Result<T> Result = default;
            string Url = _restClientConfiguration.BaseUrl + serviceUrl + id;
            var httpClient = this.GetClient();

            var json = JsonConvert.SerializeObject(t);

            HttpContent httpContent = new StringContent(json);

            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var result = await httpClient.PutAsync(Url, httpContent);

            // return result.IsSuccessStatusCode;
            //if(result.StatusCode==System.Net.HttpStatusCode.OK)

            if (result.IsSuccessStatusCode)
            {
                //return result.IsSuccessStatusCode;
                var x = await result.Content.ReadAsStringAsync();
                Result = JsonConvert.DeserializeObject<Result<T>>(x);
            }
            else
            {
                var code = result.StatusCode;
                var msg = result.Content.ReadAsStringAsync();
            }
            return Result;
        }

        public async Task<Result<T>> DeleteAsync(string serviceUrl, Guid id)
        {
            // var httpClient = this.GetClient();
            // var response = await httpClient.DeleteAsync(_restClientConfiguration.BaseUrl + id);
            // return response.IsSuccessStatusCode;

            Result<T> Result = default;

            string Url = _restClientConfiguration.BaseUrl + serviceUrl + id;
            var httpClient = this.GetClient();

            var result = await httpClient.DeleteAsync(Url);

            if (result.IsSuccessStatusCode)
            {

                var x = await result.Content.ReadAsStringAsync();
                Result = JsonConvert.DeserializeObject<Result<T>>(x);
            }
            else
            {
                var code = result.StatusCode;
                var msg = result.Content.ReadAsStringAsync();
            }
            return Result;
        }
    }
}
