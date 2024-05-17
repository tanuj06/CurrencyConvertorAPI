namespace CurrencyConverterAPI.Middleware
{
    /// <summary>
    /// All Incoming request will go through this service.
    /// This will validate the token is present or not and it should be an authorization bearer token
    /// </summary>
    public class MiddlewareService : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync("Access forbidden. Wrong header");
                return;
            }
            await next(context);
        }
    }
}
