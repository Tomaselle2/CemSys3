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

        public ConcesionController(IConcesion concesion, IViewRenderService render, PlaywrightPdfGenerator pdfGenerator, IArchivo archivo,
            IHistorialEstados historialEstados)
        {
            _concesionService = concesion;
            _viewRenderService = render;
            _pdfGenerator = pdfGenerator;
            _archivoService = archivo;
            _historialEstadosService = historialEstados;
        }

        //tabla general de concesiones, con paginacion y filtro por estado
        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> TablaGeneral(int filtroEstado = 0, int pagina = 1, int porPagina = 10)
        {
            TablaGeneralVM viewModel = new TablaGeneralVM();
            try
            {
                PaginadoResponse<TablaConcesionDTO> resultado = await _concesionService.GellAllPaginado(filtroEstado, pagina, porPagina);
                viewModel.Listado = resultado.Items;
                viewModel.Paginacion = resultado.Paginacion;

                viewModel.Paginacion.Parametros = new Dictionary<string, string>();

                viewModel.Paginacion.Parametros.Add("filtroEstado", filtroEstado.ToString());
                viewModel.Paginacion.Parametros.Add("porPagina", porPagina.ToString());
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

            viewModel.SweetAlert = TempData.GetSweetAlert();
            return View(viewModel);
        }

        //vista para hacer el contrato de conesion, por primera vez o para renovacion, dependiendo del estado del tramite
        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> GenerarContrato(int idTramite)
        {
            GenerarContratoVM viewModel = new GenerarContratoVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();

            try
            {
                viewModel.contrato = await _concesionService.SolicitarDatosParaGenerarContrato(idTramite);

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
            //generar el pdf del contrato.
            GenerarContratoDTO contratoGenerado = new GenerarContratoDTO();
            contratoGenerado.TramiteId = viewModel.contrato.TramiteId;
            contratoGenerado.EstadoTramiteId = viewModel.contrato.EstadoTramiteId;
            contratoGenerado.ParcelaId = viewModel.contrato.ParcelaId;
            contratoGenerado.TipoParcela = viewModel.contrato.TipoParcela;
            contratoGenerado.SeccionId = viewModel.contrato.SeccionId;
            contratoGenerado.NombreSeccion = viewModel.contrato.NombreSeccion;
            contratoGenerado.NroParcela = viewModel.contrato.NroParcela;
            contratoGenerado.NroFila = viewModel.contrato.NroFila;
            contratoGenerado.NroConcesion = viewModel.contrato.NroConcesion != null ? viewModel.contrato.NroConcesion : null;
            contratoGenerado.Difuntos = viewModel.contrato.Difuntos;
            contratoGenerado.Titulares = viewModel.contrato.Titulares;
            contratoGenerado.baseUrl = $"{Request.Scheme}://{Request.Host}";
            contratoGenerado.PrecioEnLetras = NumeroALetras.ConvertirALetras(viewModel.PrecioFinal);
            contratoGenerado.formaPago = viewModel.FormaDePago ?? "";
            contratoGenerado.CuotaId = viewModel.CantidadCuotaSeleccionada;
            contratoGenerado.Precio = viewModel.PrecioFinal;
            contratoGenerado.OtraFormaPago = viewModel.otraFormaPago;
            contratoGenerado.CantidadAniosId = viewModel.CantidadAniosId.Value;
            contratoGenerado.Vencimiento = viewModel.Vencimiento.Value;
            contratoGenerado.fechaGeneracion = DateTime.Now;

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
            dto.MensajeParcela = $"\n● El {DateTime.Now.ToString("dd/MM/yyyy")} se realizo contrato de concesión ({viewModel.contrato.NroConcesion?.ToString("D5") ?? "-----"}) por {EnumHelper.GetDisplayNameByValue<AniosConcesionEnum>(viewModel.CantidadAniosId.Value)}. Vencimiento {viewModel.Vencimiento}.";
            dto.InformacionAdicional = $"\n● El {DateTime.Now.ToString("dd/MM/yyyy")} se realizo contrato de concesión ({viewModel.contrato.NroConcesion?.ToString("D5") ?? "-----"}) por {EnumHelper.GetDisplayNameByValue<AniosConcesionEnum>(viewModel.CantidadAniosId.Value)}. Vencimiento {viewModel.Vencimiento}.";

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
                baseUrl = $"{Request.Scheme}://{Request.Host}",
                PrecioEnLetras = NumeroALetras.ConvertirALetras(viewModel.PrecioFinal),
                formaPago = viewModel.FormaDePago ?? "",
                CuotaId = viewModel.CantidadCuotaSeleccionada,
                Precio = viewModel.PrecioFinal,
                OtraFormaPago = viewModel.otraFormaPago,
                CantidadAniosId = viewModel.CantidadAniosId.Value,
                Vencimiento = viewModel.Vencimiento.Value,
                fechaGeneracion = DateTime.Now
            };

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

            //La nota del contrato se genera al momento de confirmar que el contrato esta firmado.
            //nota de tipo recordatorio para el contrato
            //-Guardar el contrato en la carpeta.
            //-Modificar el vencimiento en Progam
            //-Modificar el titular en Program
            //-Generar el cobro en Program (se datalla como)
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
            return View(viewModel);
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
    }
}
