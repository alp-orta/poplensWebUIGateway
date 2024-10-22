using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using poplensUserAuthenticationApi.Models.Dtos;
using System.Text;

namespace poplensWebUIGateway.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class UserAuthenticationController : ControllerBase {
        private readonly HttpClient _httpClient;

        public UserAuthenticationController(HttpClient httpClient) {
            _httpClient = httpClient;
        }

        private readonly string _authApiUrl = "https://localhost:7019/api/UserAuthentication"; // Change to your authentication API URL

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
    }
}
