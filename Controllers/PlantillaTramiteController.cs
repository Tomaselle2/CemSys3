using CemSys3.DTOs.Persona;
using CemSys3.DTOs.PlantillaTramite;
using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.PlantillaTramite;
using CemSys3.ViewModels.PlantillaTramite;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CemSys3.Controllers
{
    public class PlantillaTramiteController : Controller
    {
        private readonly IStrategyFactory _factory;
        private readonly IPlantillaTramite _service;


        public PlantillaTramiteController(IStrategyFactory factory, IPlantillaTramite service)
        {
            _factory = factory;
            _service = service;
        }

        //vista general donde aparecen todas las plantillas de trámite, con opciones para editar.
        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public IActionResult IndexPlantillas()
        {
            return View();
        }

        public async Task<IActionResult> Index(int tipoTramiteId)
        {
            var plantillas = await _service.ObtenerPorTipoTramiteAsync(tipoTramiteId);
            return View(plantillas);
        }

        public IActionResult Crear(int tipoTramiteId)
        {
            return View(new PlantillaTramiteDTO { TipoTramiteId = tipoTramiteId });
        }

        [HttpPost]
        public async Task<IActionResult> Crear(PlantillaTramiteDTO dto)
        {
            await _service.CrearAsync(dto);
            return RedirectToAction("Index", new { tipoTramiteId = dto.TipoTramiteId });
        }

        public async Task<IActionResult> Editar(int id)
        {
            var plantilla = await _service.ObtenerPorIdAsync(id);
            return View(plantilla);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(PlantillaTramiteDTO dto)
        {
            await _service.ActualizarAsync(dto);
            return RedirectToAction("Index", new { tipoTramiteId = dto.TipoTramiteId });
        }

        public async Task<IActionResult> Eliminar(int id, int tipoTramiteId)
        {
            await _service.EliminarAsync(id);
            return RedirectToAction("Index", new { tipoTramiteId });
        }

        [HttpPost]
        public async Task<IActionResult> GenerarAutorizaciones(
        int tramiteId,
        int tipoTramiteId,
        List<TitularesContratoDTO> titularesActuales, List<TitularesContratoDTO> nuevosTitulares)
        {
            var strategy = _factory.GetStrategy(tipoTramiteId);
            string parentesco = "Familiar";
            await strategy.GenerarAsync(tramiteId, titularesActuales, nuevosTitulares, 1, parentesco); // usuarioId
            return Ok();
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

        [HttpPost]
        public async Task<IActionResult> SubirImagen(IFormFile upload)
        {
            if (upload == null || upload.Length == 0)
                return BadRequest("Archivo inválido");

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(upload.FileName);

            var ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/fotos", fileName);

            using (var stream = new FileStream(ruta, FileMode.Create))
            {
                await upload.CopyToAsync(stream);
            }

            var url = "/fotos/" + fileName;

            return Json(new
            {
                uploaded = true,
                url = url
            });
        }
    }
}
