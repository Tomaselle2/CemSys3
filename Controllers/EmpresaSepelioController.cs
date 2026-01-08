using CemSys3.Business.Usuario;
using CemSys3.DTOs.EmpresaSepelio;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.Usuario;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.EmpresaSepelio;
using CemSys3.Models;
using CemSys3.ViewModels.EmpresaSepelio;
using CemSys3.ViewModels.Usuario;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class EmpresaSepelioController : Controller
    {
        private readonly IEmpresaSepelio _empresaService;
        public EmpresaSepelioController(IEmpresaSepelio empresaSepelio)
        {
            _empresaService = empresaSepelio;
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> Index(string? filtro, int pagina = 1, int porPagina = 10)
        {
            var resultado = await _empresaService.GetAllPaginado(filtro, pagina, porPagina);

            var viewModel = new EmpresaSepelioVM
            {
                ListadoEmpresas = resultado.Items,
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
        public async Task<IActionResult> Guardar(EmpresaSepelioVM viewModel)
        {

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            EmpresaSepelioRequestDTO empresa = new EmpresaSepelioRequestDTO
            {
                Id = viewModel.Id ?? 0,
                Nombre = viewModel.Nombre
            };

            try
            {
                if (viewModel.Id.HasValue && viewModel.Id.Value > 0) //modifica
                {
                    await _empresaService.Update(empresa);
                    TempData.SetSweetAlert(
                        new SweetAlertDTO
                        {
                            Titulo = "Éxito",
                            Mensaje = "Empresa actualizada correctamente.",
                            Tipo = "success"
                        }
                    );
                }
                else
                {
                    await _empresaService.Add(empresa); //registra
                    TempData.SetSweetAlert(
                        new SweetAlertDTO
                        {
                            Titulo = "Éxito",
                            Mensaje = "Empresa registrada correctamente.",
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
                         Mensaje = "Ocurrió un error al guardar la empresa: " + ex.Message,
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
            var empresa = await _empresaService.GetById(id);

            if (empresa == null)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Empresa no encontrada.",
                    Tipo = "error"
                });

                return RedirectToAction("Index");
            }

            string filtro = string.Empty;
            int pagina = 1;
            int porPagina = 10;

            var resultado = await _empresaService.GetAllPaginado(filtro, pagina, porPagina);


            var vm = new EmpresaSepelioVM
            {
                Id = empresa.Id,
                Nombre = empresa.Nombre,
                ListadoEmpresas = resultado.Items,
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
                var empresa = await _empresaService.GetById(id);

                if (empresa == null)
                {
                    TempData.SetSweetAlert(new SweetAlertDTO
                    {
                        Titulo = "Error",
                        Mensaje = "Empresa no encontrada.",
                        Tipo = "error"
                    });

                    return RedirectToAction("Index");
                }

                await _empresaService.Delete(id);

                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Éxito",
                    Mensaje = "Empresa eliminada correctamente.",
                    Tipo = "success"
                });

            }
            catch (Exception ex) {
                
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Ocurrió un error al eliminar la empresa: " + ex.Message,
                    Tipo = "error"
                }); 
            }

            return RedirectToAction("Index");
        }
    }
}
