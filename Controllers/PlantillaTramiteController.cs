using CemSys3.DTOs.Persona;
using CemSys3.DTOs.PlantillaTramite;
using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.Tarea;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.PlantillaTramite;
using CemSys3.Interfaces.Tarea;
using CemSys3.ViewModels.PlantillaTramite;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CemSys3.Controllers
{
    public class PlantillaTramiteController : Controller
    {
        private readonly IPlantillaTramite _service;
        private readonly ITareaPlantilla _tareaPlantillaService;

        public PlantillaTramiteController(IPlantillaTramite service, ITareaPlantilla tareaPlantillaService)
        {
            _service = service;
            _tareaPlantillaService = tareaPlantillaService;
        }

        //vista general donde aparecen todas las plantillas de trámite, con opciones para editar.
        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public IActionResult IndexPlantillas()
        {
            return View();
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> CambioTitularPresente(int plantillaId)
        {
            PlantillaTramiteVM viewModel = new PlantillaTramiteVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();

            try
            {
                viewModel.Dto = await _service.ObtenerPorIdAsync(plantillaId)
                    ?? new PlantillaTramiteDTO();

                viewModel.Tareas = await _tareaPlantillaService.GetAllByTipoTramite(viewModel.Dto.TipoTramiteId);


                viewModel.vista = "CambioTitularPresente";
            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Error al cargar la plantilla: {ex.Message}",
                    Tipo = "error"
                };
            }

            return View("CambioTitularPresente", viewModel);
        }

        [HttpPost]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> Guardar(PlantillaTramiteVM viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.Dto?.Contenido))
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "El contenido no puede estar vacío.",
                    Tipo = "error"
                };

                return View(viewModel.vista, viewModel);
            }

            try
            {
                // 🔥 CKEDITOR FIX
                viewModel.Dto.Contenido = WebUtility.HtmlDecode(viewModel.Dto.Contenido);

                int plantillaId;

                if (viewModel.Dto.PlantillaId == 0)
                {
                    plantillaId = await _service.CrearAsync(viewModel.Dto);
                }
                else
                {
                    plantillaId = await _service.ActualizarAsync(viewModel.Dto);
                }

                if (viewModel.Tareas != null && viewModel.Tareas.Any())
                {
                    await _tareaPlantillaService.GuardarTareas(
                        viewModel.Dto.TipoTramiteId,
                        viewModel.Tareas
                    );
                }

                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Éxito",
                    Mensaje = "Plantilla guardada correctamente.",
                    Tipo = "success"
                });

                return RedirectToAction(viewModel.vista, new { plantillaId });
            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Error al guardar: {ex.Message}",
                    Tipo = "error"
                };

                return View(viewModel.vista, viewModel);
            }
        }

        
    }
}
