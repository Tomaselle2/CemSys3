using CemSys3.DTOs.Cementerio;
using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Cementerio;
using CemSys3.ViewModels.Cementerio;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class CementerioController : Controller
    {
        private readonly ICementerio _cementerioService;

        public CementerioController(ICementerio cementerio)
        {
            _cementerioService = cementerio;
        }
        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> Index(string? filtro, int pagina = 1, int porPagina = 10)
        {
            var resultado = await _cementerioService.GetAllPaginado(filtro, pagina, porPagina);

            var viewModel = new CementerioVM
            {
                ListadoCementerios = resultado.Items,
                Paginacion = resultado.Paginacion
            };

            // Mantener filtros en la paginación
            viewModel.Paginacion.Parametros = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                viewModel.Paginacion.Parametros.Add("filtro", filtro);
            }

            viewModel.Paginacion.Parametros.Add("porPagina", porPagina.ToString());
            viewModel.SweetAlert = TempData.GetSweetAlert();

            return View(viewModel);
        }

        [HttpPost]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> Guardar(CementerioVM viewModel)
        {

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            CementerioRequestDTO cementerio = new CementerioRequestDTO
            {
                Id = viewModel.Id ?? 0,
                Nombre = viewModel.Nombre
            };

            try
            {
                if (viewModel.Id.HasValue && viewModel.Id.Value > 0) //modifica
                {
                    await _cementerioService.Update(cementerio);
                    TempData.SetSweetAlert(
                        new SweetAlertDTO
                        {
                            Titulo = "Éxito",
                            Mensaje = "Cementerio actualizado correctamente.",
                            Tipo = "success"
                        }
                    );
                }
                else
                {
                    await _cementerioService.Add(cementerio); //registra
                    TempData.SetSweetAlert(
                        new SweetAlertDTO
                        {
                            Titulo = "Éxito",
                            Mensaje = "Cementerio registrado correctamente.",
                            Tipo = "success"
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                TempData.SetSweetAlert(
                     new SweetAlertDTO
                     {
                         Titulo = "Error",
                         Mensaje = "Ocurrió un error al guardar el cementerio: " + ex.Message,
                         Tipo = "error"
                     }
                );
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> Editar(int id)
        {
            var cementerio = await _cementerioService.GetById(id);

            if (cementerio == null)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Cementerio no encontrado.",
                    Tipo = "error"
                });

                return RedirectToAction("Index");
            }

            string filtro = string.Empty;
            int pagina = 1;
            int porPagina = 10;

            var resultado = await _cementerioService.GetAllPaginado(filtro, pagina, porPagina);


            var vm = new CementerioVM
            {
                Id = cementerio.Id,
                Nombre = cementerio.Nombre,
                ListadoCementerios = resultado.Items,
                Paginacion = resultado.Paginacion
            };

            return View("Index", vm);
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var empresa = await _cementerioService.GetById(id);

                if (empresa == null)
                {
                    TempData.SetSweetAlert(new SweetAlertDTO
                    {
                        Titulo = "Error",
                        Mensaje = "Cementerio no encontrado.",
                        Tipo = "error"
                    });

                    return RedirectToAction("Index");
                }

                await _cementerioService.Delete(id);

                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Éxito",
                    Mensaje = "Cementerio eliminado correctamente.",
                    Tipo = "success"
                });

            }
            catch (Exception ex)
            {

                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Ocurrió un error al eliminar el cementerio: " + ex.Message,
                    Tipo = "error"
                });
            }

            return RedirectToAction("Index");
        }
    }
}
