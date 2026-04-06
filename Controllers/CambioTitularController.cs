using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Archivo;
using CemSys3.Interfaces.HistorialEstados;
using CemSys3.Interfaces.PlantillaTramite;
using CemSys3.Interfaces.TramiteConcesion;
using CemSys3.Models;
using CemSys3.ViewModels.TramiteConcesion;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class CambioTitularController : Controller
    {
        private readonly IPlantillaTramite _planillasService;
        private readonly ICambioTitular _cambioTitular;
        private readonly IArchivo _archivoService;
        private readonly IHistorialEstados _historialService;

        public CambioTitularController(IPlantillaTramite planillasService, ICambioTitular cambioTitular, IArchivo archivoService, IHistorialEstados historialService)
        {
            _planillasService = planillasService;
            _cambioTitular = cambioTitular;
            _archivoService = archivoService;
            _historialService = historialService;
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> CambioTitular(
            int? cambioTitularId,
            int? concesionId)
        {
            CambioTitularVM viewModel = new CambioTitularVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();

            try
            {
                viewModel.PlantillaTramite = await _planillasService.Get((int)PlantillasTramitesEnum.CambioTipo1);

                int usuarioId = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

                if (cambioTitularId.HasValue && concesionId.HasValue)
                {
                    //CONTINUAR trámite existente
                    viewModel.Dto = await _cambioTitular.Get(cambioTitularId.Value, concesionId.Value);
                    viewModel.Archivos = await _archivoService.GetAllByTramiteId(cambioTitularId.Value);
                    viewModel.Historial = await _historialService.GetAllById(cambioTitularId.Value);
                }
                else if (concesionId.HasValue)
                {
                    //INICIAR nuevo trámite
                    viewModel.Dto = await _cambioTitular.AddCambioTitular(concesionId.Value, usuarioId);
                }
                else
                {
                    throw new Exception("Parámetros inválidos.");
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = ex.Message,
                    Tipo = "error"
                });
                return RedirectToAction("Index", "TramiteConcesion", new { tramiteId = concesionId });

            }

        }
    }
}
