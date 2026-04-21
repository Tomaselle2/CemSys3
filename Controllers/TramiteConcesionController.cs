using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.Tarea;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Concesion;
using CemSys3.Interfaces.PlantillaTramite;
using CemSys3.Interfaces.Tarea;
using CemSys3.Interfaces.Tramite;
using CemSys3.Models;
using CemSys3.ViewModels.TramiteConcesion;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class TramiteConcesionController : Controller
    {
        private readonly IStrategyFactory _strategyFactory;
        //private readonly IServiceProvider _provider;
        private readonly ITarea _tareaService;
        private readonly ITramite _tramitesService;
        private readonly IConcesion _concesionService;

        public TramiteConcesionController(IStrategyFactory strategyFactory,
        //IServiceProvider provider,
        ITarea tareaService,
        ITramite tramitesService,
        IConcesion concesionService)
        {
            _strategyFactory = strategyFactory;
            //_provider = provider;
            _tareaService = tareaService;
            _tramitesService = tramitesService;
            _concesionService = concesionService;
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
                viewModel.InfoGeneral = await _concesionService.InfoGeneralMinima(viewModel.Dto.ConcesionId);
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
        public async Task<IActionResult> GuardarTareas(int tramiteId, List<TareaDTO> tareas, string returnUrl)
        {
            try
            {
                if (tareas != null && tareas.Any())
                {
                    await _tareaService.GuardarTareas(tramiteId, tareas);
                }

                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Éxito",
                    Mensaje = "Tareas guardadas correctamente",
                    Tipo = "success"
                });
            }
            catch (Exception ex)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Error al guardar tareas: {ex.Message}",
                    Tipo = "error"
                });
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("IrATramite", "Tramite", new { tramiteId = tramiteId });
        }


        // =========================
        // FINALIZAR
        // =========================
        [HttpPost]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> Finalizar(int tipoTramiteId, int tramiteId, string returnUrl, List<TareaDTO> tareas)
        {
            var strategy = _strategyFactory.GetStrategy(tipoTramiteId);

            int usuarioId = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

            try
            {
                await _tareaService.GuardarTareas(tramiteId, tareas);
                await strategy.FinalizarAsync(tramiteId, usuarioId);

                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Trámite finalizado",
                    Mensaje = "El trámite ha sido finalizado correctamente.",
                    Tipo = "success"
                });
            }
            catch(Exception ex)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al finalizar el trámite: {ex.Message}",
                    Tipo = "error"
                });
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("IrATramite", "Tramite", new { tramiteId = tramiteId });
        }

        // =========================
        // AVANZAR ESTADO
        // =========================
        public async Task<IActionResult> AvanzarEstado(
        int tipoTramiteId,
        int tramiteId,
        int nuevoEstado)
        {
            int usuarioId = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

            var strategy = _strategyFactory.GetStrategy(tipoTramiteId);

            var estado = await strategy.AvanzarEstadoAsync(tramiteId, nuevoEstado, usuarioId);

            return Ok(new { estado });
        }


    }
}
