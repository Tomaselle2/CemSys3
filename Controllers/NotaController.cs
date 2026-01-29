using CemSys3.DTOs.Nota;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Notas;
using CemSys3.ViewModels.Nota;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CemSys3.Controllers
{
    public class NotaController : Controller
    {
        private readonly INotas _notaService;

        public NotaController(INotas notasService)
        {
            _notaService = notasService;
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> Index(int estadoId = (int)EstadosNotaEnum.NotaPendiente, int filtro = 0, int pagina = 1, int porPagina = 10)
        {
            NotaVM viewModel = new NotaVM();

            if (porPagina <= 0)
                porPagina = 10;

            try
            {
                PaginadoResponse<NotaDTO> paginadoNotas = await _notaService.GetPaginadoByTipo(estadoId, filtro, pagina, porPagina);
                viewModel.ListaNotas = paginadoNotas.Items;
                viewModel.Paginacion = paginadoNotas.Paginacion;

                // Inicializar parámetros si es null
                viewModel.Paginacion.Parametros ??= new Dictionary<string, string>();

                // Actualizar solo con los valores actuales
                viewModel.Paginacion.Parametros["filtro"] = filtro.ToString();
                viewModel.Paginacion.Parametros["estadoId"] = estadoId.ToString();
                viewModel.Paginacion.Parametros["porPagina"] = porPagina.ToString();

                // Mantener otros parámetros si los hubiera
                viewModel.Paginacion.Parametros["pagina"] = pagina.ToString();
            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Ocurrió un error al cargar las notas: " + ex.Message,
                    Tipo = "error"
                };
            }
            return View(viewModel);
        }
    }
}
