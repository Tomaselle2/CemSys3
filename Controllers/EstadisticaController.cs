using CemSys3.Business.Estadistica;
using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Estadistica;
using CemSys3.ViewModels.Estadistica;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class EstadisticaController : Controller
    {
        private readonly IEstadistica _estadisticaService;

        public EstadisticaController(IEstadistica estadisticaService)
        {
            _estadisticaService = estadisticaService;
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> Index()
        {
            EstadisticasVM viewModel = new EstadisticasVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();

            try
            {
                viewModel.Estadisticas = await _estadisticaService.GetEstadisticasGenerales();
            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al cargar las estadísticas: {ex.Message}",
                    Tipo = "error"
                };
            }

            return View(viewModel);
        }
    }
}
