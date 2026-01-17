using CemSys3.Enumerables;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CemSys3.Controllers
{
    public class HomeController : Controller
    {
        [AuthorizeRole(RolUsuario.Administrador, RolUsuario.Empleado)]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
