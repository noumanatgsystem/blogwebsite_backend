using Application.Interface.User;
using Microsoft.AspNetCore.Mvc;

namespace blogging_Website.Controllers.AppUser
{
    [Route("api/AppUser")]
    [ApiController]
    public class AppUserController : ControllerBase
    {
        private IAppUser _userService;
        public AppUserController(IAppUser userService)
        {
            _userService = userService;
        }

        [HttpGet("Helloword")]
        public IActionResult sayHello()
        {
            return Ok("Hello");
        }
    }
}
