using CemSys3.DTOs.Archivo;
using CemSys3.DTOs.Generics;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Archivo;
using CemSys3.Models;
using CemSys3.ViewModels.Archivo;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class ArchivoController : Controller
    {
        private readonly IArchivo _archivoService;

        public ArchivoController(IArchivo archivoService)
        {
            _archivoService = archivoService;
        }

        [HttpPost]
        [AuthorizeRole(RolUsuario.Empleado, RolUsuario.Administrador)]
        public async Task<IActionResult> Subir(ArchivoVM viewModel)
        {
            if (string.IsNullOrWhiteSpace(viewModel.Descripcion) || string.IsNullOrWhiteSpace(viewModel.CategoriaArchivo))
            {
                TempData.SetSweetAlert(new DTOs.SweetAlert.SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "La descripcion del archivo es obligatoria",
                    Tipo = "error"
                });

                return RedirectToAction("IrATramite", "Tramite", new { tramiteId = viewModel.TramiteId });
            }

            if (viewModel.Archivo == null || viewModel.Archivo.Length == 0)
            {
                TempData.SetSweetAlert(new DTOs.SweetAlert.SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Debe seleccionar un archivo válido",
                    Tipo = "error"
                });

                return RedirectToAction("IrATramite", "Tramite", new { tramiteId = viewModel.TramiteId });
            }

            // Validar extensión
            var extension = Path.GetExtension(viewModel.Archivo.FileName).ToLower();
            var permitidas = new[] { ".png", ".jpg", ".jpeg", ".pdf" };
            if (!permitidas.Contains(extension))
            {
                TempData.SetSweetAlert(new DTOs.SweetAlert.SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Debe seleccionar un archivo válido",
                    Tipo = "error"
                });

                return RedirectToAction("IrATramite", "Tramite", new { tramiteId = viewModel.TramiteId });
            }

            // Mapear el tipo MIME
            string mimeType = extension switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".pdf" => "application/pdf",
                _ => "application/octet-stream"
            };

            ArchivoDTO archivoDTO = new ArchivoDTO
            {
                CategoriaArchivo = viewModel.CategoriaArchivo,
                TramiteId = viewModel.TramiteId,
                NombreArchivo = viewModel.Archivo.FileName ?? "sin nombre",
                Descripcion = viewModel.Descripcion,
                Archivo = viewModel.Archivo,
                MimeType = mimeType,
            };

            try
            {
                await _archivoService.Add(archivoDTO);

                TempData.SetSweetAlert(new DTOs.SweetAlert.SweetAlertDTO
                {
                    Titulo = "Éxito",
                    Mensaje = "Archivo subido correctamente",
                    Tipo = "success"
                });
                return RedirectToAction("IrATramite", "Tramite", new { tramiteId = viewModel.TramiteId });
            }
            catch (Exception ex)
            {
                TempData.SetSweetAlert(new DTOs.SweetAlert.SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Error al subir el archivo: " + ex.Message,
                    Tipo = "error"
                });

                return RedirectToAction("IrATramite", "Tramite", new { tramiteId = viewModel.TramiteId });
            }
        }

        [HttpPost]
        [AuthorizeRole(RolUsuario.Empleado, RolUsuario.Administrador)]
        public async Task<IActionResult> Editar(ArchivoVM viewModel)
        {
            if (string.IsNullOrWhiteSpace(viewModel.Descripcion))
            {
                TempData.SetSweetAlert(new DTOs.SweetAlert.SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "La descripcion del archivo es obligatoria",
                    Tipo = "error"
                });

                return RedirectToAction("IrATramite", "Tramite", new { tramiteId = viewModel.TramiteId });
            }

            if (viewModel.IdArchivo == null)
            {
                TempData.SetSweetAlert(new DTOs.SweetAlert.SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "No se encontro el Id del archivo",
                    Tipo = "error"
                });

                return RedirectToAction("IrATramite", "Tramite", new { tramiteId = viewModel.TramiteId });
            }

            ArchivoDTO archivoDTO = new ArchivoDTO
            {
                Id = viewModel.IdArchivo.Value,
                CategoriaArchivo = viewModel.CategoriaArchivo,
                TramiteId = viewModel.TramiteId,
                NombreArchivo = viewModel.Archivo?.FileName ?? "sin nombre",
                Descripcion = viewModel.Descripcion,
                Archivo = viewModel.Archivo,
            };

            try
            {
                await _archivoService.Update(archivoDTO);

                TempData.SetSweetAlert(new DTOs.SweetAlert.SweetAlertDTO
                {
                    Titulo = "Éxito",
                    Mensaje = "Archivo editado correctamente",
                    Tipo = "success"
                });
                return RedirectToAction("IrATramite", "Tramite", new { tramiteId = viewModel.TramiteId });
            }
            catch (Exception ex)
            {
                TempData.SetSweetAlert(new DTOs.SweetAlert.SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Error al editar el archivo: " + ex.Message,
                    Tipo = "error"
                });

                return RedirectToAction("IrATramite", "Tramite", new { tramiteId = viewModel.TramiteId });
            }
        }
    }
}
