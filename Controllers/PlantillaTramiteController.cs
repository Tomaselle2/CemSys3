using CemSys3.DTOs.Persona;
using CemSys3.DTOs.PlantillaTramite;
using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.Tarea;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.PlantillaTramite;
using CemSys3.Interfaces.Tarea;
using CemSys3.Interfaces.TramitesConcesion;
using CemSys3.ViewModels.PlantillaTramite;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CemSys3.Controllers
{
    public class PlantillaTramiteController : Controller
    {
        private readonly IPlantillaTramite _service;
        private readonly ITareaPlantilla _tareaPlantillaService;
        private readonly IRequisitos _requisitosService;

        public PlantillaTramiteController(IPlantillaTramite service, ITareaPlantilla tareaPlantillaService, IRequisitos requisitosService)
        {
            _service = service;
            _tareaPlantillaService = tareaPlantillaService;
            _requisitosService = requisitosService;
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

                viewModel.Requisitos = await _requisitosService.GetByTipoTramiteId(viewModel.Dto.TipoTramiteId);
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

            return View(viewModel.vista, viewModel);
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> AceptacionTitularidad(int plantillaId)
        {
            PlantillaTramiteVM viewModel = new PlantillaTramiteVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();

            try
            {
                viewModel.Dto = await _service.ObtenerPorIdAsync(plantillaId)
                    ?? new PlantillaTramiteDTO();

                viewModel.Tareas = await _tareaPlantillaService.GetAllByTipoTramite(viewModel.Dto.TipoTramiteId);


                viewModel.vista = "AceptacionTitularidad";

                viewModel.Requisitos = await _requisitosService.GetByTipoTramiteId(viewModel.Dto.TipoTramiteId);
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

            return View(viewModel.vista, viewModel);
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> CremacionAutorizacion(int plantillaId)
        {
            PlantillaTramiteVM viewModel = new PlantillaTramiteVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();

            try
            {
                viewModel.Dto = await _service.ObtenerPorIdAsync(plantillaId)
                    ?? new PlantillaTramiteDTO();

                viewModel.Tareas = await _tareaPlantillaService.GetAllByTipoTramite(viewModel.Dto.TipoTramiteId);


                viewModel.vista = "CremacionAutorizacion";

                viewModel.Requisitos = await _requisitosService.GetByTipoTramiteId(viewModel.Dto.TipoTramiteId);
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

            return View(viewModel.vista, viewModel);
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

                await _requisitosService.Update(
                    viewModel.Dto.TipoTramiteId,
                    viewModel.Requisitos.Descripcion
                );

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
