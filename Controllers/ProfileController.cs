using Microsoft.AspNetCore.Mvc;
using poplensWebUIGateway.Helper;

namespace poplensWebUIGateway.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase {
        private readonly HttpClient _httpClient;
        private readonly string _userProfileApiUrl = "https://localhost:7056/api/Profile";

        public ProfileController(HttpClient httpClient) {
            _httpClient = httpClient;
        }

        [HttpGet("GetProfile/{profileId}")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))]
        public async Task<IActionResult> GetProfile(string profileId) {
            var client = HttpContext.Items["AuthorizedHttpClient"] as HttpClient;
            var response = await client.GetAsync($"{_userProfileApiUrl}/GetProfile/{profileId}");

            if (!response.IsSuccessStatusCode) {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            return Ok(await response.Content.ReadAsStringAsync());
        }

        [HttpPost("{followerId}/follow/{followingId}")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))]
        public async Task<IActionResult> Follow(Guid followerId, Guid followingId) {
            var client = HttpContext.Items["AuthorizedHttpClient"] as HttpClient;
            var response = await client.PostAsync($"{_userProfileApiUrl}/{followerId}/follow/{followingId}", null);

            if (!response.IsSuccessStatusCode) {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            return Ok(new { Message = "Successfully followed the user" });
        }

        [HttpDelete("{followerId}/unfollow/{followingId}")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))]
        public async Task<IActionResult> Unfollow(Guid followerId, Guid followingId) {
            var client = HttpContext.Items["AuthorizedHttpClient"] as HttpClient;
            var response = await client.DeleteAsync($"{_userProfileApiUrl}/{followerId}/unfollow/{followingId}");

            if (!response.IsSuccessStatusCode) {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            return Ok(new { Message = "Successfully unfollowed the user" });
        }
    }
}
