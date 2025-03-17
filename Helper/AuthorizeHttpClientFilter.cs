using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net.Http;
using System.Net.Http.Headers;

namespace poplensWebUIGateway.Helper {
    public class AuthorizeHttpClientFilter : IActionFilter {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthorizeHttpClientFilter(IHttpClientFactory httpClientFactory) {
            _httpClientFactory = httpClientFactory;
        }
        public void OnActionExecuting(ActionExecutingContext context) {
            var token = context.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token)) {
                context.Result = new UnauthorizedObjectResult(new { message = "Authorization token is missing" });
                return;
            }
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            context.HttpContext.Items["AuthorizedHttpClient"] = client;
        }

        public void OnActionExecuted(ActionExecutedContext context) {
            // No action needed after the method execution
        }
    }
}
