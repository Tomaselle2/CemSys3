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
        private readonly IPlantillaTramite _plantillaTramiteService;

        public PlantillaTramiteController(IPlantillaTramite plantillaTramiteService)
        {
            _plantillaTramiteService = plantillaTramiteService;
        }

        //vista general donde aparecen todas las plantillas de trámite, con opciones para editar.
        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public IActionResult Index()
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
                viewModel.Dto = await _plantillaTramiteService.Get(plantillaId);
                viewModel.vista = "CambioTitularPresente";
            }
            catch (Exception ex) {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al cargar la plantilla de trámite: {ex.Message}",
                    Tipo = "error"
                };
            }

            return View(viewModel);
        }

        [HttpPost]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> Guardar(PlantillaTramiteVM viewModel)
        {
            if(viewModel.Dto.Contenido == null || viewModel.Dto?.Contenido?.Length == 0)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "El contenido de la plantilla de trámite no puede estar vacío.",
                    Tipo = "error"
                };

                return View($"{viewModel.vista}", viewModel);
            }

            try
            {
                viewModel.Dto.Contenido = WebUtility.HtmlDecode(viewModel.Dto.Contenido);
                int plantillaId = await _plantillaTramiteService.Update(viewModel.Dto);

                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Éxito",
                    Mensaje = "La plantilla de trámite se ha guardado correctamente.",
                    Tipo = "success"
                }); 

                return RedirectToAction($"{viewModel.vista}", new { plantillaId = viewModel?.Dto?.PlantillaId });

            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al guardar la plantilla de trámite: {ex.Message}",
                    Tipo = "error"
                };
                return View($"{viewModel.vista}", viewModel);
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
