using CemSys3.DTOs.Calendario;
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

        [HttpPost]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> CrearEvento(CalendarDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.title))
            {
                return BadRequest("Debe ingresar un título.");
            }

            if (dto.start == DateTime.MinValue)
            {
                return BadRequest("Debe ingresar una fecha.");
            }

            await _calendarioService.Add(dto);

            return Ok();
        }

        [HttpPost]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> ModificarEvento(CalendarDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.title))
            {
                return BadRequest("Debe ingresar un título.");
            }

            if (dto.start == DateTime.MinValue)
            {
                return BadRequest("Debe ingresar una fecha.");
            }

            await _calendarioService.Update(dto);

            return Ok();
        }

        [HttpPost]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> EliminarEvento(int id)
        {
            await _calendarioService.Delete(id);
            return Ok();
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> ModalEvento(int id)
        {
            CalendarDTO dto;

            if (id == 0)
            {
                dto = new CalendarDTO
                {
                    start = DateTime.Now
                };
            }
            else
            {
                dto = await _calendarioService.Get(id);
            }

            return PartialView("_EventoModal", dto);
        }
    }
}
