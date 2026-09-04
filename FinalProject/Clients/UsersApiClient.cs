namespace FinalProject.Clients
{
    public class UsersApiClient : GenericApiClient<User>
    {
        public UsersApiClient(IRestClient<User> restClient) : base(restClient)
        {

        }

        protected override string GetControllerName()
        {
            return "UsersApi";
        }

        public async Task<Result<bool>> Register(User entity)
        {
            var Result = await _restClient.PostAsync<bool>(this.GetControllerName() + "/Register", entity);
            return Result;
        }

        public async Task<Result<string>> Login(User entity)
        {
            var Result = await _restClient.PostAsync<string>(this.GetControllerName() + "/Login", entity);
            return Result;
        }
    }
}