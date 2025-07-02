using Microsoft.AspNetCore.Http;

namespace SharedLibrary
{
    public class RestrictAccessMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            var referrer = context.Request.Headers["Referrer"];
            var refKey = context.Request.Headers["Referrer-Key"];

            if (string.IsNullOrEmpty(referrer) || !string.Equals(refKey, "salty-key"))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Acesso negado!");
                return;
            }
            await next(context);
        }
    }
}
