using Microsoft.AspNetCore.Builder;

namespace Cardlytics.BasicApi.Middleware
{
    public static class Extensions
    {
        public static IApplicationBuilder UseRequestLogMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLogMiddleware>();
        }

        public static IApplicationBuilder UseExceptionCaptureMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionCaptureMiddleware>();
        }
    }
}
