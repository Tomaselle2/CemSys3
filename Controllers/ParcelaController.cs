using CemSys3.Business.Concesion;
using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.Parcela;
using CemSys3.DTOs.Seccion;
using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers;
using CemSys3.Helpers.Enumerable;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Concesion;
using CemSys3.Interfaces.Parcela;
using CemSys3.Interfaces.Seccion;
using CemSys3.Models;
using CemSys3.ViewModels.Parcela;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class ParcelaController : Controller
    {
        private readonly IParcela _parcelaService;
        private readonly ISeccion _seccionService;
        private readonly IConcesion _concesionService;

        public ParcelaController(IParcela parcelaService, ISeccion seccionService, IConcesion concesionService)
        {
            _parcelaService = parcelaService;
            _seccionService = seccionService;
            _concesionService = concesionService;
        }

        //listado de parcelas de una seccion
        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> Index(int seccionId, int filtro = 0, int pagina = 1, int porPagina = 10)
        {
            ParcelaIndexVM viewModel = new ParcelaIndexVM();

            try
            {
                SeccionRequestDTO seccion = await _seccionService.Get(seccionId);
                PaginadoResponse<ParcelaIndexRequestDTO> resultado = await _parcelaService.GetAllPaginadoBySeccion(seccionId, filtro, pagina, porPagina);
                viewModel.NombreSeccion = seccion.Nombre.ToUpper();
                viewModel.TipoParcelaId = seccion.TipoParcelaId;

                viewModel.ListadoParcelas = resultado.Items;
                viewModel.Paginacion = resultado.Paginacion;

                viewModel.Paginacion.Parametros = new Dictionary<string, string>();

                viewModel.Paginacion.Parametros.Add("filtro", filtro.ToString());
                viewModel.Paginacion.Parametros.Add("seccionId", seccionId.ToString());
                viewModel.Paginacion.Parametros.Add("porPagina", porPagina.ToString());
            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al cargar las parcelas: {ex.Message}",
                    Tipo = "error"
                };
            }


            return View(viewModel);
        }

        //para cargar las parcelas en el select de la pantalla ingreso, se usan en vista parcial
        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> ObtenerParcelasPorSeccion(int seccionId, int estadoDifuntoId)
        {
            try
            {
                var parcelas = await _parcelaService.GetAllBySeccionId(seccionId, estadoDifuntoId);
                return Json(parcelas);
            }
            catch (Exception ex)
            {
                return Content($$"""
                <script>
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: '{{ex.Message.Replace("'", "\\'")}}'
                    });
                </script>
            """);
            }
        }


        //Vista de historial de parcela
        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> HistorialParcela(int parcelaId)
        {
            ParcelaHistorialVM viewModel = new ParcelaHistorialVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();

            try
            {
                viewModel.Historial = await _parcelaService.HistorialParcela(parcelaId);
                viewModel.ParcelaTieneConcesion = await _parcelaService.ParcelaTieneConcesion(parcelaId);
            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Ocurrió un error al cargar el historial: " + ex.Message,
                    Tipo = "error"
                };
            }

            return View(viewModel);
        }

        [HttpPost]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> Guardar(ParcelaHistorialVM viewModel)
        {

            ModificarParcelaDTO dto = new ModificarParcelaDTO();
            dto.Id = viewModel.Historial.Id;
            dto.TipoPanteonId = viewModel.Historial.TipoPanteonId;
            dto.TipoNichoId = viewModel.Historial.TipoNichoId;
            dto.NombrePanteon = viewModel.Historial.NombrePanteon;
            dto.infoAdicional = viewModel.Historial.infoAdicional;

            try
            {
                await _parcelaService.UpdateParcela(dto);
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Éxito",
                    Mensaje = "Parcela guardada exitosamente.",
                    Tipo = "success"
                });

                return RedirectToAction("HistorialParcela", new { parcelaId = viewModel.Historial.Id });
            }
            catch (Exception ex)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al guardar los datos de la parcela: {ex.Message}",
                    Tipo = "error"
                });

                return View(viewModel);
            }
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado, RolUsuario.Administrador)]
        public async Task<IActionResult> ExportarNichosDisponibles()
        {
            var datos = await _parcelaService.GetAllNichosDisponibles();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Nichos Disponibles");

            // Estilo encabezado de sección
            var estiloSeccion = ws.Style;

            int fila = 1;

            // Agrupar por sección
            var porSeccion = datos
                .GroupBy(p => new { p.SeccionId, p.NombreSeccion })
                .OrderBy(g => g.Key.NombreSeccion);

            bool primeraSeccion = true;

            foreach (var seccion in porSeccion)
            {
                if (!primeraSeccion)
                {
                    // 3 filas vacías entre secciones
                    fila += 3;
                }
                primeraSeccion = false;

                // ── Título de sección ──
                var celdaTitulo = ws.Cell(fila, 1);
                celdaTitulo.Value = $"SECCIÓN: {seccion.Key.NombreSeccion?.ToUpper()}";
                celdaTitulo.Style.Font.Bold = true;
                celdaTitulo.Style.Font.FontSize = 12;
                celdaTitulo.Style.Fill.BackgroundColor = XLColor.FromHtml("#2E75B6");
                celdaTitulo.Style.Font.FontColor = XLColor.White;
                ws.Range(fila, 1, fila, 3).Merge();
                ws.Range(fila, 1, fila, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                fila++;

                // ── Encabezados de columna ──
                string[] headers = { "Sección", "Parcela", "Tipo de Nicho" };
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = ws.Cell(fila, i + 1);
                    cell.Value = headers[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#BDD7EE");
                    cell.Style.Font.FontColor = XLColor.Black;
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                }
                fila++;

                // ── Filas de datos ──
                foreach (var nicho in seccion.OrderBy(p => p.NroFila).ThenBy(p => p.NroParcela))
                {
                    string parcela = $"Nicho {nicho.NroParcela} - Fila {nicho.NroFila}";
                    string tipoNicho = EnumHelper.GetDisplayNameByValue<TipoNichoEnum>(nicho.TipoNichoId ?? 0);

                    ws.Cell(fila, 1).Value = nicho.NombreSeccion?.ToUpper() ?? "";
                    ws.Cell(fila, 2).Value = parcela;
                    ws.Cell(fila, 3).Value = tipoNicho;

                    // Filas alternas para mejor legibilidad
                    if (fila % 2 == 0)
                        ws.Row(fila).Style.Fill.BackgroundColor = XLColor.FromHtml("#F2F7FB");

                    fila++;
                }
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            string fileName = $"NichosDisponibles_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> GenerarConcesionManualmente(int parcelaId)
        {
            ConcesionDTO concesionNueva = new ConcesionDTO
            {
                ParcelaId = parcelaId,
                UsuarioId = HttpContext.Session.GetInt32("IdUsuario") ?? 0,
                FechaInicio = DateTime.Now,
            };

            GenericResultDTO resultado = new GenericResultDTO();

            try
            {
                resultado = await _concesionService.AddManualmente(concesionNueva);

                if(resultado.Success)
                {
                    TempData.SetSweetAlert(new SweetAlertDTO
                    {
                        Titulo = "Éxito",
                        Mensaje = $"Concesión realizada correctamente",
                        Tipo = "success"
                    });
                }

            } catch (Exception ex) {

                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Ocurrió un error al realizar la concesión. " + ex.Message,
                    Tipo = "error"
                });

                return RedirectToAction("HistorialParcela", new { parcelaId = parcelaId });

            }

            return RedirectToAction("Concesion", "Concesion", new { tramiteId = resultado.Id });
        }
    }
}
