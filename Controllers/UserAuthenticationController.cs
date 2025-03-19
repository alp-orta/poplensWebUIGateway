using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using poplensUserAuthenticationApi.Models.Dtos;
using System.Net.Http.Headers;
using System.Text;

namespace poplensWebUIGateway.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class UserAuthenticationController : ControllerBase {
        private readonly HttpClient _httpClient;
        private readonly string _authApiUrl = "https://localhost:7019/api/UserAuthentication";

        public UserAuthenticationController(HttpClient httpClient) {
            _httpClient = httpClient;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterInfo user) {
            var response = await _httpClient.PostAsync($"{_authApiUrl}/Register",
                new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));

            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginInfo user) {
            var response = await _httpClient.PostAsync($"{_authApiUrl}/Login",
                new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json"));

            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }

        [HttpGet("FetchIdsFromUsername/{username}")]
        public async Task<IActionResult> FetchIdsFromUsername(string username) {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token)) {
                return Unauthorized(new { Message = "Authorization token is missing" });
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"{_authApiUrl}/FetchIdsFromUsername/{username}");

            var content = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, content);
        }

        [HttpGet("ProtectedEndpoint")]
        public async Task<IActionResult> ProtectedEndpoint() {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token)) {
                return Unauthorized(new { Message = "Authorization token is missing" });
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"{_authApiUrl}/ProtectedData");

            if (!response.IsSuccessStatusCode) {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            return Ok(await response.Content.ReadAsStringAsync());
        }
    }
}
