using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using poplensUserAuthenticationApi.Models;
using poplensWebUIGateway.Helper;
using poplensWebUIGateway.Models.Media;

namespace poplensWebUIGateway.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase {
        private readonly HttpClient _httpClient;
        private readonly string _mediaApiUrl = "http://poplensMediaApi:8080/api/Media";
        private readonly string _userApiUrl = "http://poplensUserAuthenticationApi:8080/api/UserAuthentication";

        public SearchController(HttpClient httpClient) {
            _httpClient = httpClient;
        }

        [HttpGet("SearchMediaAndUsers")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))]
        public async Task<IActionResult> SearchMediaAndUsers(string query) {
            var client = HttpContext.Items["AuthorizedHttpClient"] as HttpClient;

            // Query media by name
            var mediaResponse = await client.GetAsync($"{_mediaApiUrl}/SearchMedia?query={query}");
            if (!mediaResponse.IsSuccessStatusCode) {
                return StatusCode((int)mediaResponse.StatusCode, await mediaResponse.Content.ReadAsStringAsync());
            }
            var mediaContent = await mediaResponse.Content.ReadAsStringAsync();
            var mediaResults = JsonConvert.DeserializeObject<List<Media>>(mediaContent);

            // Query users by username
            var userResponse = await client.GetAsync($"{_userApiUrl}/SearchUserByUsername/{query}");
            if (!userResponse.IsSuccessStatusCode) {
                return StatusCode((int)userResponse.StatusCode, await userResponse.Content.ReadAsStringAsync());
            }
            var userContent = await userResponse.Content.ReadAsStringAsync();
            var userResults = JsonConvert.DeserializeObject<List<User>>(userContent);

            return Ok(new { Media = mediaResults, Users = userResults });
        }
    }
}