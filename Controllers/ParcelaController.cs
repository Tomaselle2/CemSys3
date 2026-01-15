using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.Parcela;
using CemSys3.DTOs.Seccion;
using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Parcela;
using CemSys3.Interfaces.Seccion;
using CemSys3.ViewModels.Parcela;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CemSys3.Controllers
{
    public class ParcelaController : Controller
    {
        private readonly IParcela _parcelaService;
        private readonly ISeccion _seccionService;

        public ParcelaController(IParcela parcelaService, ISeccion seccionService)
        {
            _parcelaService = parcelaService;
            _seccionService = seccionService;
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> Index(int seccionId, int filtro = 0, int pagina = 1, int porPagina = 10)
        {
            ParcelaIndexVM viewModel = new ParcelaIndexVM();

            try
            {
                SeccionRequestDTO seccion = await _seccionService.Get(seccionId);
                PaginadoResponse<ParcelaIndexRequestDTO> resultado = await _parcelaService.GetAllPaginadoBySeccion(seccionId, filtro, pagina, porPagina);
                viewModel.NombreSeccion = seccion.Nombre.ToUpper();
                viewModel.TipoParcelaId = seccion.TipoParcelaId;

                viewModel.ListadoParcelas = resultado.Items;
                viewModel.Paginacion = resultado.Paginacion;

                viewModel.Paginacion.Parametros = new Dictionary<string, string>();

                viewModel.Paginacion.Parametros.Add("filtro", filtro.ToString());
                viewModel.Paginacion.Parametros.Add("seccionId", seccionId.ToString());
                viewModel.Paginacion.Parametros.Add("porPagina", porPagina.ToString());
            }
            catch (Exception ex) {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al cargar las parcelas: {ex.Message}",
                    Tipo = "error"
                };
            }
            

            return View(viewModel);
        }
    }
}
