using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using poplensUserProfileApi.Models.Dtos;
using poplensWebUIGateway.Helper;
using poplensWebUIGateway.Models.Common;
using poplensWebUIGateway.Models.Profile;
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

        [HttpPost("{profileId}/AddReview")]
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

        [HttpDelete("{profileId}/DeleteReview/{mediaId}")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))]
        public async Task<IActionResult> DeleteReview(Guid profileId, string mediaId) {
            var client = HttpContext.Items["AuthorizedHttpClient"] as HttpClient;

            var response = await client.DeleteAsync($"{_reviewApiUrl}/{profileId}/DeleteReview/{mediaId}");

            if (!response.IsSuccessStatusCode) {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound) {
                    return NotFound();
                }
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            return NoContent();
        }

        [HttpGet("GetMediaMainPageReviewInfo/{mediaId}")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))]
        public async Task<IActionResult> GetMediaMainPageReviewInfo(string mediaId) {
            var client = HttpContext.Items["AuthorizedHttpClient"] as HttpClient;

            var response = await client.GetAsync($"{_reviewApiUrl}/GetMediaMainPageReviewInfo/{mediaId}");

            if (!response.IsSuccessStatusCode) {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            var mediaMainPageReviewInfo = JsonConvert.DeserializeObject<MediaMainPageReviewInfo>(await response.Content.ReadAsStringAsync());
            return Ok(mediaMainPageReviewInfo);
        }

        [HttpGet("GetMediaReviews/{mediaId}")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))]
        public async Task<IActionResult> GetMediaReviews(string mediaId, int page = 1, int pageSize = 10, string sortOption = "mostrecent") {
            var client = HttpContext.Items["AuthorizedHttpClient"] as HttpClient;

            var response = await client.GetAsync($"{_reviewApiUrl}/GetMediaReviews/{mediaId}?page={page}&pageSize={pageSize}&sortOption={sortOption}");

            if (!response.IsSuccessStatusCode) {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            var reviews = JsonConvert.DeserializeObject<PageResult<ReviewWithUsername>>(await response.Content.ReadAsStringAsync());
            return Ok(reviews);
        }
    }
}
