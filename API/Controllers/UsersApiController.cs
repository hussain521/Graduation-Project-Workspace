using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.UnifiedResult;

namespace API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersApiController : BaseController<User>
    {
        private readonly UserRepository _userRepository;

        public UsersApiController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        [Authorize(Roles = Shared.Constant.Roles.Users.Page)]
        public async Task<Result<List<User>>> GetAll()
        {
            var userInfo = GetUserInfo();

            var result = await _userRepository.GetAllAsync();

            if (!result.IsSuccess)
            {
                return result;
            }

            if (userInfo != null && userInfo.OrganizationId.HasValue)
            {
                var users = result.Data?
                    .Where(u => u.OrganizationId == userInfo.OrganizationId.Value)
                    .ToList()
                    ?? new List<User>();

                return new Result<List<User>>(
                    true,
                    result.Error,
                    users
                );
            }

            return result;
        }

        [HttpPost]
        public Result<string> Login([FromBody] User user)
        {
            var result = _userRepository.LoginAsync(user);

            return result;
        }

        [HttpPost]
        public async Task<Result<bool>> Register([FromBody] User user)
        {
            var result = await _userRepository.RegisterAsync(user);

            return result;
        }
    }
}