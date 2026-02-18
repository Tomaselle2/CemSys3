using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.Parcela;
using CemSys3.DTOs.Seccion;
using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
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

        //listado de parcelas de una seccion
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
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al cargar las parcelas: {ex.Message}",
                    Tipo = "error"
                };
            }


            return View(viewModel);
        }

        //para cargar las parcelas en el select de la pantalla ingreso, se usan en vista parcial
        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> ObtenerParcelasPorSeccion(int seccionId, int estadoDifuntoId)
        {
            try
            {
                var parcelas = await _parcelaService.GetAllBySeccionId(seccionId, estadoDifuntoId);
                return Json(parcelas);
            }
            catch (Exception ex)
            {
                return Content($$"""
                <script>
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: '{{ex.Message.Replace("'", "\\'")}}'
                    });
                </script>
            """);
            }
        }


        //Vista de historial de parcela
        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> HistorialParcela(int parcelaId)
        {
            ParcelaHistorialVM viewModel = new ParcelaHistorialVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();

            try
            {
                viewModel.Historial = await _parcelaService.HistorialParcela(parcelaId);
            }
            catch (Exception ex) {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Ocurrió un error al cargar el historial: " + ex.Message,
                    Tipo = "error"
                };
            }

            return View(viewModel);
        }
    }
}
