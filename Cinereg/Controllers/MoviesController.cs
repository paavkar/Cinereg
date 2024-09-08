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
        readonly UserManager<ApplicationUser> _userManager;

        public MoviesController(IMovieService movieService, UserManager<ApplicationUser> userManager)
        {
            _movieService = movieService;
            _userManager = userManager;
        }

        // GET: api/Movies
        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<MovieWithGenres>>> GetMovies()
        {
            var authenticationResult = await HttpContext.AuthenticateAsync("Identity.Bearer");
            if (!authenticationResult.Succeeded)
            {
                return Unauthorized("No token, malformed token or expired token given.");
            }

            var user = await _userManager.GetUserAsync(authenticationResult.Principal);
            return await _movieService.GetAllMovies(user!.Id);
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieWithGenres>> GetMovie(string id)
        {
            var authenticationResult = await HttpContext.AuthenticateAsync("Identity.Bearer");
            if (!authenticationResult.Succeeded)
            {
                return Unauthorized("No token, malformed token or expired token given.");
            }
            var user = await _userManager.GetUserAsync(authenticationResult.Principal);
            var movie = await _movieService.GetMovieById(id);

            if (movie is null)
            {
                return NotFound("No movie found with given Id.");
            }

            if (movie.UserId != user!.Id)
            {
                return Unauthorized("Authenticated user's and movie's user do not match.");
            }

            return movie;
        }

        // PUT: api/Movies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult> PutMovie(string id, [FromBody] MovieWithGenres movie)
        {
            var authenticationResult = await HttpContext.AuthenticateAsync("Identity.Bearer");
            if (!authenticationResult.Succeeded)
            {
                return Unauthorized("No token, malformed token or expired token given.");
            }
            var user = await _userManager.GetUserAsync(authenticationResult.Principal);

            if (id != movie.Id)
            {
                return BadRequest();
            }

            if (movie.UserId != user!.Id)
            {
                return Unauthorized("Authenticated user's and movie's user do not match.");
            }

            var updatedMovie = await _movieService.UpdateMovie(id, movie);
            return Ok(updatedMovie);
        }

        // POST: api/Movies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MovieWithGenres>> PostMovie([FromBody] MovieWithGenres movie)
        {
            var authenticationResult = await HttpContext.AuthenticateAsync("Identity.Bearer");
            if (!authenticationResult.Succeeded)
            {
                return Unauthorized("No token, malformed token or expired token given.");
            }
            var user = await _userManager.GetUserAsync(authenticationResult.Principal);

            movie.UserId = user!.Id;

            await _movieService.AddMovie(movie);

            return CreatedAtAction("GetMovie", new { id = movie.Id }, movie);
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMovie(string id)
        {
            var authenticationResult = await HttpContext.AuthenticateAsync("Identity.Bearer");
            if (!authenticationResult.Succeeded)
            {
                return Unauthorized("No token, malformed token or expired token given.");
            }
            var user = await _userManager.GetUserAsync(authenticationResult.Principal);

            await _movieService.DeleteMovie(id);

            return NoContent();
        }
    }
}
