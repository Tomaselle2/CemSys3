using CemSys3.Enumerables;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Calendario;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class CalendarioController : Controller
    {
        private readonly ICalendario _calendarioService;

        public CalendarioController(ICalendario calendarioService)
        {
            _calendarioService = calendarioService;
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> GetEvents()
        {
            var eventos = await _calendarioService.GetEventsAsync();
            return Json(eventos);
        }
    }
}
