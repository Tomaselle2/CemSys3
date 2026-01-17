using CemSys3.Business.Seccion;
using CemSys3.DTOs.ConceptosTarifaria;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.ConceptoTarifaria;
using CemSys3.ViewModels.ConceptoTarifaria;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CemSys3.Controllers
{
    public class ConceptoTarifariaController : Controller
    {
        private readonly IConceptoTarifaria _conceptoService;

        public ConceptoTarifariaController(IConceptoTarifaria conceptoService)
        {
            _conceptoService = conceptoService;
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> Index(string? filtro, int pagina = 1, int porPagina = 10)
        {
            ConceptoTarifariaVM viewModel = new ConceptoTarifariaVM();
            try
            {
                await ListarConceptosTarifaria(viewModel, filtro, pagina, porPagina);
            }
            catch (Exception ex) {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al cargar los conceptos de la tarifaria: {ex.Message}",
                    Tipo = "error"
                };
            }

            viewModel.SweetAlert = TempData.GetSweetAlert();
            return View(viewModel);
        }

        [HttpPost]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> Guardar(ConceptoTarifariaVM viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            ConceptoTarifariaDTO concepto = new ConceptoTarifariaDTO
            {
                Id = viewModel.Id ?? 0,
                Nombre = viewModel.Nombre,
                TemaId = viewModel.TemaId.Value,
                Visibilidad = true
            };

            try
            {
                if (viewModel.Id.HasValue && viewModel.Id.Value > 0) //modifica
                {
                    await _conceptoService.Update(concepto);
                    TempData.SetSweetAlert(
                        new SweetAlertDTO
                        {
                            Titulo = "Éxito",
                            Mensaje = "Concepto actualizado correctamente.",
                            Tipo = "success"
                        }
                    );
                }
                else
                {
                    await _conceptoService.Add(concepto); //registra
                    TempData.SetSweetAlert(
                        new SweetAlertDTO
                        {
                            Titulo = "Éxito",
                            Mensaje = "Concepto registrado correctamente.",
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
                         Mensaje = "Ocurrió un error al guardar el concepto: " + ex.Message,
                         Tipo = "error"
                     }
                );
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> Editar(int id, int TipoParcelaId)
        {
            ConceptoTarifariaDTO concepto = await _conceptoService.Get(id);

            if (concepto == null)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Concepto no encontrado.",
                    Tipo = "error"
                });

                return RedirectToAction("Index");
            }

            string filtro = string.Empty;
            int pagina = 1;
            int porPagina = 10;

            ConceptoTarifariaVM viewModel = new ConceptoTarifariaVM();

            await ListarConceptosTarifaria(viewModel, filtro, pagina, porPagina);

            viewModel.Id = concepto.Id;
            viewModel.Nombre = concepto.Nombre;
            viewModel.TemaId = concepto.TemaId;

            return View("Index", viewModel);

        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> Eliminar(int id, int TipoParcelaId)
        {
            try
            {
                ConceptoTarifariaDTO concepto = await _conceptoService.Get(id);

                if (concepto == null)
                {
                    TempData.SetSweetAlert(new SweetAlertDTO
                    {
                        Titulo = "Error",
                        Mensaje = "Concepto no encontrado.",
                        Tipo = "error"
                    });

                    return RedirectToAction("Index");
                }

                await _conceptoService.Delete(id);

                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Éxito",
                    Mensaje = "Concepto eliminado correctamente.",
                    Tipo = "success"
                });
            }
            catch (Exception ex) {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Ocurrió un error al eliminar el concepto: " + ex.Message,
                    Tipo = "error"
                });
            }

            return RedirectToAction("Index");
        }

        private async Task ListarConceptosTarifaria(ConceptoTarifariaVM viewModel, string? filtro, int pagina = 1, int porPagina = 10)
        {
            PaginadoResponse<ConceptoTarifariaDTO> resultado = await _conceptoService.GetAllPaginado(filtro, pagina, porPagina);

            viewModel.Paginacion = resultado.Paginacion;
            viewModel.ListadoConceptos = resultado.Items;

            viewModel.Paginacion.Parametros = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                viewModel.Paginacion.Parametros.Add("filtro", filtro);
            }

            viewModel.Paginacion.Parametros.Add("porPagina", porPagina.ToString());
        }
    }
}
