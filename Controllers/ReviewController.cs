using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using poplensUserProfileApi.Models.Dtos;
using System.Net.Http.Headers;
using System.Text;

namespace poplensWebUIGateway.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase {
        private readonly HttpClient _httpClient;
        private readonly string _reviewApiUrl = "https://localhost:7056/api/Review";

        public ReviewController(HttpClient httpClient) {
            _httpClient = httpClient;
        }

        [HttpPost("{profileId}/addReview")]
        public async Task<IActionResult> AddReview(Guid profileId, [FromBody] CreateReviewRequest request) {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token)) {
                return Unauthorized(new { Message = "Authorization token is missing" });
            }
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jsonContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_reviewApiUrl}/{profileId}/addReview", jsonContent);

            if (!response.IsSuccessStatusCode) {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }


            var createdReview = JsonConvert.DeserializeObject<CreateReviewRequest>(await response.Content.ReadAsStringAsync());
            return Created($"{_reviewApiUrl}/{profileId}/addReview", createdReview);

        }

    }
}
