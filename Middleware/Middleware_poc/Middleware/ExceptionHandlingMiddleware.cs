using Newtonsoft.Json;
using System.Net;

namespace Middleware_poc.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private static async Task HandleException(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            object error;
            switch (exception)
            {
                case HttpRequestException ex:
                    error = new
                    {
                        context.Response.StatusCode,
                        exception.Message,
                        Type = typeof(Exception).Name
                    };
                    break;

                case InvalidOperationException ex:
                    error = new
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        exception.Message,
                        Type = typeof(Exception).Name
                    };
                    break;

                case ArgumentNullException ex:
                    error = new
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        exception.Message,
                        Type = typeof(Exception).Name
                    };
                    break;

                default:
                    error = new
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError,
                        exception.Message,
                        Type = typeof(Exception).Name
                    };
                    break;
            }
            var result = JsonConvert.SerializeObject(error);
            await context.Response.WriteAsync(result);

        }
    }
}
