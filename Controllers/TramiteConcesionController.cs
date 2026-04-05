using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Tramite;
using CemSys3.ViewModels.TramiteConcesion;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class TramiteConcesionController : Controller
    {
        private readonly ITramite _tramitesService;

        public TramiteConcesionController(ITramite tramitesService)
        {
            _tramitesService = tramitesService;
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

    }
}
