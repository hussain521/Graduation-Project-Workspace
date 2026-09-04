using Domain.Entities.Base;

namespace FinalProject.Clients
{
    public class DocumentsApiClient : GenericApiClient<Document>
    {
        public DocumentsApiClient(IRestClient<Document> restClient) : base(restClient)
        {

        }
        protected override string GetControllerName()
        {
            return "DocumentsApi";
        }

        public async Task<Result<Document>> RefreshSerialNum(Document entity)
        {
            var Result = await _restClient.PostAsync<Document>(this.GetControllerName() + "/RefreshSerialNum", entity);
            return Result;
        }

        public async Task<Result<List<AccountStatementViewModel>>> GetAccountStatement(Document entity)
        {
            var Result = await _restClient.PostAsync<List<AccountStatementViewModel>>(this.GetControllerName() + "/GetAccountStatement", entity);
            return Result;
        }
    }
}
