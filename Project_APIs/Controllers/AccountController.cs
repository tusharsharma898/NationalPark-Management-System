using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_APIs.Models;
using Project_APIs.Repository.IRepository;

namespace Project_APIs.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IUserRepository _userRepository;
        public AccountController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            if (ModelState.IsValid)
            {
                var isUniqueuser = _userRepository.IsUniqueUser(user.UserName);
                if(!isUniqueuser) return BadRequest("User Exist ");
                var userInfo = _userRepository.Register(user.UserName, user.Password);
                if (userInfo == null)
                    return NotFound();
                user = userInfo;
            }
            return Ok(user);
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody]User user)
        {
            var userInDb = _userRepository.Authenticate(user.UserName, user.Password);
            if (userInDb == null)
                return BadRequest("wrong user/password");
            return Ok(userInDb);
        }
    }
}
