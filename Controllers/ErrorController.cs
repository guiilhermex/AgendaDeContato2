using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AgendaContato.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult HandleError()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

            return Problem(
                title: "Ocorreu um erro inesperado no servidor",
                detail: exception?.Message,
                statusCode: 500
            );
        }
    }
}
