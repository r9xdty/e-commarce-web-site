using System.Net;
using Microsoft.AspNetCore.Http;

namespace CategoriesWithProducts.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly ILogger<ExceptionHandlerMiddleware> loggler;
        private readonly RequestDelegate next;

        public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> loggler, RequestDelegate next)
        {
            this.loggler = loggler;
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid();
                //Log this exception
                loggler.LogError(ex, $"{errorId} : {ex.Message}");

                //Return a custom error response
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";

                var error = new
                {
                    Id = errorId,
                    ErrorMessage = "Something went wrong!"
                };

                await httpContext.Response.WriteAsJsonAsync(error);
            }
        }
    }
}
