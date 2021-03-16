

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace FirstCustomMiddleware
{
    public class CustomLogger
    {
        private RequestDelegate _next;

        public CustomLogger(RequestDelegate next)
        {
            _next = next;
        }


        public Task Invoke(HttpContext context)
        {
            //Some Logic Here
            var title = context.Request.Query["title"];
            Console.WriteLine(title);

            return _next(context);
        }
    }

    public static class CustomLoggerExtentions
    {
        public static IApplicationBuilder UseCustomLogger(this IApplicationBuilder applicationBuilder)
        {
            return applicationBuilder.UseMiddleware<CustomLogger>();
        }
    }
}
