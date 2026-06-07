using CemSys3.DTOs.Archivo;
using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.PDF;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers;
using CemSys3.Helpers.Enumerable;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.PDF;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Archivo;
using CemSys3.Interfaces.Concesion;
using CemSys3.Interfaces.HistorialEstados;
using CemSys3.Interfaces.PDF;
using CemSys3.ViewModels.Concesion;
using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class ConcesionController : Controller
    {
        public readonly IConcesion _concesionService;
        private readonly IViewRenderService _viewRenderService;
        private readonly PlaywrightPdfGenerator _pdfGenerator;
        private readonly IArchivo _archivoService;
        private readonly IHistorialEstados _historialEstadosService;
        private readonly IWebHostEnvironment _env;
        private readonly IDeudaConcesion _deudaConcesionService;

        public ConcesionController(IConcesion concesion, IViewRenderService render, PlaywrightPdfGenerator pdfGenerator, IArchivo archivo,
            IHistorialEstados historialEstados, IWebHostEnvironment env, IDeudaConcesion deudaConcesionService)
        {
            _concesionService = concesion;
            _viewRenderService = render;
            _pdfGenerator = pdfGenerator;
            _archivoService = archivo;
            _historialEstadosService = historialEstados;
            _env = env;
            _deudaConcesionService = deudaConcesionService;
        }

        //tabla general de concesiones, con paginacion y filtro por estado
        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)] 
        public async Task<IActionResult> TablaGeneral(int filtroEstado = 0, string nombre = "",
            string apellido = "", int concesion = 0, int? tipoParcelaID = null, int? seccionID = null, int? parcelaID = null, DateOnly? fechaDesde = null, DateOnly? fechaHasta = null, int pagina = 1, int porPagina = 10)
        {
            TablaGeneralVM viewModel = new TablaGeneralVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();

            try
            {
                PaginadoResponse<TablaConcesionDTO> resultado = await _concesionService.GellAllPaginado(filtroEstado, pagina, porPagina, nombre, apellido, concesion, tipoParcelaID, seccionID, parcelaID, fechaDesde, fechaHasta);
                viewModel.Listado = resultado.Items;
                viewModel.Paginacion = resultado.Paginacion;

                viewModel.Paginacion.Parametros = new Dictionary<string, string>();

                viewModel.Paginacion.Parametros.Add("filtroEstado", filtroEstado.ToString());
                viewModel.Paginacion.Parametros.Add("porPagina", porPagina.ToString());
                viewModel.Paginacion.Parametros.Add("nombre", nombre);
                viewModel.Paginacion.Parametros.Add("apellido", apellido);
                viewModel.Paginacion.Parametros.Add("concesion", concesion.ToString("D5"));
                viewModel.Paginacion.Parametros.Add("tipoParcelaID", viewModel.TipoParcelaID?.ToString() ?? "");
                viewModel.Paginacion.Parametros.Add("seccionID", viewModel.SeccionID?.ToString() ?? "");
                viewModel.Paginacion.Parametros.Add("parcelaID", viewModel.ParcelaID?.ToString() ?? "");
                viewModel.Paginacion.Parametros.Add("fechaDesde", fechaDesde?.ToString("yyyy-MM-dd") ?? "");
                viewModel.Paginacion.Parametros.Add("fechaHasta", fechaHasta?.ToString("yyyy-MM-dd") ?? "");

                viewModel.Concesion = concesion;
                viewModel.Nombre = nombre;
                viewModel.Apellido = apellido;
                viewModel.TipoParcelaID = tipoParcelaID;
                viewModel.SeccionID = seccionID;
                viewModel.ParcelaID = parcelaID;
                viewModel.FechaDesde = fechaDesde;
                viewModel.FechaHasta = fechaHasta;

                if (viewModel.Listado.Count() == 0)
                {
                    viewModel.SweetAlert = new SweetAlertDTO
                    {
                        Titulo = "Advertencia",
                        Mensaje = "No se encontraton resultados",
                        Tipo = "warning"
                    };
                }
            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al cargar las concesiones: {ex.Message}",
                    Tipo = "error"
                };
            }

            return View(viewModel);
        }

        //vista para hacer el contrato de conesion, por primera vez o para renovacion, dependiendo del estado del tramite
        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> GenerarContrato(int idTramite, bool EsRenovacion)
        {
            GenerarContratoVM viewModel = new GenerarContratoVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();

            try
            {
                viewModel.contrato = await _concesionService.SolicitarDatosParaGenerarContrato(idTramite);
                viewModel.contrato.EsRenovacion = EsRenovacion;

                if (viewModel.contrato.TipoParcela == "Nicho")
                {
                    viewModel.CalcularPrecioNichoUnAnio();
                }
            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al cargar los datos del contrato: {ex.Message}",
                    Tipo = "error"
                };
            }

            return View(viewModel);
        }

        [HttpPost]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> GenerarContrato(GenerarContratoVM viewModel)
        {
            GenerarContratoDTO contratoGenerado = DatosConcesionParaPDF(viewModel);

            //si es nicho voy a vista de contrato nicho sino contrato fosa
            string nombreVistaContrato = viewModel.contrato.TipoParcela ?? "";

            try
            {
                string html = await _viewRenderService.RenderToStringAsync($"Concesion/{nombreVistaContrato}", contratoGenerado);

                var pdfBytes = await GenerarPdfContrato(contratoGenerado, nombreVistaContrato);
                return File(pdfBytes, "application/pdf");
            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al cargar los datos del contrato: {ex.Message}",
                    Tipo = "error"
                };

                return View(viewModel);
            }

        }

        private GenerarContratoDTO DatosConcesionParaPDF(GenerarContratoVM viewModel)
        {
            GenerarContratoDTO contratoGenerado = new GenerarContratoDTO
            {
                TramiteId = viewModel.contrato.TramiteId,
                EstadoTramiteId = viewModel.contrato.EstadoTramiteId,
                ParcelaId = viewModel.contrato.ParcelaId,
                TipoParcela = viewModel.contrato.TipoParcela,
                SeccionId = viewModel.contrato.SeccionId,
                NombreSeccion = viewModel.contrato.NombreSeccion,
                NroParcela = viewModel.contrato.NroParcela,
                NroFila = viewModel.contrato.NroFila,
                NroConcesion = viewModel.contrato.NroConcesion,
                Difuntos = viewModel.contrato.Difuntos,
                Titulares = viewModel.contrato.Titulares,
                PrecioEnLetras = NumeroALetras.ConvertirALetras(viewModel.PrecioFinal),
                formaPago = viewModel.FormaDePago ?? "",
                CuotaId = viewModel.CantidadCuotaSeleccionada,
                Precio = viewModel.PrecioFinal,
                OtraFormaPago = viewModel.otraFormaPago,
                CantidadAniosId = viewModel.CantidadAniosId.Value,
                Vencimiento = viewModel.Vencimiento.Value,
                fechaGeneracion = DateTime.Now,
                EsRenovacion = viewModel.contrato.EsRenovacion,
                LogoBase64 = ObtenerImagenBase64("logoMuni.png"),
                PieBase64 = ObtenerImagenBase64("pieContrato.png")
            };

            var ruta = Path.Combine(_env.WebRootPath, "config", "intendente.txt");

            string intendente = System.IO.File.Exists(ruta)
                ? System.IO.File.ReadAllText(ruta)
                : "-----";
            contratoGenerado.NombreIntendente = intendente;
            
            return contratoGenerado;
        }

        [HttpPost]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> ValidarContrato(GenerarContratoVM viewModel)
        {
            ModelState.Remove("contrato.NroConcesion");

            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Los datos ingresados no son válidos."
                });
            }

            return Json(new { success = true });
        }

        //btn para pasar a la pantalla de contrato. Guarda en BD.
        [HttpPost]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> ContratoFirmado(GenerarContratoVM viewModel)
        {

            if (!ModelState.IsValid)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Los datos ingresados no son válidos. Por favor, revise el formulario.",
                    Tipo = "error"
                };
                return View(viewModel);
            }

            //todos los datos se deben guardar al precionar en contrato firmado, no antes, para evitar que se guarden datos erroneos en la base de datos.
            ConcesionDTO dto = new ConcesionDTO();
            dto.TramiteId = viewModel.contrato.TramiteId;
            dto.Concesion = viewModel.contrato.NroConcesion;
            dto.Vencimiento = viewModel.Vencimiento;
            dto.Precio = viewModel.PrecioFinal;
            dto.Visibilidad = true;
            dto.CantidadAniosId = viewModel.CantidadAniosId;
            dto.CuotaId = viewModel.CantidadCuotaSeleccionada;
            dto.UsuarioId = HttpContext.Session.GetInt32("IdUsuario") ?? 0;
            dto.EstadoTramiteId = viewModel.contrato.EstadoTramiteId;
            dto.TipoParcela = viewModel.contrato.TipoParcela;
            dto.ParcelaId = viewModel.contrato.ParcelaId;
            dto.EstadoTramiteId = (int)EstadosConcesionEnum.Vigente;
            dto.MensajeParcela = $"\n● El {DateTime.Now.ToString("dd/MM/yyyy")} se realizó contrato de concesión ({viewModel.contrato.NroConcesion?.ToString("D5") ?? "-----"}) por {EnumHelper.GetDisplayNameByValue<AniosConcesionEnum>(viewModel.CantidadAniosId.Value)}. Vencimiento {viewModel.Vencimiento}.";
            dto.InformacionAdicional = $"\n● El {DateTime.Now.ToString("dd/MM/yyyy")} se realizó contrato de concesión ({viewModel.contrato.NroConcesion?.ToString("D5") ?? "-----"}) por {EnumHelper.GetDisplayNameByValue<AniosConcesionEnum>(viewModel.CantidadAniosId.Value)}. Vencimiento {viewModel.Vencimiento}.";

            //pasar de TitularesDTO a PersonaDTO
            List<PersonaDTO> titulares = new List<PersonaDTO>();
            foreach (var titular in viewModel.contrato.Titulares)
            {
                PersonaDTO persona = new PersonaDTO
                {
                    Id = titular.Id ?? 0,
                    Nombre = titular.Nombre,
                    Apellido = titular.Apellido,
                    Dni = titular.Dni,
                    Domicilio = titular.Domicilio,
                    Celular = titular.Celular,
                    Sexo = titular.Sexo,
                    Correo = titular.CorreoElectronico,
                    Visibilidad = true,
                    CategoriaPersonaId = (int)CategoriaPersonaEnum.Titular
                };
                titulares.Add(persona);
            }

            dto.Titulares = titulares;
            dto.Difuntos = viewModel.contrato.Difuntos;

            // 1. Armar nuevamente el DTO del contrato (igual que en GenerarContrato) para genera el PDF sin mostrarlo
            GenerarContratoDTO contratoGenerado = DatosConcesionParaPDF(viewModel);

            string nombreVistaContrato = viewModel.contrato.TipoParcela ?? "";

            //Update al contrato con los datos
            GenericResultDTO resultado = new GenericResultDTO();
            try
            {
                resultado = await _concesionService.Update(dto);
            }
            catch (Exception ex)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al guardar el contrato: {ex.Message}",
                    Tipo = "error"
                });

                return RedirectToAction("GenerarContrato", new { idTramite = contratoGenerado.TramiteId });
            }


            if (resultado.Success == false)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al guardar el contrato.",
                    Tipo = "error"
                });

                return RedirectToAction("GenerarContrato", new { idTramite = contratoGenerado.TramiteId });
            }

            

            // 2. Generar PDF
            var pdfBytes = await GenerarPdfContrato(contratoGenerado, nombreVistaContrato);

            // 3. Crear ArchivoDTO
            ArchivoDTO archivoDto = new ArchivoDTO
            {
                TramiteId = viewModel.contrato.TramiteId,
                CategoriaArchivo = ((int)CategoriaArchivosEnum.Contrato_Concesion).ToString(),
                NombreArchivo = $"Contrato_{viewModel.contrato.NroConcesion?.ToString("D5")}_{DateTime.Now.Year.ToString()}.pdf",
                MimeType = "application/pdf",
                Descripcion = $"Contrato concesión {viewModel.contrato.NroConcesion?.ToString("D5")} sin firmar",
                Contenido = pdfBytes
            };

            try
            {
                // 4. Guardar
                await _archivoService.AddDesdeBytes(archivoDto);

                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Éxito",
                    Mensaje = $"Concesión {contratoGenerado.NroConcesion?.ToString("D5")} realizada correctamente",
                    Tipo = "success"
                });

                return RedirectToAction("Concesion", new { tramiteId = contratoGenerado.TramiteId });
            }
            catch (Exception ex)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al guardar el contrato: {ex.Message}",
                    Tipo = "error"
                });

                return RedirectToAction("GenerarContrato", new { idTramite = contratoGenerado.TramiteId });
            }

        }

        //vista de menu
        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> Concesion(int tramiteId)
        {
            ConcesionVM viewModel = new ConcesionVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();

            try
            {
                viewModel.Dto = await _concesionService.InfoGeneral(tramiteId);
                viewModel.historial = await _historialEstadosService.GetAllById(tramiteId);

            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al cargar los datos de la concesión: {ex.Message}",
                    Tipo = "error"
                };
            }
            return View(viewModel);
        }

        //vista de menu
        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> ArchivosConcesion(int tramiteId)
        {
            ArchivosConcesionVM viewModel = new ArchivosConcesionVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();

            try
            {
                viewModel.TramiteId = tramiteId;
                viewModel.Dto = await _concesionService.InfoGeneralMinima(tramiteId);
                viewModel.Archivos = await _archivoService.GetAllByTramiteId(tramiteId);
            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al cargar los archivos de la concesión: {ex.Message}",
                    Tipo = "error"
                };
            }
            return View(viewModel);
        }

        //vista de menu
        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> ModificarConcesion(int tramiteId)
        {
            ModificarConcesionVM viewModel = new();
            viewModel.SweetAlert = TempData.GetSweetAlert();

            try
            {
                viewModel.Dto = await _concesionService.ModificarDatosConecesion(tramiteId);
                viewModel.DtoInfo = await _concesionService.InfoGeneralMinima(tramiteId);

            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al cargar los datos de la concesión: {ex.Message}",
                    Tipo = "error"
                };
            }
            return View(viewModel);
        }

        //vista de menu
        [HttpPost]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> ModificarConcesion(ModificarConcesionVM viewModel)
        {
            ModificarDatosConcesionDTO dto = new ModificarDatosConcesionDTO();
            //pasar de TitularesDTO a PersonaDTO
            List<PersonaDTO> titulares = new List<PersonaDTO>();
            foreach (var titular in viewModel.Dto.Titulares)
            {
                PersonaDTO persona = new PersonaDTO
                {
                    Id = titular.Id ?? 0,
                    Nombre = titular.Nombre,
                    Apellido = titular.Apellido,
                    Dni = titular.Dni,
                    Domicilio = titular.Domicilio,
                    Celular = titular.Celular,
                    Sexo = titular.Sexo,
                    Correo = titular.CorreoElectronico,
                    Visibilidad = true,
                    CategoriaPersonaId = (int)CategoriaPersonaEnum.Titular
                };

                titulares.Add(persona);
            }

            dto.Vencimiento = viewModel.Dto.Vencimiento;
            dto.NroConcesion = viewModel.Dto.NroConcesion;
            dto.TramiteId = viewModel.Dto.TramiteId;
            dto.TitularesPost = titulares;

            try
            {
                await _concesionService.ModificarDatosConecesion(dto);
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Éxito",
                    Mensaje = $"Concesión actualizada correctamente",
                    Tipo = "success"
                });
            }
            catch (Exception ex) {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al enviar los datos de la concesión: {ex.Message}",
                    Tipo = "error"
                };
                return View(viewModel);

            }

            return RedirectToAction("ModificarConcesion", new {tramiteId = viewModel.Dto.TramiteId});
        }

        //vista de menu
        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> HistorialConcesion(int tramiteId)
        {
            HistorialConcesionVM viewModel = new HistorialConcesionVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();
            try
            {
                viewModel.TramiteId = tramiteId;
                viewModel.Titulares = await _historialEstadosService.HistorialTitulares(tramiteId);
                viewModel.Tramites = await _historialEstadosService.HistorialTramitesConcesion(tramiteId);
                viewModel.Dto = await _concesionService.InfoGeneralMinima(tramiteId);

            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al cargar el historial de la concesión: {ex.Message}",
                    Tipo = "error"
                };
            }
            return View(viewModel);
        }

        //vista de menu
        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> CalcularDeuda(int tramiteId)
        {
            CalculoDeudaVM viewModel = new CalculoDeudaVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();
            viewModel.TramiteId = tramiteId;
            try
            {
                viewModel.MensajeDeuda = await _deudaConcesionService.CalculoDeudaConcesion(tramiteId);
                viewModel.Dto = await _concesionService.InfoGeneralMinima(tramiteId);

            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al calcular la deuda de la concesión: {ex.Message}",
                    Tipo = "error"
                };
            }

            return View(viewModel);
        }




        // ─── Exportar Excel ────────────────────────────────────────────────────
        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> ExportarExcel(
            int filtroEstado = 0, string nombre = "", string apellido = "",
            int concesion = 0, int? tipoParcelaID = null, int? seccionID = null,
            int? parcelaID = null, DateOnly? fechaDesde = null, DateOnly? fechaHasta = null)
        {
            var datos = await _concesionService.GetAllParaExportar(
                filtroEstado, nombre, apellido, concesion,
                tipoParcelaID, seccionID, parcelaID, fechaDesde, fechaHasta);

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Concesiones");

            // Encabezados
            string[] headers = { "Concesión", "Difuntos", "Sección", "Parcela", "Titular/es", "Celular", "Correo", "Vencimiento", "Estado" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#2E75B6");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            // Datos
            int fila = 2;
            foreach (var item in datos)
            {
                string parcela = item.TipoParcelaId switch
                {
                    (int)TipoParcelaEnum.Nicho => $"Nicho {item.NroParcela} Fila {item.NroFila}",
                    (int)TipoParcelaEnum.Fosa => $"Fosa {item.NroParcela}",
                    (int)TipoParcelaEnum.Panteon => $"Lote {item.NroParcela}",
                    _ => item.NroParcela.ToString() ?? ""
                };

                string difuntos = item.Difuntos?.Any() == true
                    ? string.Join(" / ", item.Difuntos.Select(d => $"{d.Apellido.ToUpper()}, {d.Nombre.ToUpper()}"))
                    : "---";
                string titulares = item.Titulares?.Any() == true
                    ? string.Join(" / ", item.Titulares.Select(t => $"{t.Apellido.ToUpper()}, {t.Nombre.ToUpper()}"))
                    : "---";
                string estado = EnumHelper.GetDisplayNameByValue<EstadosConcesionEnum>(item.EstadoTramiteId);

                ws.Cell(fila, 1).Value = item.Concesion?.ToString("D5") ?? "---";
                ws.Cell(fila, 2).Value = difuntos;
                ws.Cell(fila, 3).Value = item.NombreSeccion?.ToUpper() ?? "";
                ws.Cell(fila, 4).Value = parcela;
                ws.Cell(fila, 5).Value = titulares;
                // ── nuevas ──
                ws.Cell(fila, 6).Value = item.Titulares?.Any() == true
                    ? string.Join(" / ", item.Titulares.Select(t => t.celular).Where(c => !string.IsNullOrWhiteSpace(c)))
                    : "";
                ws.Cell(fila, 7).Value = item.Titulares?.Any() == true
                    ? string.Join(" / ", item.Titulares.Select(t => t.correo).Where(c => !string.IsNullOrWhiteSpace(c)))
                    : "";
                // ────────────
                ws.Cell(fila, 8).Value = item.Vencimiento?.ToString("dd/MM/yyyy") ?? "";
                ws.Cell(fila, 9).Value = estado;

                // Color de fila según estado
                DateOnly hoy = DateOnly.FromDateTime(DateTime.Today);
                XLColor? color = null;
                if (item.EstadoTramiteId == (int)EstadosConcesionEnum.Vencido)
                    color = XLColor.FromHtml("#F4CCCC");
                else if (item.Vencimiento.HasValue && item.Vencimiento.Value.Year == hoy.Year && item.Vencimiento.Value >= hoy)
                    color = XLColor.FromHtml("#FFF2CC");

                if (color != null)
                    ws.Row(fila).Style.Fill.BackgroundColor = color;

                fila++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            string fileName = $"Concesiones_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        // ─── Exportar Word ─────────────────────────────────────────────────────
        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> ExportarWord(
            int filtroEstado = 0, string nombre = "", string apellido = "",
            int concesion = 0, int? tipoParcelaID = null, int? seccionID = null,
            int? parcelaID = null, DateOnly? fechaDesde = null, DateOnly? fechaHasta = null)
        {
            var datos = await _concesionService.GetAllParaExportar(
                filtroEstado, nombre, apellido, concesion,
                tipoParcelaID, seccionID, parcelaID, fechaDesde, fechaHasta);

            using var stream = new MemoryStream();
            using (var doc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document, true))
            {
                var mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document(new Body());
                var body = mainPart.Document.Body!;

                // ── Estilos básicos ──────────────────────────────────────────
                var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
                stylesPart.Styles = new Styles();

                // Título
                body.AppendChild(new Paragraph(
                    new ParagraphProperties(
                        new Justification { Val = JustificationValues.Center },
                        new SpacingBetweenLines { After = "200" }),
                    new Run(
                        new RunProperties(
                            new Bold(),
                            new FontSize { Val = "36" },
                            new Color { Val = "2E75B6" }),
                        new Text("Tabla de Concesiones"))));

                body.AppendChild(new Paragraph(
                    new Run(new Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}"))));
                body.AppendChild(new Paragraph(new Run(new Text(""))));

                // ── Tabla ────────────────────────────────────────────────────
                string[] headers2 = { "Concesión", "Difuntos", "Sección", "Parcela", "Titular/es", "Vencimiento", "Estado" };
                // Anchos en DXA (total ~9360 para A4 con márgenes de 1")
                int[] widths = { 900, 1800, 1200, 1400, 1800, 1100, 1160 };

                var table = new Table();

                // Propiedades de tabla
                table.AppendChild(new TableProperties(
                    new TableWidth { Width = "9360", Type = TableWidthUnitValues.Dxa },
                    new TableBorders(
                        new TopBorder { Val = BorderValues.Single, Size = 4, Color = "CCCCCC" },
                        new BottomBorder { Val = BorderValues.Single, Size = 4, Color = "CCCCCC" },
                        new LeftBorder { Val = BorderValues.Single, Size = 4, Color = "CCCCCC" },
                        new RightBorder { Val = BorderValues.Single, Size = 4, Color = "CCCCCC" },
                        new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4, Color = "CCCCCC" },
                        new InsideVerticalBorder { Val = BorderValues.Single, Size = 4, Color = "CCCCCC" })));

                // Fila de encabezado
                var headerRow = new TableRow();
                for (int i = 0; i < headers2.Length; i++)
                {
                    headerRow.AppendChild(CreateWordCell(
                        headers2[i], widths[i], bold: true,
                        bgColor: "2E75B6", fontColor: "FFFFFF"));
                }
                table.AppendChild(headerRow);

                // Filas de datos
                bool alternar = false;
                DateOnly hoy2 = DateOnly.FromDateTime(DateTime.Today);
                foreach (var item in datos)
                {
                    string parcela = item.TipoParcelaId switch
                    {
                        (int)TipoParcelaEnum.Nicho => $"Nicho {item.NroParcela} Fila {item.NroFila}",
                        (int)TipoParcelaEnum.Fosa => $"Fosa {item.NroParcela}",
                        (int)TipoParcelaEnum.Panteon => $"Lote {item.NroParcela}",
                        _ => item.NroParcela.ToString() ?? ""
                    };

                    string difuntos = item.Difuntos?.Any() == true
                        ? string.Join(", ", item.Difuntos.Select(d => $"{d.Apellido.ToUpper()}, {d.Nombre.ToUpper()}"))
                        : "---";
                    string titulares = item.Titulares?.Any() == true
                        ? string.Join(", ", item.Titulares.Select(t => $"{t.Apellido.ToUpper()}, {t.Nombre.ToUpper()}"))
                        : "---";
                    string estado = EnumHelper.GetDisplayNameByValue<EstadosConcesionEnum>(item.EstadoTramiteId);

                    string bg = "FFFFFF";
                    if (item.EstadoTramiteId == (int)EstadosConcesionEnum.Vencido)
                        bg = "F4CCCC";
                    else if (item.Vencimiento.HasValue && item.Vencimiento.Value.Year == hoy2.Year && item.Vencimiento.Value >= hoy2)
                        bg = "FFF2CC";
                    else if (alternar)
                        bg = "F5F5F5";

                    var row = new TableRow();
                    string[] valores = {
                item.Concesion?.ToString("D5") ?? "---",
                difuntos,
                item.NombreSeccion?.ToUpper() ?? "",
                parcela,
                titulares,
                item.Vencimiento?.ToString("dd/MM/yyyy") ?? "",
                estado
            };

                    for (int i = 0; i < valores.Length; i++)
                        row.AppendChild(CreateWordCell(valores[i], widths[i], bgColor: bg));

                    table.AppendChild(row);
                    alternar = !alternar;
                }

                body.AppendChild(table);

                // Pie con total
                body.AppendChild(new Paragraph(new Run(new Text(""))));
                body.AppendChild(new Paragraph(
                    new Run(new Text($"Total de registros: {datos.Count}"))));

                mainPart.Document.Save();
            }

            string fileNameWord = $"Concesiones_{DateTime.Now:yyyyMMdd_HHmm}.docx";
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                fileNameWord);
        }

        // ─── Helper para celdas Word ───────────────────────────────────────────
        private static TableCell CreateWordCell(
            string text, int widthDxa,
            bool bold = false, string bgColor = "FFFFFF", string fontColor = "000000")
        {
            var runProps = new RunProperties(
                new Color { Val = fontColor },
                new FontSize { Val = "18" });  // 9pt
            if (bold) runProps.AppendChild(new Bold());

            var cell = new TableCell(
                new TableCellProperties(
                    new TableCellWidth { Width = widthDxa.ToString(), Type = TableWidthUnitValues.Dxa },
                    new Shading { Val = ShadingPatternValues.Clear, Fill = bgColor },
                    new TableCellMargin(
                        new LeftMargin { Width = "100", Type = TableWidthUnitValues.Dxa },
                        new RightMargin { Width = "100", Type = TableWidthUnitValues.Dxa })),
                new Paragraph(
                    new ParagraphProperties(
                        new SpacingBetweenLines { Before = "60", After = "60" }),
                    new Run(runProps, new Text(text))));

            return cell;
        }








        private async Task<byte[]> GenerarPdfContrato(GenerarContratoDTO contratoGenerado, string nombreVistaContrato)
        {
            string html = await _viewRenderService.RenderToStringAsync(
                $"Concesion/{nombreVistaContrato}", contratoGenerado);

            var pdfBytes = await _pdfGenerator.GenerateFromHtmlAsync(
                html,
                new PdfOptionsDto
                {
                    Landscape = false,
                    MarginTop = "20px",
                    MarginLeft = "30px"
                });

            return pdfBytes;
        }

        private string ObtenerImagenBase64(string archivo)
        {
            var ruta = Path.Combine(_env.WebRootPath, "fotos", archivo);

            if (!System.IO.File.Exists(ruta))
                return "";

            var bytes = System.IO.File.ReadAllBytes(ruta);

            var base64 = Convert.ToBase64String(bytes);

            var extension = Path.GetExtension(ruta).ToLower();

            var mime = extension switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".svg" => "image/svg+xml",
                _ => "application/octet-stream"
            };

            return $"data:{mime};base64,{base64}";
        }
    }
}
