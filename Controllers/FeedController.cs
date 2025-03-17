using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Newtonsoft.Json;
using poplensFeedApi.Models.Common;
using poplensUserProfileApi.Models;
using poplensWebUIGateway.Helper;
using System.Net.Http.Headers;

namespace poplensWebUIGateway.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class FeedController : ControllerBase {
        private readonly HttpClient _httpClient;
        private readonly string _feedApiUrl = "https://localhost:7067/api/Feed";

        public FeedController(HttpClient httpClient) {
            _httpClient = httpClient;
        }
        
        [HttpGet("GetFollowerFeed/{profileId}")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))]
        public async Task<ActionResult<PageResult<ReviewDetail>>> GetFollowerFeed(string profileId, int page = 1, int pageSize = 10) {
            try {
                var response = await _httpClient.GetAsync($"{_feedApiUrl}/GetFollowerFeed/{profileId}?page={page}&pageSize={pageSize}");

                if (!response.IsSuccessStatusCode) {
                    return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
                }

                var reviews = JsonConvert.DeserializeObject<PageResult<ReviewDetail>>(await response.Content.ReadAsStringAsync());
                return Ok(reviews);
            } catch (Exception ex) {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
