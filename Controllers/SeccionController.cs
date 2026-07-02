using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.Parcela;
using CemSys3.DTOs.Seccion;
using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Parcela;
using CemSys3.Interfaces.Seccion;
using CemSys3.ViewModels.Seccion;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;

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
                Nombre = viewModel.Nombre.Trim().ToUpper(),
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
            viewModel.Nombre = seccion.Nombre.Trim().ToUpper();
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

                    return RedirectToAction(vistaRedirigir);
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

        //para obtener las secciones por tipo de parcela (ajax) (se usa en ingreso)
        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> ObtenerSeccionesPorTipo(int tipoParcelaId)
        {
            try
            {
                var secciones = await _seccionService.GetAllByTipo(tipoParcelaId);
                return Json(secciones);
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




        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> ExportarSecciones()
        {
            var datos = await _seccionService.GetAllSeccionesExcel();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Secciones");

            // ── Encabezados (nombre de propiedad) ──
            string[] headers = { "Id", "Nombre", "Visibilidad", "Filas", "NroParcelas", "TipoNumeracionParcelaId", "TipoParcelaId" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#BDD7EE");
                cell.Style.Font.FontColor = XLColor.Black;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            }

            // ── Filas de datos ──
            int fila = 2;
            foreach (var sec in datos.OrderBy(s => s.Id))
            {
                ws.Cell(fila, 1).Value = sec.Id;
                ws.Cell(fila, 2).Value = sec.Nombre;
                ws.Cell(fila, 3).Value = sec.Visibilidad ? 1 : 0;
                ws.Cell(fila, 4).Value = sec.Filas;
                ws.Cell(fila, 5).Value = sec.NroParcelas;
                ws.Cell(fila, 6).Value = sec.TipoNumeracionParcelaId;
                ws.Cell(fila, 7).Value = sec.TipoParcelaId;

                if (fila % 2 == 0)
                    ws.Row(fila).Style.Fill.BackgroundColor = XLColor.FromHtml("#F2F7FB");

                fila++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            string fileName = $"Secciones_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> ExportarParcelas()
        {
            var datos = await _parcelaService.GetAllParcelasExcel();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Parcelas");

            // ── Encabezados (nombre de propiedad) ──
            string[] headers =
            {
        "Id", "Visibilidad", "NroParcela", "NroFila", "CantidadDifuntos", "NombrePanteon", "InformacionAdicional", "SeccionId",
        "TipoNichoId", "TipoPanteonId", "TipoParcelaId",   
         
    };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#BDD7EE");
                cell.Style.Font.FontColor = XLColor.Black;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                cell.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            }

            // ── Filas de datos ──
            int fila = 2;
            foreach (var p in datos.OrderBy(p => p.SeccionId).ThenBy(p => p.NroFila).ThenBy(p => p.NroParcela))
            {
                ws.Cell(fila, 1).Value = p.Id;
                ws.Cell(fila, 2).Value = p.Visibilidad ? 1 : 0;
                ws.Cell(fila, 3).Value = p.NroParcela;
                ws.Cell(fila, 4).Value = p.NroFila;
                ws.Cell(fila, 5).Value = p.CantidadDifuntos;
                ws.Cell(fila, 6).Value = p.NombrePanteon;
                ws.Cell(fila, 7).Value = p.InformacionAdicional ?? "";
                ws.Cell(fila, 8).Value = p.SeccionId;
                ws.Cell(fila, 9).Value = p.TipoNichoId;
                ws.Cell(fila, 10).Value = p.TipoPanteonId;
                ws.Cell(fila, 11).Value = p.TipoParcelaId;

                if (fila % 2 == 0)
                    ws.Row(fila).Style.Fill.BackgroundColor = XLColor.FromHtml("#F2F7FB");

                fila++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            string fileName = $"Parcelas_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        [HttpPost]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> ImportarSecciones(IFormFile excel)
        {
            if (excel == null || excel.Length == 0)
                return BadRequest("Debe adjuntar un archivo Excel.");

            var secciones = new List<SeccionDTO>();
            var errores = new List<string>();

            using (var stream = new MemoryStream())
            {
                await excel.CopyToAsync(stream);
                using var wb = new XLWorkbook(stream);
                var ws = wb.Worksheet(1);
                var filas = ws.RangeUsed()!.RowsUsed().Skip(1); // saltar encabezado

                foreach (var fila in filas)
                {
                    try
                    {
                        secciones.Add(new SeccionDTO
                        {
                            Id = fila.Cell(1).GetValue<int>(),
                            Nombre = fila.Cell(2).GetString(),
                            Visibilidad = fila.Cell(3).GetValue<int>() == 1,
                            Filas = fila.Cell(4).GetValue<int>(),
                            NroParcelas = fila.Cell(5).GetValue<int>(),
                            TipoNumeracionParcelaId = fila.Cell(6).GetValue<int>(),
                            TipoParcelaId = fila.Cell(7).GetValue<int>()
                        });
                    }
                    catch (Exception ex)
                    {
                        errores.Add($"Fila {fila.RowNumber()}: {ex.Message}");
                    }
                }
            }

            if (errores.Count > 0)
                return BadRequest(new { mensaje = "Se encontraron errores en el archivo.", errores });

            var cantidad = await _seccionService.ImportarSecciones(secciones);

            return Ok(new { mensaje = $"{cantidad} secciones importadas correctamente." });
        }

        [HttpPost]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> ImportarParcelas(IFormFile excel)
        {
            if (excel == null || excel.Length == 0)
                return BadRequest("Debe adjuntar un archivo Excel.");

            var parcelas = new List<ParcelaDTO>();
            var errores = new List<string>();

            using (var stream = new MemoryStream())
            {
                await excel.CopyToAsync(stream);
                using var wb = new XLWorkbook(stream);
                var ws = wb.Worksheet(1);
                var filas = ws.RangeUsed()!.RowsUsed().Skip(1); // saltar encabezado

                foreach (var fila in filas)
                {
                    try
                    {
                        parcelas.Add(new ParcelaDTO
                        {
                            Id = fila.Cell(1).GetValue<int>(),
                            Visibilidad = fila.Cell(2).GetValue<int>() == 1,
                            NroParcela = fila.Cell(3).GetValue<int>(),
                            NroFila = fila.Cell(4).GetValue<int>(),
                            CantidadDifuntos = fila.Cell(5).GetValue<int>(),
                            NombrePanteon = fila.Cell(6).GetString(),
                            InformacionAdicional = fila.Cell(7).GetString(),
                            SeccionId = fila.Cell(8).GetValue<int>(),
                            TipoNichoId = fila.Cell(9).GetValue<int>(),
                            TipoPanteonId = fila.Cell(10).GetValue<int>(),
                            TipoParcelaId = fila.Cell(11).GetValue<int>()
                        });
                    }
                    catch (Exception ex)
                    {
                        errores.Add($"Fila {fila.RowNumber()}: {ex.Message}");
                    }
                }
            }

            if (errores.Count > 0)
                return BadRequest(new { mensaje = "Se encontraron errores en el archivo.", errores });

            var cantidad = await _parcelaService.ImportarParcelas(parcelas);

            return Ok(new { mensaje = $"{cantidad} parcelas importadas correctamente." });
        }
    }
}
