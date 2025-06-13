using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using poplensFeedApi.Models;
using poplensWebUIGateway.Helper;
using poplensWebUIGateway.Models.Common;
using poplensWebUIGateway.Models.Media;

namespace poplensWebUIGateway.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class FeedController : ControllerBase {
        private readonly string _feedApiUrl = "http://poplensFeedApi:8080/api/Feed";

        public FeedController() { }

        [HttpGet("GetFollowerFeed/{profileId}")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))] // Apply the filter to inject HttpClient
        public async Task<ActionResult<PageResult<ReviewProfileDetail>>> GetFollowerFeed(string profileId, int page = 1, int pageSize = 10) {
            try {
                // Get the authorized HttpClient from the HttpContext.Items set by the filter
                var client = HttpContext.Items["AuthorizedHttpClient"] as HttpClient;

                // Make the request to the Feed API using the HttpClient
                var response = await client.GetAsync($"{_feedApiUrl}/GetFollowerFeed/{profileId}?page={page}&pageSize={pageSize}");

                // Check if the response is successful, else return the status code and error
                if (!response.IsSuccessStatusCode) {
                    return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
                }

                // Deserialize the response to PageResult
                var reviews = JsonConvert.DeserializeObject<PageResult<ReviewProfileDetail>>(await response.Content.ReadAsStringAsync());

                // Return the reviews
                return Ok(reviews);
            } catch (Exception ex) {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetForYouFeed/{profileId}")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))] // Apply the filter to inject HttpClient
        public async Task<ActionResult<PageResult<ReviewProfileDetail>>> GetForYouFeed(string profileId, int pageSize = 10) {
            try {
                // Get the authorized HttpClient from the HttpContext.Items set by the filter
                var client = HttpContext.Items["AuthorizedHttpClient"] as HttpClient;

                // Make the request to the Feed API using the HttpClient
                var response = await client.GetAsync($"{_feedApiUrl}/GetForYouFeed/{profileId}?pageSize={pageSize}");

                // Check if the response is successful, else return the status code and error
                if (!response.IsSuccessStatusCode) {
                    return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
                }

                // Deserialize the response to PageResult
                var recommendations = JsonConvert.DeserializeObject<PageResult<ReviewProfileDetail>>(await response.Content.ReadAsStringAsync());

                // Return the recommendations
                return Ok(recommendations);
            } catch (Exception ex) {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetMediaRecommendations/{profileId}")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))]
        public async Task<ActionResult<List<Media>>> GetMediaRecommendations(
                string profileId,
                int pageSize = 3,
                string? mediaType = null) {
            try {
                var client = HttpContext.Items["AuthorizedHttpClient"] as HttpClient;

                // Build query string
                var url = $"{_feedApiUrl}/GetMediaRecommendations/{profileId}?pageSize={pageSize}";
                if (!string.IsNullOrEmpty(mediaType))
                    url += $"&mediaType={Uri.EscapeDataString(mediaType)}";

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode) {
                    return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
                }

                var recommendations = JsonConvert.DeserializeObject<List<Media>>(await response.Content.ReadAsStringAsync());
                return Ok(recommendations);
            } catch (Exception ex) {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
