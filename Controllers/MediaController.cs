using Microsoft.AspNetCore.Mvc;
using poplensWebUIGateway.Helper;

namespace poplensWebUIGateway.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase {
        private readonly HttpClient _httpClient;
        private readonly string _mediaApiUrl = "http://poplensMediaApi:8080/api/Media";

        public MediaController(HttpClient httpClient) {
            _httpClient = httpClient;
        }

        [HttpGet("SearchMedia")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))]
        public async Task<IActionResult> SearchMedia(string mediaType, string query) {
            var client = HttpContext.Items["AuthorizedHttpClient"] as HttpClient;

            var response = new HttpResponseMessage();
            if (mediaType == "media") {
                response = await client.GetAsync($"{_mediaApiUrl}/SearchMedia?query={query}");
            } else if (mediaType == "film") {
                response = await client.GetAsync($"{_mediaApiUrl}/SearchFilms?query={query}");
            } else if (mediaType == "book") {
                response = await client.GetAsync($"{_mediaApiUrl}/SearchBooks?query={query}");
            } else if (mediaType == "game") {
                response = await client.GetAsync($"{_mediaApiUrl}/SearchGames?query={query}");
            } else {
                return BadRequest(new { Message = "Invalid media type" });
            }

            if (!response.IsSuccessStatusCode) {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            return Ok(await response.Content.ReadAsStringAsync());
        }

        [HttpGet("GetMediaWithFilters")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))]
        public async Task<IActionResult> GetMediaWithFilters(
            [FromQuery] string mediaType,
            [FromQuery] string? decade,
            [FromQuery] string? genre,
            [FromQuery] string? sortBy,
            [FromQuery] string? query,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10) {

            var client = HttpContext.Items["AuthorizedHttpClient"] as HttpClient;
            var response = await client.GetAsync($"{_mediaApiUrl}/GetMediaWithFilters?mediaType={mediaType}&decade={decade}&genre={genre}&sortBy={sortBy}&query={query}&page={page}&pageSize={pageSize}");

            if (!response.IsSuccessStatusCode) {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            return Ok(await response.Content.ReadAsStringAsync());
        }
    }
}