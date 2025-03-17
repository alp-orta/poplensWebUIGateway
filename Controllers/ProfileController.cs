using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace poplensWebUIGateway.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase {
        private readonly HttpClient _httpClient;
        private readonly string _userProfileApiUrl = "https://localhost:7056/api/Profile";

        public ProfileController(HttpClient httpClient) {
            _httpClient = httpClient;
        }

        [HttpGet("{profileId}")]
        public async Task<IActionResult> GetProfile(string profileId) {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token)) {
                return Unauthorized(new { Message = "Authorization token is missing" });
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"{_userProfileApiUrl}/{profileId}");

            if (!response.IsSuccessStatusCode) {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            return Ok(await response.Content.ReadAsStringAsync());
        }
    }
}
