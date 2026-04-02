using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.ViewModels.Imagenes;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class ImagenesController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public ImagenesController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public IActionResult Index()
        {
            ImagenesVM viewModel = new ImagenesVM();

            var ruta = Path.Combine(_env.WebRootPath, "config", "intendente.txt");

            string nombre = "";

            if (System.IO.File.Exists(ruta))
            {
                nombre = System.IO.File.ReadAllText(ruta);
            }

            viewModel.nombreIntendente = nombre;
            viewModel.SweetAlert = TempData.GetSweetAlert();

            return View(viewModel);
        }

        [HttpPost]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> SubirLogo(IFormFile archivo)
        {
            if (archivo == null || archivo?.Length == 0)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Advertencia",
                    Mensaje = "Debe seleccionar una imagen",
                    Tipo = "warning"
                });

                return RedirectToAction("Index");
            }

            try
            {
                if(archivo != null && archivo.Length > 0)
                {
                    var ruta = Path.Combine(_env.WebRootPath, "fotos", "logoMuni.png");

                    using (var stream = new FileStream(ruta, FileMode.Create))
                    {
                        await archivo.CopyToAsync(stream);
                    }
                }

                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Éxito",
                    Mensaje = "La imagen se ha subido correctamente",
                    Tipo = "success"
                });

            }
            catch (Exception ex)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "La imagen no se ha subido." + ex.Message,
                    Tipo = "error"
                });
            }
                
            

            return RedirectToAction("Index");
        }

        [HttpPost]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> SubirPie(IFormFile archivo)
        {
            if (archivo == null || archivo?.Length == 0)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Advertencia",
                    Mensaje = "Debe seleccionar una imagen",
                    Tipo = "warning"
                });

                return RedirectToAction("Index");
            }

            try
            {
                if (archivo != null && archivo.Length > 0)
                {
                    var ruta = Path.Combine(_env.WebRootPath, "fotos", "pieContrato.png");

                    using (var stream = new FileStream(ruta, FileMode.Create))
                    {
                        await archivo.CopyToAsync(stream);
                    }
                }

                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Éxito",
                    Mensaje = "La imagen se ha subido correctamente",
                    Tipo = "success"
                });

            }
            catch (Exception ex)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "La imagen no se ha subido." + ex.Message,
                    Tipo = "error"
                });
            }



            return RedirectToAction("Index");
        }

        [HttpPost]
        [AuthorizeRole(RolUsuario.Administrador)]
        public IActionResult GuardarIntendente(ImagenesVM viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.nombreIntendente))
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Advertencia",
                    Mensaje = "Revise el nombre del intendente. No puede estar vacio.",
                    Tipo = "warning"
                });

                return RedirectToAction("Index");
            }

            try
            {
                var ruta = Path.Combine(_env.WebRootPath, "config", "intendente.txt");

                System.IO.File.WriteAllText(ruta, viewModel.nombreIntendente.Trim().ToUpper());

                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Éxito",
                    Mensaje = "Nombre del intendente modificado correctamente",
                    Tipo = "success"
                });
            }
            catch (Exception ex)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Error al modificar el nomrbe." + ex.Message,
                    Tipo = "error"
                });

            }

            return RedirectToAction("Index");

        }
    }
}
