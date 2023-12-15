using Blogs.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Authentication;

namespace Blogs.WebApi.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            try
            {
                throw context.Exception;
            }
            catch (InvalidRequestDataException e)
            {
                context.Result = GetJsonResult(e.Message, 400);
            }
            catch (InvalidCredentialException e)
            {
                context.Result = GetJsonResult(e.Message, 401);
            }
            catch (ResourceNotFoundException e)
            {
                context.Result = GetJsonResult(e.Message, 404);
            }
            catch (InvalidOperationException e)
            {
                context.Result = GetJsonResult(e.Message, 409);
            }
            catch (Exception)
            {
                context.Result = GetJsonResult("Issues encountered, try again later", 500);
            }
        }

        #region Helpers

        private static JsonResult GetJsonResult(string message, int? statusCode)
        {
            return new JsonResult(message) {StatusCode = statusCode};
        }

        #endregion 
    }
}
