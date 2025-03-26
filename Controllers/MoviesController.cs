using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieBuddy.DTOs;
using MovieBuddy.Service;

namespace MovieBuddy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : ControllerBase
    {
        private readonly ITheMovieDbService _movieDbService;

        public MoviesController(ITheMovieDbService movieDbService)
        {
            _movieDbService = movieDbService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] SearchRequestDto request)
        {
            var result = await _movieDbService.SearchMoviesAsync(request);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetails(int id, [FromQuery] bool isTvShow = false)
        {
            var movie = await _movieDbService.GetMovieDetailsAsync(id, isTvShow);
            return Ok(movie);
        }

        [HttpGet("recommendations/{userId}")]
        public async Task<IActionResult> GetRecommendations(int userId)
        {
            var recommendations = await _movieDbService.GetRecommendationsAsync(userId);
            return Ok(recommendations);
        }

        [HttpGet("genres")]
        public async Task<IActionResult> GetAllGenres()
        {
            try
            {
                var genres = await _movieDbService.GetAllGenresAsync();
                return Ok(genres);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}/similar")]
        public async Task<IActionResult> GetSimilarMovies(int id, [FromQuery] bool isTvShow = false)
        {
            var similarMovies = await _movieDbService.GetSimilarMoviesAsync(id, isTvShow);
            return Ok(similarMovies);
        }
    }
}
