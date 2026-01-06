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
    }
}
