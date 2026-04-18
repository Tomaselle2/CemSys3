using CemSys3.DTOs.PDF;
using CemSys3.DTOs.PlantillaTramite;
using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.PDF;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.PDF;
using CemSys3.Interfaces.PlantillaTramite;
using CemSys3.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.RegularExpressions;

namespace CemSys3.Controllers
{
    public class DocumentoController : Controller
    {
        private readonly IDocumentoTramiteService _service;
        private readonly PlaywrightPdfGenerator _pdfGenerator;
        private readonly IViewRenderService _viewRenderService;


        public DocumentoController(IDocumentoTramiteService service, PlaywrightPdfGenerator pdfGenerator, IViewRenderService viewRenderService)
        {
            _service = service;
            _pdfGenerator = pdfGenerator;
            _viewRenderService = viewRenderService;
        }

        [HttpPost]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> Editar(int id, string contenidoHtml, int tramiteId, string returnUrl)
        {
            contenidoHtml = WebUtility.HtmlDecode(contenidoHtml);

            DocumentoDTO dto = new DocumentoDTO
            {
                Id = id,
                ContenidoHtml = contenidoHtml,
                TramiteId = tramiteId
            };

            try
            {
                await _service.ActualizarAsync(dto);

                return RedirigirConMensaje(returnUrl, tramiteId,
                    "Éxito", "El documento se ha actualizado correctamente.", "success");
            }
            catch (Exception ex)
            {
                return RedirigirConMensaje(returnUrl, tramiteId,
                    "Error", $"Ocurrió un error al actualizar el documento: {ex.Message}", "error");
            }
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> Eliminar(int documentoId, int tramiteId, string returnUrl)
        {
            try
            {
                await _service.Delete(documentoId);

                return RedirigirConMensaje(returnUrl, tramiteId,
                    "Éxito", "Autorización eliminada correctamente", "success");
            }
            catch (Exception ex)
            {
                return RedirigirConMensaje(returnUrl, tramiteId,
                    "Error", $"Error al eliminar el documento: {ex.Message}", "error");
            }
        }


        private IActionResult RedirigirConMensaje(string returnUrl, int tramiteId, string titulo, string mensaje, string tipo)
        {
            TempData.SetSweetAlert(new SweetAlertDTO
            {
                Titulo = titulo,
                Mensaje = mensaje,
                Tipo = tipo
            });

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "TramiteConcesion", new { tramiteId = tramiteId });
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> GenerarPDF(int id)  // Cambiar a recibir ID en lugar de HTML
        {
            // Obtener el contenido del documento por ID
            var documento = await _service.ObtenerDocumentoPorId(id);
            if (documento == null)
                return NotFound();

            // Reemplazar las rutas de imágenes con la URL completa
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var contenidoConImagenes = ReemplazarRutasImagenes(documento.ContenidoHtml, baseUrl);

            // Renderizar la plantilla PDF con el contenido modificado
            string html = await _viewRenderService.RenderToStringAsync(
                "TramiteConcesion/PlantillaPDF",
                contenidoConImagenes);

            var pdfBytes = await _pdfGenerator.GenerateFromHtmlAsync(
                html,
                new PdfOptionsDto
                {
                    Landscape = false,
                    MarginTop = "5px",
                    MarginLeft = "60px",
                    MarginRight = "30px"
                });

            return File(pdfBytes, "application/pdf");
        }

        // Método auxiliar para reemplazar rutas de imágenes
        private string ReemplazarRutasImagenes(string html, string baseUrl)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            // Limpiar query strings
            html = Regex.Replace(html, @"\?v=\d+", "");

            // Reemplazar cualquier src que no sea URL absoluta
            html = Regex.Replace(html,
                @"src=""(?!https?:\/\/)([^""]+)""",
                match =>
                {
                    var ruta = match.Groups[1].Value;

                    // Limpiar la ruta
                    ruta = ruta.TrimStart('/', '.');
                    ruta = ruta.Replace("../", "").Replace("./", "");

                    // Si la ruta empieza con "fotos/", mantenerla, si no, agregar "fotos/"
                    if (!ruta.StartsWith("fotos/"))
                    {
                        // Intentar extraer solo el nombre del archivo
                        var nombreArchivo = Path.GetFileName(ruta);
                        ruta = $"fotos/{nombreArchivo}";
                    }

                    return $"src=\"{baseUrl}/{ruta}\"";
                });

            // Mismo proceso para comillas simples
            html = Regex.Replace(html,
                @"src='(?!https?:\/\/)([^']+)'",
                match =>
                {
                    var ruta = match.Groups[1].Value;
                    ruta = ruta.TrimStart('/', '.');
                    ruta = ruta.Replace("../", "").Replace("./", "");

                    if (!ruta.StartsWith("fotos/"))
                    {
                        var nombreArchivo = Path.GetFileName(ruta);
                        ruta = $"fotos/{nombreArchivo}";
                    }

                    return $"src='{baseUrl}/{ruta}'";
                });

            return html;
        }


    }
}
