using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Models.RequestDto;

namespace Middleware_poc.Controllers
{
    public class MyController : Controller
    {
        [HttpPost("Execute")]
        public IActionResult Execute([FromBody]RequestDto request)
        {
            throw request.StatusType switch
            {
                Models.Entity.StatusType.NotFound => new DllNotFoundException("Not Found"),
                Models.Entity.StatusType.InvalidOperation => new InvalidOperationException("Invalid Operation"),
                Models.Entity.StatusType.HttpRequestException => new HttpRequestException("Http Request Exception"),
                Models.Entity.StatusType.ArithmeticException => new ArithmeticException("Arithmetic Exception"),
                Models.Entity.StatusType.NotSupported => new InvalidOperationException("Not Supported! "),
                _ => new Exception("InternalServer Error"),
            };
        }
    }
}
