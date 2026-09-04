namespace FinalProject.Clients
{
    public class CategoriesApiClient : GenericApiClient<Category>
    {
        public CategoriesApiClient(IRestClient<Category> restClient) :base(restClient) 
        {
                
        }
        protected override string GetControllerName()
        {
            return "CategoriesApi";
        }
    }
}
