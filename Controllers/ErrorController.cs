using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult AccesoDenegado(string mensaje = null)
        {
            ViewBag.Mensaje = mensaje ?? "No tiene permisos para acceder a este recurso";
            return View();
        }

        [Route("Error")]
        public IActionResult Index()
        {
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            // Acá podés loguear
            // exceptionFeature?.Error
            // exceptionFeature?.Path

            return View();
        }

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            return View("StatusCode", statusCode);
        }
    }
}
