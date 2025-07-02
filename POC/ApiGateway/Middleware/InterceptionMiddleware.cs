namespace ApiGateway.Middleware
{
    public class InterceptionMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.Headers["Referrer"] = "Api-Gateway";
            context.Request.Headers["Referrer-Key"] = "salty-key";
            await next(context);
        }
    }
}
