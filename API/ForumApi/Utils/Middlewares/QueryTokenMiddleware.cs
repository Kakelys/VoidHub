namespace ForumApi.Utils.Middlewares
{
    public class QueryTokenMiddleware(
        RequestDelegate next,
        ILogger<QueryTokenMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Query["access_token"];
            if(!string.IsNullOrEmpty(token))
            {
                context.Request.Headers.Authorization = $"Bearer {token}";
            }

            await next(context);
        }
    }
}