using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace Tasko.Server
{
    public class TaskoExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            context.Response = context.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, context.Exception.Message);
        }
    }
}