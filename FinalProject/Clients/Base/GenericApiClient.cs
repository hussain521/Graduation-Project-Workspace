namespace FinalProject.Clients.Base
{
    public class GenericApiClient<T> : IGenericApiClient<T> where T : BaseEntity
    {
        protected IRestClient<T> _restClient;
        protected virtual string GetControllerName()
        {
            return string.Empty;
        }

        public GenericApiClient(IRestClient<T> restClient)
        {
            _restClient = restClient;
        }

        public virtual async Task<Result<T>> Add(T entity)
        {
            var Result = await _restClient.PostAsync(this.GetControllerName() + "/Add", entity);
            return Result;
        }

        public virtual async Task<Result<T>> Update(Guid id,T entity)
        {
            var Result = await _restClient.PutAsync(this.GetControllerName() + "/Update?id=" ,id, entity);
            return Result;
        }

        public virtual async Task<Result<T>> Delete(Guid id)
        {
            var Result = await _restClient.DeleteAsync(this.GetControllerName() + "/Delete/", id);
            return Result;
        }

        public virtual async Task<Result<T>> FindById(Guid id)
        {
            var Result = await _restClient.GetSingleAsync(this.GetControllerName() + "/FindById/", id);
            return Result;
        }

        public async Task<Result<List<T>>> GetAll()
        {            
            var Result = await _restClient.GetAsync(this.GetControllerName() + "/GetAll");
            return Result;
        }

        public async Task<Result<List<T>>> GetList()
        {
            var Result = await _restClient.GetAsync(this.GetControllerName() + "/GetList");
            return Result;
        }

        /*public virtual async Task<Result<T>> Navigate(T entity)
        {
            var result = await _restClient.PostAsyncVar(this.GetControllerName() + "Navigate", entity);
            return result;
        }

        public virtual async Task<Result<List<T>>> GetAll(T entity)
        {
            var result = await _restClient.PostAsyncList(this.GetControllerName() + "GetAll", entity);
            return result;
        }

        public virtual async Task<Result<List<T>>> GetAutocompleteList(T entity)
        {
            var result = await _restClient.PostAsyncList(this.GetControllerName() + "GetAutocompleteList", entity);
            return result;
        }

        public virtual async Task<PagedResult<List<T>>> GetPage()
        {
            var result = await _restClient.GetAsyncPagedList(this.GetControllerName() + "GetPage");
            return result;
        }

        public virtual async Task<PagedResult<List<T>>> GetPage(PagedRequest<T> entity)
        {
            var result = await _restClient.PostAsyncPagesList(this.GetControllerName() + "GetPage", entity);
            return result;
        }*/
    }
}
