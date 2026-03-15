using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Concesion;
using CemSys3.ViewModels.Concesion;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class ConcesionController : Controller
    {
        public readonly IConcesion _concesionService;

        public ConcesionController(IConcesion concesion)
        {
            _concesionService = concesion;
        }

        //tabla general de concesiones, con paginacion y filtro por estado
        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> TablaGeneral(int filtroEstado = 0, int pagina = 1, int porPagina = 10)
        {
            TablaGeneralVM viewModel = new TablaGeneralVM();
            try
            {
                PaginadoResponse<TablaConcesionDTO> resultado = await _concesionService.GellAllPaginado(filtroEstado, pagina, porPagina);
                viewModel.Listado = resultado.Items;
                viewModel.Paginacion = resultado.Paginacion;

                viewModel.Paginacion.Parametros = new Dictionary<string, string>();

                viewModel.Paginacion.Parametros.Add("filtroEstado", filtroEstado.ToString());
                viewModel.Paginacion.Parametros.Add("porPagina", porPagina.ToString());
            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al cargar las concesiones: {ex.Message}",
                    Tipo = "error"
                };
            }

            viewModel.SweetAlert = TempData.GetSweetAlert();
            return View(viewModel);
        }

        //vista para hacer el contrato de conesion, por primera vez o para renovacion, dependiendo del estado del tramite
        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> GenerarContrato(int idTramite)
        {
            GenerarContratoVM viewModel = new GenerarContratoVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();

            try
            {
                viewModel.contrato = await _concesionService.SolicitarDatosParaGenerarContrato(idTramite);
                viewModel.CalcularPrecioNichoUnAnio();
            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al cargar los datos del contrato: {ex.Message}",
                    Tipo = "error"
                };
            }

            return View(viewModel);
        }
    }
}
