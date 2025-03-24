using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using poplensUserProfileApi.Models.Dtos;
using poplensWebUIGateway.Helper;
using System.Text;

namespace poplensWebUIGateway.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase {
        private readonly HttpClient _httpClient;
        private readonly string _reviewApiUrl = "http://poplensUserProfileApi:8080/api/Review";

        public ReviewController(HttpClient httpClient) {
            _httpClient = httpClient;
        }

        [HttpPost("{profileId}/addReview")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))]
        public async Task<IActionResult> AddReview(Guid profileId, [FromBody] CreateReviewRequest request) {
            var client = HttpContext.Items["AuthorizedHttpClient"] as HttpClient;

            var jsonContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_reviewApiUrl}/{profileId}/addReview", jsonContent);

            if (!response.IsSuccessStatusCode) {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }


            var createdReview = JsonConvert.DeserializeObject<CreateReviewRequest>(await response.Content.ReadAsStringAsync());
            return Created($"{_reviewApiUrl}/{profileId}/addReview", createdReview);

        }

    }
}
