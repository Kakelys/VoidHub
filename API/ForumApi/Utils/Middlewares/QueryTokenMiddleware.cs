namespace ForumApi.Utils.Middlewares
{
    public class QueryTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<QueryTokenMiddleware> _logger;

        public QueryTokenMiddleware(
            RequestDelegate next,
            ILogger<QueryTokenMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Query["access_token"];
            if(!string.IsNullOrEmpty(token))
            {
                context.Request.Headers["Authorization"] = $"Bearer {token}";
            }

            await _next(context);
        }
    }
}