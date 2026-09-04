namespace FinalProject.Plugins
{
    public interface IRestClient<T> where T : BaseEntity
    {
        Task<Result<List<T>>> GetAsync(string serviceUrl);

        Task<Result<T>> GetSingleAsync(string serviceUrl, Guid id);
        Task<Result<T>> PostAsync(string serviceUrl, T t);

        Task<Result<TDest>> PostAsync<TDest>(string serviceUrl, T t);        

        Task<Result<T>> PutAsync(string serviceUrl, Guid id, T t);

        Task<Result<T>> DeleteAsync(string serviceUrl, Guid id);        
    }
}
