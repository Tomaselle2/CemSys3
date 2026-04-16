using CemSys3.DTOs.PlantillaTramite;
using CemSys3.Helpers.Mensajes;
using CemSys3.Interfaces.PlantillaTramite;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CemSys3.Controllers
{
    public class DocumentoController : Controller
    {
        private readonly IDocumentoTramiteService _service;

        public DocumentoController(IDocumentoTramiteService service)
        {
            _service = service;
        }

        [HttpPost]
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

                TempData.SetSweetAlert(new DTOs.SweetAlert.SweetAlertDTO
                {
                    Titulo = "Éxito",
                    Mensaje = "El documento se ha actualizado correctamente.",
                    Tipo = "success"
                });

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

            }
            catch (Exception ex)
            {

                TempData.SetSweetAlert(new DTOs.SweetAlert.SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al actualizar el documento: {ex.Message}",
                    Tipo = "error"
                });

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
            }

            return RedirectToAction("Index", "TramiteConcesion", new { tramiteId = tramiteId });
        }


    }
}
