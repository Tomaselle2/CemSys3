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
using CemSys3.Interfaces.PlantillaTramite;
using CemSys3.Interfaces.TramitesConcesion;
using CemSys3.ViewModels.Concesion;
using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Mvc;
using A = DocumentFormat.OpenXml.Drawing;
using Pic = DocumentFormat.OpenXml.Drawing.Pictures;
using Wp = DocumentFormat.OpenXml.Drawing.Wordprocessing;

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
        private readonly ITemplateProcessor _processor;
        private readonly IRequisitos _requisitos;

        public ConcesionController(IConcesion concesion, IViewRenderService render, PlaywrightPdfGenerator pdfGenerator, IArchivo archivo,
            IHistorialEstados historialEstados, IWebHostEnvironment env, IDeudaConcesion deudaConcesionService, ITemplateProcessor processor, IRequisitos requisitos)
        {
            _concesionService = concesion;
            _viewRenderService = render;
            _pdfGenerator = pdfGenerator;
            _archivoService = archivo;
            _historialEstadosService = historialEstados;
            _env = env;
            _deudaConcesionService = deudaConcesionService;
            _processor = processor;
            _requisitos = requisitos;
        }

        //tabla general de concesiones, con paginacion y filtro por estado
        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)] 
        public async Task<IActionResult> TablaGeneral(int filtroEstado = 0, string nombre = "",
            string apellido = "",string nombrePanteon = "", int concesion = 0, int? tipoParcelaID = null, int? seccionID = null, int? parcelaID = null, DateOnly? fechaDesde = null, DateOnly? fechaHasta = null, int pagina = 1, int porPagina = 10)
        {
            TablaGeneralVM viewModel = new TablaGeneralVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();

            try
            {
                PaginadoResponse<TablaConcesionDTO> resultado = await _concesionService.GellAllPaginado(filtroEstado, pagina, porPagina, nombre, apellido, nombrePanteon, concesion, tipoParcelaID, seccionID, parcelaID, fechaDesde, fechaHasta);
                viewModel.Listado = resultado.Items;
                viewModel.Paginacion = resultado.Paginacion;

                viewModel.Paginacion.Parametros = new Dictionary<string, string>();


                viewModel.Concesion = concesion;
                viewModel.Nombre = nombre;
                viewModel.Apellido = apellido;
                viewModel.NombrePanteon = nombrePanteon;
                viewModel.TipoParcelaID = tipoParcelaID;
                viewModel.SeccionID = seccionID;
                viewModel.ParcelaID = parcelaID;
                viewModel.FechaDesde = fechaDesde;
                viewModel.FechaHasta = fechaHasta;


                viewModel.Paginacion.Parametros.Add("filtroEstado", filtroEstado.ToString());
                viewModel.Paginacion.Parametros.Add("porPagina", porPagina.ToString());
                viewModel.Paginacion.Parametros.Add("nombre", nombre);
                viewModel.Paginacion.Parametros.Add("apellido", apellido);
                viewModel.Paginacion.Parametros.Add("nombrePanteon", nombrePanteon);
                viewModel.Paginacion.Parametros.Add("concesion", concesion.ToString("D5"));
                viewModel.Paginacion.Parametros.Add("tipoParcelaID", viewModel.TipoParcelaID?.ToString() ?? "");
                viewModel.Paginacion.Parametros.Add("seccionID", viewModel.SeccionID?.ToString() ?? "");
                viewModel.Paginacion.Parametros.Add("parcelaID", viewModel.ParcelaID?.ToString() ?? "");
                viewModel.Paginacion.Parametros.Add("fechaDesde", fechaDesde?.ToString("yyyy-MM-dd") ?? "");
                viewModel.Paginacion.Parametros.Add("fechaHasta", fechaHasta?.ToString("yyyy-MM-dd") ?? "");

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
                    viewModel.AplicarDescuentoUrnario();   // 1º: descuento urnario si corresponde
                    viewModel.CalcularPrecioNichoUnAnio(); // 2º: cálculo especial de 1 año
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
                PieBase64 = ObtenerImagenBase64("pieContrato.png"),
                IgnorarFechaIngreso = viewModel.contrato.IgnorarFechaIngreso,
                TipoNichoId = viewModel.contrato.TipoNichoId
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

            // Si es modo manual, el select de precio automático no aplica
            var modoPrecio = Request.Form["ModoPrecio"].ToString();
            if (modoPrecio.Contains("manual"))
            {
                ModelState.Remove("PrecioSeleccionado");
            }


            if (!ModelState.IsValid)
            {
                var errores = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .Select(x => $"{x.Key}: {string.Join(", ", x.Value!.Errors.Select(e => e.ErrorMessage))}");

                return Json(new
                {
                    success = false,
                    message = string.Join(" | ", errores) // temporal para debug
                });
            }

            return Json(new { success = true });
        }

        //btn para pasar a la pantalla de contrato. Guarda en BD.
        [HttpPost]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> ContratoFirmado(GenerarContratoVM viewModel)
        {
            var modoPrecio = Request.Form["ModoPrecio"].ToString();
            if (modoPrecio.Contains("manual") || viewModel.CantidadAniosId.HasValue)
            {
                ModelState.Remove("PrecioSeleccionado");
            }

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
            string descripcionAnios = viewModel.CantidadAniosId.HasValue
            ? EnumHelper.GetDisplayNameByValue<AniosConcesionEnum>(viewModel.CantidadAniosId.Value)
            : "cantidad no especificada";

            dto.MensajeParcela = $"\n● El {DateTime.Now:dd/MM/yyyy} se realizó contrato de concesión ({viewModel.contrato.NroConcesion?.ToString("D5") ?? "-----"}) por {descripcionAnios}. Vencimiento {viewModel.Vencimiento}.";
            dto.InformacionAdicional = $"\n● El {DateTime.Now:dd/MM/yyyy} se realizó contrato de concesión ({viewModel.contrato.NroConcesion?.ToString("D5") ?? "-----"}) por {descripcionAnios}. Vencimiento {viewModel.Vencimiento}.";
            
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

            if (viewModel.Dto.Titulares != null && viewModel.Dto.Titulares.Count() > 0)
            {
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
            }
            

            dto.Vencimiento = viewModel.Dto.Vencimiento;
            dto.NroConcesion = viewModel.Dto.NroConcesion;
            dto.TramiteId = viewModel.Dto.TramiteId;
            dto.TitularesPost = titulares;
            dto.FechaInicio = viewModel.Dto.FechaInicio;

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
                viewModel.Difuntos = await _historialEstadosService.DifuntosEnConcesion(tramiteId);

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

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> TrasladarDifuntoManualmente(int tramiteId,int difuntoId, int parcelaNuevaId, int parcelaAntiguaId, int concesionNuevaId, int conesionAntiguaId, DateTime? fechaInicio)
        {
            try
            {
                if (difuntoId <= 0)
                {
                    TempData.SetSweetAlert(new SweetAlertDTO
                    {
                        Titulo = "Error",
                        Mensaje = $"ID de difunto no válido.",
                        Tipo = "error"
                    });

                    return RedirectToAction("ModificarConcesion", new {tramiteId = tramiteId} );
                }

                if (fechaInicio == null)
                {
                    TempData.SetSweetAlert(new SweetAlertDTO
                    {
                        Titulo = "Error",
                        Mensaje = $"Fecha de ingreso no válida.",
                        Tipo = "error"
                    });

                    return RedirectToAction("ModificarConcesion", new { tramiteId = tramiteId });
                }

                await _concesionService.TrasladarDifuntoManualmente(difuntoId, parcelaNuevaId, parcelaAntiguaId, concesionNuevaId, conesionAntiguaId, fechaInicio);

                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Éxito",
                    Mensaje = $"Difunto trasladado correctamente.",
                    Tipo = "success"
                });
            }
            catch(Exception ex)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al trasladar el difunto: {ex.Message}",
                    Tipo = "error"
                });

                return RedirectToAction("ModificarConcesion", new { tramiteId = tramiteId });

            }

            return RedirectToAction("ModificarConcesion", new { tramiteId = tramiteId });
        }


        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> CaducarConcesion(int concesionId)
        {
            if(concesionId == 0)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"ID de concesión no válido.",
                    Tipo = "error"
                });

                return RedirectToAction("TablaGeneral");
            }

            try
            {
                await _concesionService.CaducarConcesion(concesionId);

                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Éxito",
                    Mensaje = $"Concesión caducada correctamente.",
                    Tipo = "success"
                });
                return RedirectToAction("ModificarConcesion", new { tramiteId = concesionId });

            }
            catch (Exception ex)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"Ocurrió un error al caducar la concesión: {ex.Message}",
                    Tipo = "error"
                });
                return RedirectToAction("ModificarConcesion", new { tramiteId = concesionId });
            }
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

            var textoRequisito = await _requisitos.GetByTipoTramiteId((int)TipoTramiteEnum.WordConcesiones);


            


            using var stream = new MemoryStream();
            using (var doc = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document, true))
            {
                var mainPart = doc.AddMainDocumentPart();
                mainPart.Document = new Document(new Body());
                var body = mainPart.Document.Body!;

                // ── Estilos básicos ──────────────────────────────────────────
                var stylesPart = mainPart.AddNewPart<StyleDefinitionsPart>();
                stylesPart.Styles = new Styles();

                // ── Cargar imagen UNA sola vez ───────────────────────────────────
                string rutaImagen = Path.Combine(_env.WebRootPath, "Fotos", "logoMuni.png"); // ajustá el path
                byte[] imagenBytes = await System.IO.File.ReadAllBytesAsync(rutaImagen);

                var imagePart = mainPart.AddImagePart(ImagePartType.Png);
                using (var imgStream = new MemoryStream(imagenBytes))
                    imagePart.FeedData(imgStream);

                string rId = mainPart.GetIdOfPart(imagePart);
                // ────────────────────────────────────────────────────────────────

                int contador = 0;
                uint imgId = 1;

                foreach (var item in datos)
                {
                    contador++;

                    // ── Imagen ──────────────────────────────────────────────────────
                    long anchoEmu = 2000000L;
                    long altoEmu = 600000L;

                    var imgParagraph = new Paragraph(
                        new ParagraphProperties(
                            new Justification { Val = JustificationValues.Center }),
                        new Run(
                            new Drawing(
                                new Wp.Inline(
                                    new Wp.Extent { Cx = anchoEmu, Cy = altoEmu },
                                    new Wp.EffectExtent { LeftEdge = 0, TopEdge = 0, RightEdge = 0, BottomEdge = 0 },
                                    new Wp.DocProperties { Id = imgId++, Name = $"Logo{contador}" }, // ← Id único
                                    new A.Graphic(
                                        new A.GraphicData(
                                            new Pic.Picture(
                                                new Pic.NonVisualPictureProperties(
                                                    new Pic.NonVisualDrawingProperties { Id = 0U, Name = "logo.png" },
                                                    new Pic.NonVisualPictureDrawingProperties()),
                                                new Pic.BlipFill(
                                                    new A.Blip { Embed = rId },
                                                    new A.Stretch(new A.FillRectangle())),
                                                new Pic.ShapeProperties(
                                                    new A.Transform2D(
                                                        new A.Offset { X = 0L, Y = 0L },
                                                        new A.Extents { Cx = anchoEmu, Cy = altoEmu }),
                                                    new A.PresetGeometry(new A.AdjustValueList())
                                                    { Preset = A.ShapeTypeValues.Rectangle })))
                                        { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" }))
                                {
                                    DistanceFromTop = 0U,
                                    DistanceFromBottom = 0U,
                                    DistanceFromLeft = 0U,
                                    DistanceFromRight = 0U
                                })));

                    body.AppendChild(imgParagraph);

                    // ── Texto ────────────────────────────────────────────────────────
                    string parcela = item.TipoParcelaId switch
                    {
                        (int)TipoParcelaEnum.Nicho => $"NICHO {item.NroParcela} SECC {item.NombreSeccion.ToUpper()} FILA {item.NroFila}",
                        (int)TipoParcelaEnum.Fosa => $"FOSA {item.NroParcela} SECC {item.NombreSeccion.ToUpper()}",
                        (int)TipoParcelaEnum.Panteon => $"LOTE {item.NroParcela} SECC {item.NombreSeccion.ToUpper()}",
                        _ => item.NroParcela.ToString() ?? ""
                    };

                    string titulares = item.Titulares?.Any() == true
                        ? string.Join(" / ", item.Titulares.Select(t => $"{t.Apellido.ToUpper()}, {t.Nombre.ToUpper()}"))
                        : "---";

                    string sexoReferencia = item.Titulares?.Any() == true
                        ? item.Titulares.First().sexo
                        : "masculino";

                    var variables = new Dictionary<string, string>
    {
        { "Fecha",                 DateTime.Now.ToLongDateString() },
        { "articuloTitularActual", sexoReferencia == "masculino" ? "Sr." : "Sra." },
        { "TitularesActuales",     titulares },
        { "NroConcesion",          item.Concesion?.ToString("D5") ?? "-----" },
        { "Parcela",               parcela },
        { "vencimientoConcesion",  item.Vencimiento?.ToString("dd/MM/yyyy") ?? "--/--/----" }
    };

                    string mensaje = _processor.Procesar(textoRequisito.Descripcion, variables);

                    body.AppendChild(new Paragraph(
                        new Run(
                            new RunProperties(new FontSize { Val = "30" }),
                            new Text(mensaje))));

                    // ── Separador ────────────────────────────────────────────────────
                    bool esUltimo = contador == datos.Count;

                    if (!esUltimo)
                    {
                        if (contador % 3 == 0)
                        {
                            // Cada 3 → nueva página
                            body.AppendChild(new Paragraph(
                                new Run(new Break { Type = BreakValues.Page })));
                        }
                        else
                        {
                            // Dentro de la misma página → espacio
                            body.AppendChild(new Paragraph(
                                new ParagraphProperties(
                                    new SpacingBetweenLines { Before = "400", After = "400" }),
                                new Run(new Text(""))));
                        }
                    }
                }






                // ── Listado de difuntos por sección ─────────────────────────────────────
                // Nueva página para el listado
                body.AppendChild(new Paragraph(
                    new Run(new Break { Type = BreakValues.Page })));

                // Título general del listado
                body.AppendChild(new Paragraph(
                    new ParagraphProperties(
                        new Justification { Val = JustificationValues.Center }),
                    new Run(
                        new RunProperties(new Bold(), new FontSize { Val = "36" }),
                        new Text("LISTADO DE DIFUNTOS POR SECCIÓN"))));

                // Espacio
                body.AppendChild(new Paragraph(new Run(new Text(""))));

                // Agrupar por sección
                var porSeccion = datos
                    .GroupBy(d => string.IsNullOrWhiteSpace(d.NombreSeccion)
                                  ? "SIN SECCIÓN"
                                  : d.NombreSeccion.ToUpper())
                    .OrderBy(g => g.Key);

                foreach (var seccion in porSeccion)
                {
                    // ── Nombre de la sección como encabezado ────────────────────────
                    body.AppendChild(new Paragraph(
                        new ParagraphProperties(
                            new SpacingBetweenLines { Before = "240", After = "80" }),
                        new Run(
                            new RunProperties(new Bold(), new FontSize { Val = "28" }),
                            new Text($"Sección: {seccion.Key}"))));

                    // ── Agrupar por NroParcela dentro de la sección ─────────────────
                    var porParcela = seccion
                        .GroupBy(d => d.NroParcela)
                        .OrderBy(g => g.Key);

                    foreach (var parcelaGroup in porParcela)
                    {
                        var primerItem = parcelaGroup.First();

                        // Descripción de la parcela
                        string tipoParcela = primerItem.TipoParcelaId switch
                        {
                            (int)TipoParcelaEnum.Nicho => $"Nicho {primerItem.NroParcela} - Fila {primerItem.NroFila}",
                            (int)TipoParcelaEnum.Fosa => $"Fosa {primerItem.NroParcela}",
                            (int)TipoParcelaEnum.Panteon => $"Lote {primerItem.NroParcela}",
                            _ => $"Parcela {primerItem.NroParcela}"
                        };

                        // Todos los difuntos de la parcela (varios items pueden compartir parcela)
                        // separados por " / "
                        string difuntos = parcelaGroup
                            .SelectMany(d => d.Difuntos)
                            .Select(p => $"{p.Apellido.ToUpper()}, {p.Nombre.ToUpper()}")
                            .Distinct()
                            .DefaultIfEmpty("---")
                            .Aggregate((a, b) => $"{a} / {b}");

                        // Una línea por parcela con sangría
                        body.AppendChild(new Paragraph(
                            new ParagraphProperties(
                                new Indentation { Left = "720" },
                                new SpacingBetweenLines { Before = "40", After = "40" }),
                            new Run(
                                new RunProperties(new FontSize { Val = "24" }),
                                new Text($"{tipoParcela}:  {difuntos}")
                                { Space = SpaceProcessingModeValues.Preserve })));
                    }

                    // ── Línea separadora entre secciones ────────────────────────────
                    body.AppendChild(new Paragraph(
                        new ParagraphProperties(
                            new ParagraphBorders(
                                new BottomBorder
                                {
                                    Val = BorderValues.Single,
                                    Size = 4,
                                    Color = "AAAAAA"
                                }),
                            new SpacingBetweenLines { Before = "120", After = "120" })));
                }

                mainPart.Document.Save();
            }

            string fileNameWord = $"Concesiones_{DateTime.Now:dd_MM_yyyy_HHmm}.docx";
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                fileNameWord);
        }




        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> EnviarWhatsapp(string celular, int tramiteId)
        {
            string numero = celular.Replace(" ", "")
                                   .Replace("-", "")
                                   .Replace("(", "")
                                   .Replace(")", "");

            string mensaje = Uri.EscapeDataString(await _deudaConcesionService.CalculoDeudaConcesion(tramiteId));

            if (!numero.StartsWith("549"))
            {
                numero = "549" + numero;
            }

            return Redirect($"https://wa.me/{numero}?text={mensaje}");
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
