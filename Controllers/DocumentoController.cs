using CemSys3.DTOs.PlantillaTramite;
using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.PlantillaTramite;
using CemSys3.Models;
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


    }
}
