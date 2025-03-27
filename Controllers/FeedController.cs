using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using poplensFeedApi.Models;
using poplensWebUIGateway.Helper;
using poplensWebUIGateway.Models.Common;

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
    }
}
