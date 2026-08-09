using CemSys3.DTOs.PDF;
using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.Tarifaria;
using CemSys3.Enumerables;
using CemSys3.Helpers;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.PDF;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.PDF;
using CemSys3.Interfaces.Seccion;
using CemSys3.Interfaces.Tarifaria;
using CemSys3.ViewModels.Tarifaria;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class TarifariaController : Controller
    {
        private readonly ITarifaria _tarifariaService;
        private readonly IPrecioIngresoService _iprecioIngresoService;
        private readonly ISeccionNichoTarifaria _seccionTarifariaService;
        private readonly IViewRenderService _viewRenderService;
        private readonly PlaywrightPdfGenerator _pdfGenerator;

        public TarifariaController(ITarifaria tarifaria, ISeccionNichoTarifaria seccionTarifariaNicho, IPrecioIngresoService iprecioIngresoService, IViewRenderService render, PlaywrightPdfGenerator pdfGenerator)
        {
            _tarifariaService = tarifaria;
            _seccionTarifariaService = seccionTarifariaNicho;
            _iprecioIngresoService = iprecioIngresoService;
            _viewRenderService = render;
            _pdfGenerator = pdfGenerator;
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
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> ActualizarPreciosTarifaria([FromBody] List<PrecioActualizarDTO> precios)
        {
            try
            {
                // Validar que se recibieron datos
                if (precios == null || !precios.Any())
                {
                    return Json(new { success = false, message = "No se recibieron precios para actualizar." });
                }

                // Validar el modelo
                if (!ModelState.IsValid)
                {
                    var errores = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return Json(new { success = false, message = $"Datos inválidos: {string.Join(", ", errores)}" });
                }

                // Validaciones adicionales
                foreach (var precio in precios)
                {
                    // Id == 0 ahora es válido (significa INSERT de precio nuevo)
                    if (precio.Id < 0)
                    {
                        return Json(new { success = false, message = "ID de precio inválido." });
                    }

                    if (precio.ConceptoTarifariaId <= 0)
                    {
                        return Json(new { success = false, message = "ID de concepto tarifario inválido." });
                    }

                    if (precio.Precio < 0)
                    {
                        return Json(new { success = false, message = "El precio no puede ser negativo." });
                    }
                }

                // Actualizar/insertar los precios
                var nuevosIds = await _tarifariaService.ActualizarPreciosTarifaria(precios);

                return Json(new
                {
                    success = true,
                    message = "Precios actualizados correctamente.",
                    nuevosIds = nuevosIds
                });
            }
            catch (ArgumentException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "No se actualizó ningún precio." });
            }
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> GetAllPreciosIngreso()
        {
            var reglas = (await _iprecioIngresoService.ObtenerTodasLasReglasAsync()).ToList();
            var resumen = await _iprecioIngresoService.ObtenerResumenGeneralAsync();

            var reporte = new ReporteIngresoDTO { Reglas = reglas, Resumen = resumen };

            string html = await _viewRenderService.RenderToStringAsync("Tarifaria/GetAllPreciosIngreso", reporte);

            var pdfBytes = await _pdfGenerator.GenerateFromHtmlAsync(
                   html,
                   new PdfOptionsDto
                   {
                       Landscape = true,
                       MarginTop = "60px",
                       MarginLeft = "30px"
                   });
            //var pdfBytes = await _pdfGenerator.GenerateFromHtmlAsync(html); // por defecto en vertical
            return File(pdfBytes, "application/pdf", $"Precios_Ingresos_{DateTime.Now.Year}.pdf");
        }




        [HttpPost]
        [IgnoreAntiforgeryToken]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> AplicarAumentoPorcentual([FromBody] AumentoTarifariaDTO dto)
        {
            try
            {
                if (dto == null || dto.Porcentaje <= 0)
                    return Json(new { success = false, message = "Porcentaje inválido." });

                await _tarifariaService.AplicarAumentoPorcentual(dto.Porcentaje, dto.Decimales);

                return Json(new { success = true, message = $"Tarifaria actualizada con un aumento del {dto.Porcentaje}%." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> PreviewAumento(decimal porcentaje, int decimales)
        {
            if (porcentaje <= 0)
                return Json(new { success = false, message = "Porcentaje inválido." });

            const int FONDO_TEMA_ID = 7;
            decimal factor = 1 + (porcentaje / 100);

            var precios = await _tarifariaService.GetPrecios();
            var preview = precios
                .Where(p => p.TemaId != FONDO_TEMA_ID)
                .Select(p => new
                {
                    p.Id,
                    p.ConceptoTarifariaId,
                    p.NombreConcepto,
                    p.TemaId,
                    p.SeccionId,
                    p.NroFila,
                    p.AniosConcesionId,
                    PrecioActual = p.Precio,
                    PrecioNuevo = TarifariaHelper.Redondear(p.Precio * factor, decimales)
                });

            return Json(new { success = true, data = preview });
        }



        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> GetPdfPreciosNichosFosas()
        {
            var datos = await _tarifariaService.ObtenerDatosParaPdfNichosFosasAsync();

            string html = await _viewRenderService.RenderToStringAsync(
                "Tarifaria/GetPdfPreciosNichosFosas",
                datos);

            var pdfBytes = await _pdfGenerator.GenerateFromHtmlAsync(
                html,
                new PdfOptionsDto
                {
                    Landscape = false,
                    MarginTop = "50px",
                    MarginBottom = "30px",
                    MarginLeft = "100px",
                    MarginRight = "30px",
                });

            return File(
                pdfBytes,
                "application/pdf",
                $"Precios_Nichos_Fosas_{DateTime.Now.Year}.pdf");
        }

    }
}

