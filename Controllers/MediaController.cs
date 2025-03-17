using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace poplensWebUIGateway.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase {
        private readonly HttpClient _httpClient;
        private readonly string _mediaApiUrl = "https://localhost:7207/api/Media";

        public MediaController(HttpClient httpClient) {
            _httpClient = httpClient;
        }

        [HttpGet("SearchMedia")]
        public async Task<IActionResult> SearchMedia(string mediaType, string query) {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token)) {
                return Unauthorized(new { Message = "Authorization token is missing" });
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = new HttpResponseMessage();
            if (mediaType == "media") {
                response = await _httpClient.GetAsync($"{_mediaApiUrl}/SearchMedia?query={query}");
            } else if (mediaType == "film") {
                response = await _httpClient.GetAsync($"{_mediaApiUrl}/SearchFilms?query={query}");
            } else if (mediaType == "book") {
                response = await _httpClient.GetAsync($"{_mediaApiUrl}/SearchBooks?query={query}");
            } else if (mediaType == "game") {
                response = await _httpClient.GetAsync($"{_mediaApiUrl}/SearchGames?query={query}");
            } else {
                return BadRequest(new { Message = "Invalid media type" });
            }

            if (!response.IsSuccessStatusCode) {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            return Ok(await response.Content.ReadAsStringAsync());
        }

    }
}