using Cinereg.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Cinereg.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        readonly UserManager<ApplicationUser> _userManager;

        public UserController(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<ApplicationUser>> GetUser()
        {
            var authenticationResult = await _httpContextAccessor.HttpContext!.AuthenticateAsync("Identity.Bearer");
            if (!authenticationResult.Succeeded)
            {
                return Unauthorized("No token, malformed token or expired token given.");
            }
            var user = await _userManager.GetUserAsync(authenticationResult.Principal);

            if (user is null) return NotFound("No user found.");

            return Ok(user);
        }
    }
}
