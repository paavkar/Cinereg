using Cinereg.Data;
using Cinereg.Models;
using Cinereg.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cinereg.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        readonly UserManager<ApplicationUser> _userManager;

        public MoviesController(IMovieService movieService, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _movieService = movieService;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        // GET: api/Movies
        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            var authenticationResult = await _httpContextAccessor.HttpContext.AuthenticateAsync("Identity.Bearer");
            if (!authenticationResult.Succeeded)
            {
                return Unauthorized("No token given or expired token given.");
            }

            var user = await _userManager.GetUserAsync(authenticationResult.Principal);
            return await _movieService.GetAllMovies(user.Id);
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(string id)
        {
            var authenticationResult = await _httpContextAccessor.HttpContext.AuthenticateAsync("Identity.Bearer");
            if (!authenticationResult.Succeeded)
            {
                return Unauthorized("No token given or expired token given.");
            }
            var user = await _userManager.GetUserAsync(authenticationResult.Principal);
            var movie = await _movieService.GetMovieById(id);

            if (movie is null)
            {
                return NotFound("No movie found with given Id.");
            }

            if (movie.UserId != user.Id)
            {
                return Unauthorized("Authenticated user's and movie's user do not match.");
            }

            return movie;
        }

        // PUT: api/Movies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(string id, [FromBody] Movie movie)
        {
            var authenticationResult = await _httpContextAccessor.HttpContext!.AuthenticateAsync("Identity.Bearer");
            if (!authenticationResult.Succeeded)
            {
                return Unauthorized("No token given or expired token given.");
            }
            var user = await _userManager.GetUserAsync(authenticationResult.Principal);

            if (id != movie.Id)
            {
                return BadRequest();
            }

            if (movie.UserId != user.Id)
            {
                return Unauthorized("Authenticated user's and movie's user do not match.");
            }

            await _movieService.UpdateMovie(id, movie);

            return Ok();
        }

        // POST: api/Movies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie([FromBody] Movie movie)
        {
            var authenticationResult = await _httpContextAccessor.HttpContext.AuthenticateAsync("Identity.Bearer");
            if (!authenticationResult.Succeeded)
            {
                return Unauthorized("No token given or expired token given.");
            }

            await _movieService.AddMovie(movie);

            return CreatedAtAction("GetMovie", new { id = movie.Id }, movie);
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(string id)
        {
            var authenticationResult = await _httpContextAccessor.HttpContext.AuthenticateAsync("Identity.Bearer");
            if (!authenticationResult.Succeeded)
            {
                return Unauthorized("No token given or expired token given.");
            }
            var user = await _userManager.GetUserAsync(authenticationResult.Principal);

            await _movieService.DeleteMovie(id);

            return NoContent();
        }

        //public async Task<IActionResult> GetUser()
        //{
        //    var authenticationResult = await _httpContextAccessor.HttpContext.AuthenticateAsync("Identity.Bearer");
        //    if (!authenticationResult.Succeeded)
        //    {
        //        return Unauthorized("No token given or expired token given.");
        //    }
        //    var user = await _userManager.GetUserAsync(authenticationResult.Principal);

        //    return user;
        //}
    }
}
