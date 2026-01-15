using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.Seccion;
using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Parcela;
using CemSys3.Interfaces.Seccion;
using CemSys3.ViewModels.Seccion;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CemSys3.Controllers
{
    public class SeccionController : Controller
    {
        private readonly ISeccion _seccionService;
        private readonly IParcela _parcelaService;

        public SeccionController(ISeccion seccion, IParcela parcelaService)
        {
            _seccionService = seccion;
            _parcelaService = parcelaService;
        }

        public IActionResult Index()
        {
            return View();
        }

        //vista de secciones de nichos
        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador, RolUsuario.Empleado)]
        public async Task<IActionResult> IndexSeccionesNichos(string? filtro, int pagina = 1, int porPagina = 10)
        {
            SeccionVM viewModel = new SeccionVM();

            try
            {
                await ListarSecciones((int)TipoParcelaEnum.Nicho, viewModel, filtro, pagina, porPagina);
            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al cargar las secciones de nichos: {ex.Message}",
                    Tipo = "error"
                };
            }

            viewModel.TipoParcelaId = (int)TipoParcelaEnum.Nicho;
            viewModel.SweetAlert = TempData.GetSweetAlert();

            return View(viewModel);
        }

        //vista de secciones de fosas
        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador, RolUsuario.Empleado)]
        public async Task<IActionResult> IndexSeccionesFosas(string? filtro, int pagina = 1, int porPagina = 10)
        {
            SeccionVM viewModel = new SeccionVM();

            try
            {
                await ListarSecciones((int)TipoParcelaEnum.Fosa, viewModel, filtro, pagina, porPagina);
            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al cargar las secciones de fosas: {ex.Message}",
                    Tipo = "error"
                };
            }

            viewModel.TipoParcelaId = (int)TipoParcelaEnum.Fosa;
            viewModel.Filas = 1; // Las fosas no tienen filas, 1 es por defecto
            viewModel.TipoNumeracionParcelaId = (int)TipoNumeracionParcelaEnum.Antigua; // Las fosas siempre tienen numeración antigua
            viewModel.SweetAlert = TempData.GetSweetAlert();


            return View(viewModel);
        }

        //vista de secciones de panteones
        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador, RolUsuario.Empleado)]
        public async Task<IActionResult> IndexSeccionesPanteones(string? filtro, int pagina = 1, int porPagina = 10)
        {
            SeccionVM viewModel = new SeccionVM();

            try
            {
                await ListarSecciones((int)TipoParcelaEnum.Panteon, viewModel, filtro, pagina, porPagina);
            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al cargar las secciones de panteones: {ex.Message}",
                    Tipo = "error"
                };
            }

            viewModel.TipoParcelaId = (int)TipoParcelaEnum.Panteon;
            viewModel.Filas = 1; // Los panteones no tienen filas, 1 es por defecto
            viewModel.TipoNumeracionParcelaId = (int)TipoNumeracionParcelaEnum.Antigua; // Los panteones siempre tienen numeración antigua
            viewModel.SweetAlert = TempData.GetSweetAlert();
            return View(viewModel);
        }

        [HttpPost]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> Guardar(SeccionVM viewModel)
        {
            string vistaRedirigir = ObtenerVistaRedirigir(viewModel.TipoParcelaId);

            if (!ModelState.IsValid)
            {
                return View(vistaRedirigir, viewModel);
            }

            SeccionRequestDTO seccion = new SeccionRequestDTO
            {
                Id = viewModel.Id ?? 0,
                Nombre = viewModel.Nombre.Trim(),
                NroParcelas = viewModel.NroParcelas.Value,
                Filas = viewModel.Filas.Value,
                TipoNumeracionParcelaId = viewModel.TipoNumeracionParcelaId,
                TipoParcelaId = viewModel.TipoParcelaId
            };

            try
            {
                if (viewModel.Id.HasValue && viewModel.Id.Value > 0) //modifica
                {
                    await _seccionService.Update(seccion);
                    TempData.SetSweetAlert(
                        new SweetAlertDTO
                        {
                            Titulo = "Éxito",
                            Mensaje = "Sección actualizada correctamente.",
                            Tipo = "success"
                        }
                    );
                }
                else
                {
                    await _seccionService.Add(seccion); //registra
                    TempData.SetSweetAlert(
                        new SweetAlertDTO
                        {
                            Titulo = "Éxito",
                            Mensaje = "Sección registrada correctamente.",
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
                         Mensaje = "Ocurrió un error al guardar la sección: " + ex.Message,
                         Tipo = "error"
                     }
                );
            }

            return RedirectToAction(vistaRedirigir);
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> Editar(int id, int TipoParcelaId)
        {
            string vistaRedirigir = ObtenerVistaRedirigir(TipoParcelaId);


            SeccionRequestDTO seccion = await _seccionService.Get(id);

            if (seccion == null)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Sección no encontrada.",
                    Tipo = "error"
                });

                return RedirectToAction(vistaRedirigir);
            }

            string filtro = string.Empty;
            int pagina = 1;
            int porPagina = 10;
            
            SeccionVM viewModel = new SeccionVM();

            await ListarSecciones(TipoParcelaId, viewModel, filtro, pagina, porPagina);

            viewModel.Id = seccion.Id;
            viewModel.Nombre = seccion.Nombre;
            viewModel.NroParcelas = seccion.NroParcelas;
            viewModel.Filas = seccion.Filas;
            viewModel.TipoNumeracionParcelaId = seccion.TipoNumeracionParcelaId;
            viewModel.TipoParcelaId = seccion.TipoParcelaId;


            return View(vistaRedirigir, viewModel);
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> Eliminar(int id, int TipoParcelaId)
        {
            string vistaRedirigir = ObtenerVistaRedirigir(TipoParcelaId);

            try
            {
                SeccionRequestDTO seccion = await _seccionService.Get(id);

                if (seccion == null)
                {
                    TempData.SetSweetAlert(new SweetAlertDTO
                    {
                        Titulo = "Error",
                        Mensaje = "Sección no encontrada.",
                        Tipo = "error"
                    });

                    return RedirectToAction("Index");
                }

                await _seccionService.Delete(id);

                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Éxito",
                    Mensaje = "Sección eliminada correctamente.",
                    Tipo = "success"
                });

            }
            catch (Exception ex)
            {

                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Ocurrió un error al eliminar la sección: " + ex.Message,
                    Tipo = "error"
                });
            }

            return RedirectToAction(vistaRedirigir);
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> AddOne(int id, int TipoParcelaId)
        {
            string vistaRedirigir = ObtenerVistaRedirigir(TipoParcelaId);

            try
            {
                GenericResultDTO resultado = await _parcelaService.AddOne(id);

                if(resultado.Id != null && resultado.Id > 0)
                {
                    TempData.SetSweetAlert(new SweetAlertDTO
                    {
                        Titulo = "Éxito",
                        Mensaje = resultado.Message,
                        Tipo = "success"
                    });
                }
            }
            catch (Exception ex)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Ocurrió un error al agregar una parcela: " + ex.Message,
                    Tipo = "error"
                });
            }

            return RedirectToAction(vistaRedirigir);
        }

        private async Task ListarSecciones(int tipoId, SeccionVM viewModel, string? filtro, int pagina = 1, int porPagina = 10)
        {
            PaginadoResponse<SeccionRequestDTO> resultado = await _seccionService.GetAllByTipoPaginado(tipoId, filtro, pagina, porPagina);
            viewModel.ListadoSecciones = resultado.Items;
            viewModel.Paginacion = resultado.Paginacion;

            viewModel.Paginacion.Parametros = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                viewModel.Paginacion.Parametros.Add("filtro", filtro);
            }

            viewModel.Paginacion.Parametros.Add("porPagina", porPagina.ToString());
        }

        private string ObtenerVistaRedirigir(int tipoParcelaId)
        {
            return tipoParcelaId switch
            {
                (int)TipoParcelaEnum.Nicho => nameof(IndexSeccionesNichos),
                (int)TipoParcelaEnum.Fosa => nameof(IndexSeccionesFosas),
                (int)TipoParcelaEnum.Panteon => nameof(IndexSeccionesPanteones),
                _ => nameof(IndexSeccionesNichos)
            };

        }
    }
}
