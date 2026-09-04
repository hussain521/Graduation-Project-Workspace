namespace FinalProject.Clients.Base
{
    public interface IGenericApiClient<T> where T : BaseEntity
    {
        public Task<Result<T>> Add(T entity);
        public Task<Result<T>> Update(Guid id,T entity);

        public Task<Result<T>> Delete(Guid id);

        public Task<Result<T>> FindById(Guid id);

        /*public Task<Result<List<T>>> FindByNameAsList(T entity);

        public Task<Result<string>> FindByNameAsJson(T entity);

        //public Task<Result<T>> FindByNameOrId(T entity);

        public Task<Result<string>> FindByNameOrIdAsJson(T entity);

        //public Task<Result<List<T>>> FindByNameOrIdAsList(T entity);        

        public Task<Result<T>> Navigate(T entity);

        public Task<Result<List<T>>> GetAll();

        public Task<Result<List<T>>> GetAll(T entity);

        public Task<Result<List<T>>> GetAutocompleteList(T entity);

        public Task<PagedResult<List<T>>> GetPage();*/
    }
}
