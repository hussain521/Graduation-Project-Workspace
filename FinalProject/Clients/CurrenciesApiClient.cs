namespace FinalProject.Clients
{
    public class CurrenciesApiClient : GenericApiClient<Currency>
    {
        public CurrenciesApiClient(IRestClient<Currency> restClient) : base(restClient)
        {
        }

        protected override string GetControllerName()
        {
            return "CurrenciesApi";
        }
    }
}
