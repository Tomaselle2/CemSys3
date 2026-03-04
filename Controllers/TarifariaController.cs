using CemSys3.DTOs.PDF;
using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.Tarifaria;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.PDF;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.PDF;
using CemSys3.Interfaces.Seccion;
using CemSys3.Interfaces.Tarifaria;
using CemSys3.ViewModels.Tarifaria;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CemSys3.Controllers
{
    public class TarifariaController : Controller
    {
        private readonly ITarifaria _tarifariaService;
        private readonly IPrecioIngresoService _iprecioIngresoService;
        private readonly ISeccionNichoTarifaria _seccionTarifariaService;
        private readonly IViewRenderService _viewRenderService;

        public TarifariaController(ITarifaria tarifaria, ISeccionNichoTarifaria seccionTarifariaNicho, IPrecioIngresoService iprecioIngresoService, IViewRenderService render)
        {
            _tarifariaService = tarifaria;
            _seccionTarifariaService = seccionTarifariaNicho;
            _iprecioIngresoService = iprecioIngresoService;
            _viewRenderService = render;
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> Index()
        {
            TarifariaVM viewModel = new TarifariaVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();
            try
            {
                viewModel.ListadoPrecios = await _tarifariaService.GetPrecios();
                viewModel.ListadoSeccionesNicho = await _seccionTarifariaService.GetAllSeccionesNichosParaTarifaria();
            }
            catch (Exception ex) {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Ocurrió un error al cargar los precios: " + ex.Message,
                    Tipo = "error"
                };
            }
                
            return View(viewModel);
        }

        // Método actualizado para AJAX
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ActualizarPreciosTarifaria([FromBody] List<PrecioActualizarDTO> precios)
        {
            try
            {
                // Validar que se recibieron datos
                if (precios == null || !precios.Any())
                {
                    return Json(new
                    {
                        success = false,
                        message = "No se recibieron precios para actualizar."
                    });
                }

                // Validar el modelo
                if (!ModelState.IsValid)
                {
                    var errores = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return Json(new
                    {
                        success = false,
                        message = $"Datos inválidos: {string.Join(", ", errores)}"
                    });
                }

                // Validaciones adicionales
                foreach (var precio in precios)
                {
                    if (precio.Id <= 0)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "ID de precio inválido."
                        });
                    }

                    if (precio.ConceptoTarifariaId <= 0)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "ID de concepto tarifario inválido."
                        });
                    }

                    if (precio.Precio < 0)
                    {
                        return Json(new
                        {
                            success = false,
                            message = "El precio no puede ser negativo."
                        });
                    }
                }

                // Actualizar los precios usando el business logic
                await _tarifariaService.ActualizarPreciosTarifaria(precios);

                return Json(new
                {
                    success = true,
                    message = "Precios actualizados correctamente."
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "No se actualizo ningun precio"
                });
            }

        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> GetAllPreciosIngreso()
        {
            IEnumerable<PrecioIngresoDTO> Listado = await _iprecioIngresoService.ObtenerTodasLasReglasAsync();

            string html = await _viewRenderService.RenderToStringAsync("Tarifaria/GetAllPreciosIngreso", Listado);

            var pdfGenerator = new PlaywrightPdfGenerator();
            var pdfBytes = await pdfGenerator.GenerateFromHtmlAsync(
                    html,
                    new PdfOptionsDto
                    {
                        Landscape = true,
                        MarginTop = "60px",
                        MarginLeft = "30px",
                    });
            //var pdfBytes = await _pdfGenerator.GenerateFromHtmlAsync(html); // por defecto en vertical
            return File(pdfBytes, "application/pdf", $"Precios_Ingresos_{DateTime.Now.Year}.pdf");
        }
    }
}

