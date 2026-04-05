using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.PlantillaTramite;
using CemSys3.Interfaces.TramiteConcesion;
using CemSys3.ViewModels.TramiteConcesion;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class CambioTitularController : Controller
    {
        private readonly IPlantillaTramite _planillasService;
        private readonly ICambioTitular _cambioTitular;
        public CambioTitularController(IPlantillaTramite planillasService, ICambioTitular cambioTitular)
        {
            _planillasService = planillasService;
            _cambioTitular = cambioTitular;
        }

        //tramite de cambio de titular ambos titulares presentes. Se genera el tramite
        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> CambioTitular(int tramiteConcesionId)
        {
            CambioTitularVM viewModel = new CambioTitularVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();

            try
            {
                viewModel.PlantillaTramite = await _planillasService.Get((int)PlantillasTramitesEnum.CambioTipo1);
                int usuarioId = HttpContext.Session.GetInt32("IdUsuario") ?? 0;
                viewModel.Dto = await _cambioTitular.AddCambioTitular(tramiteConcesionId, usuarioId);
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
