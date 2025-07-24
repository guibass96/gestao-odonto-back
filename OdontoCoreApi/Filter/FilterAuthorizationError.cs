using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GBarber.WebApi.Filter
{
    public class FilterAuthorizationError : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.HttpContext.Response.StatusCode == 403)
            {
                context.Result = new ContentResult
                {
                    Content = "Você não tem permissão para acessar este recurso.",
                    StatusCode = 403
                };
                context.ExceptionHandled = true;
            }
            if (context.HttpContext.Response.StatusCode == 401)
            {
                context.Result = new ContentResult
                {
                    Content = "Você não tem permissão para acessar este recurso.",
                    StatusCode = 403
                };
                context.ExceptionHandled = true;
            }
            if (context.HttpContext.Response.StatusCode == 400)
            {
                context.Result = new ContentResult
                {
                    Content = "Senha ou Email incorreto",
                    StatusCode = 403
                };
                context.ExceptionHandled = true;
            }
        }
    }
}
