namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersApiController : BaseController<User>
    {
        UserRepository _userRepository;        

        public UsersApiController (
            UserRepository userRepository            
            )
        {
            this._userRepository = userRepository;           
        }

        [HttpGet]
        [Authorize(Roles = Shared.Constant.Roles.Users.Page)]
        public async Task<Result<List<User>>> GetAll()
        {
            var userInfo = this.GetUserInfo();
            var result = await this._userRepository.GetAllAsync();
            if (result.IsSuccess && userInfo != null && userInfo.OrganizationId.HasValue)
            {
                result.Data = result.Data.Where(u => u.OrganizationId == userInfo.OrganizationId).ToList();
            }
            return result;
        }

        [HttpPost]
        public Result<string> Login([FromBody] User user)
        {
            var Result =  this._userRepository.LoginAsync(user);
            return Result;
        }

        [HttpPost]
        public async Task<Result<bool>> Register([FromBody] User user)
        {
            var Result = await this._userRepository.RegisterAsync(user);
            return Result;
        }
    }
}
