using CemSys3.Enumerables;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Notificaciones;
using CemSys3.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CemSys3.Controllers
{
    public class HomeController : Controller
    {
        private readonly INotificaciones _notificacionesService;

        public HomeController(INotificaciones notificaciones)
        {
            _notificacionesService = notificaciones;
        }

        [AuthorizeRole(RolUsuario.Administrador, RolUsuario.Empleado)]
        public IActionResult Index()
        {
            
            return View();
        }
    }
}
