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

        [HttpGet("{reviewId}/GetReviewById")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))]
        public async Task<IActionResult> GetReviewById(Guid reviewId) {
            var client = HttpContext.Items["AuthorizedHttpClient"] as HttpClient;

            var response = await client.GetAsync($"{_reviewApiUrl}/{reviewId}/GetReviewById");

            if (!response.IsSuccessStatusCode) {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound) {
                    return NotFound();
                }
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            var review = JsonConvert.DeserializeObject<Review>(await response.Content.ReadAsStringAsync());
            return Ok(review);
        }

        [HttpGet("{reviewId}/GetReviewDetail")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))]
        public async Task<IActionResult> GetReviewDetail(Guid reviewId) {
            var client = HttpContext.Items["AuthorizedHttpClient"] as HttpClient;

            var response = await client.GetAsync($"{_reviewApiUrl}/{reviewId}/GetReviewDetail");

            if (!response.IsSuccessStatusCode) {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound) {
                    return NotFound();
                }
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            var reviewDetail = JsonConvert.DeserializeObject<ReviewDetail>(await response.Content.ReadAsStringAsync());
            return Ok(reviewDetail);
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

        // ────────────────────── Likes ──────────────────────

        [HttpPost("{profileId}/{reviewId}/Like")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))]
        public async Task<IActionResult> AddLike(Guid profileId, Guid reviewId) {
            var client = HttpContext.Items["AuthorizedHttpClient"] as HttpClient;

            var response = await client.PostAsync($"{_reviewApiUrl}/{profileId}/{reviewId}/Like", null);

            if (!response.IsSuccessStatusCode) {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            return NoContent();
        }

        [HttpDelete("{profileId}/{reviewId}/Unlike")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))]
        public async Task<IActionResult> RemoveLike(Guid profileId, Guid reviewId) {
            var client = HttpContext.Items["AuthorizedHttpClient"] as HttpClient;

            var response = await client.DeleteAsync($"{_reviewApiUrl}/{profileId}/{reviewId}/Unlike");

            if (!response.IsSuccessStatusCode) {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            return NoContent();
        }

        [HttpGet("{reviewId}/LikeCount")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))]
        public async Task<IActionResult> GetLikeCount(Guid reviewId) {
            var client = HttpContext.Items["AuthorizedHttpClient"] as HttpClient;

            var response = await client.GetAsync($"{_reviewApiUrl}/{reviewId}/LikeCount");

            if (!response.IsSuccessStatusCode) {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            var count = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());
            return Ok(count);
        }

        [HttpGet("{profileId}/{reviewId}/HasLiked")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))]
        public async Task<IActionResult> HasUserLiked(Guid profileId, Guid reviewId) {
            var client = HttpContext.Items["AuthorizedHttpClient"] as HttpClient;

            var response = await client.GetAsync($"{_reviewApiUrl}/{profileId}/{reviewId}/HasLiked");

            if (!response.IsSuccessStatusCode) {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            var hasLiked = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
            return Ok(hasLiked);
        }

        // ────────────────────── Comments ──────────────────────

        [HttpPost("{profileId}/{reviewId}/Comment")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))]
        public async Task<IActionResult> AddComment(Guid profileId, Guid reviewId, [FromBody] CreateCommentRequest request) {
            var client = HttpContext.Items["AuthorizedHttpClient"] as HttpClient;

            var jsonContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_reviewApiUrl}/{profileId}/{reviewId}/Comment", jsonContent);

            if (!response.IsSuccessStatusCode) {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            return NoContent();
        }

        [HttpPut("{commentId}/EditComment")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))]
        public async Task<IActionResult> UpdateComment(Guid commentId, [FromBody] string newContent) {
            var client = HttpContext.Items["AuthorizedHttpClient"] as HttpClient;

            var jsonContent = new StringContent(JsonConvert.SerializeObject(newContent), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"{_reviewApiUrl}/{commentId}/EditComment", jsonContent);

            if (!response.IsSuccessStatusCode) {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            return NoContent();
        }

        [HttpDelete("{commentId}/DeleteComment")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))]
        public async Task<IActionResult> DeleteComment(Guid commentId) {
            var client = HttpContext.Items["AuthorizedHttpClient"] as HttpClient;

            var response = await client.DeleteAsync($"{_reviewApiUrl}/{commentId}/DeleteComment");

            if (!response.IsSuccessStatusCode) {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            return NoContent();
        }

        [HttpGet("{reviewId}/TopLevelComments")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))]
        public async Task<IActionResult> GetTopLevelComments(Guid reviewId) {
            var client = HttpContext.Items["AuthorizedHttpClient"] as HttpClient;

            var response = await client.GetAsync($"{_reviewApiUrl}/{reviewId}/TopLevelComments");

            if (!response.IsSuccessStatusCode) {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            var comments = JsonConvert.DeserializeObject<List<Comment>>(await response.Content.ReadAsStringAsync());
            return Ok(comments);
        }

        [HttpGet("{parentCommentId}/Replies")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))]
        public async Task<IActionResult> GetReplies(Guid parentCommentId) {
            var client = HttpContext.Items["AuthorizedHttpClient"] as HttpClient;

            var response = await client.GetAsync($"{_reviewApiUrl}/{parentCommentId}/Replies");

            if (!response.IsSuccessStatusCode) {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            var replies = JsonConvert.DeserializeObject<List<Comment>>(await response.Content.ReadAsStringAsync());
            return Ok(replies);
        }

        [HttpGet("{reviewId}/CommentCount")]
        [ServiceFilter(typeof(AuthorizeHttpClientFilter))]
        public async Task<IActionResult> GetCommentCount(Guid reviewId) {
            var client = HttpContext.Items["AuthorizedHttpClient"] as HttpClient;

            var response = await client.GetAsync($"{_reviewApiUrl}/{reviewId}/CommentCount");

            if (!response.IsSuccessStatusCode) {
                return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
            }

            var count = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync());
            return Ok(count);
        }

    }
}
