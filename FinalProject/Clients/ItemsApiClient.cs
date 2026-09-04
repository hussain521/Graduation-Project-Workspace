namespace FinalProject.Clients
{
    public class ItemsApiClient : GenericApiClient<Item>
    {
        public ItemsApiClient(IRestClient<Item> restClient) : base(restClient)
        {

        }

        protected override string GetControllerName()
        {
            return "ItemsApi";
        }
    }
}