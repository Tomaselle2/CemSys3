using CemSys3.Business.Tarea;
using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.Tarea;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Tarea;
using CemSys3.Interfaces.Tramite;
using CemSys3.ViewModels.TramiteConcesion;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class TramiteConcesionController : Controller
    {
        private readonly ITramite _tramitesService;
        private readonly ITarea _tareaService;

        public TramiteConcesionController(ITramite tramitesService, ITarea tareaService)
        {
            _tramitesService = tramitesService;
            _tareaService = tareaService;
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> Index(int tramiteId) //recibe el id del tramite de concesion.
        {
            ListadoTramitesVM viewModel = new ListadoTramitesVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();

            try
            {
                viewModel.Dto = await _tramitesService.GetListadoTramitesDeConcesion(tramiteId);
            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al generar el trámite: {ex.Message}",
                    Tipo = "error"
                };
            }

            return View(viewModel);
        }

        [HttpPost]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> GuardarTareas(int tramiteId, List<TareaDTO> tareas)
        {
            try
            {
                if (tareas != null && tareas.Any())
                {
                    await _tareaService.GuardarTareas(tramiteId, tareas);
                }

                return Json(new { success = true, message = "Tareas guardadas correctamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al guardar tareas" + ex.Message });
            }
        }

    }
}
