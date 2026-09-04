namespace FinalProject.Clients
{
    public class AccountsApiClient : GenericApiClient<Account>
    {
        public AccountsApiClient(IRestClient<Account> restClient) : base(restClient)
        {

        }
        protected override string GetControllerName()
        {
            return "AccountsApi";
        }
    }
}
