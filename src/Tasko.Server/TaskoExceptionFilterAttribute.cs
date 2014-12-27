using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using Tasko.Server.Properties;

namespace Tasko.Server
{
    public class TaskoExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            HttpStatusCode status;
            string message;

            if (context.Exception is TaskoInvalidInputException)
            {
                status = HttpStatusCode.BadRequest;
                message = context.Exception.Message;
            }
            else
            {
                status = HttpStatusCode.InternalServerError;
                message = Resources.GenericErrorMessage;
            }

            context.Response = context.Request.CreateErrorResponse(status, message);
        }
    }
}