using AspNetCoreGeneratedDocument;
using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.PDF;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.PDF;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Concesion;
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

        public ConcesionController(IConcesion concesion, IViewRenderService render, PlaywrightPdfGenerator pdfGenerator)
        {
            _concesionService = concesion;
            _viewRenderService = render;
            _pdfGenerator = pdfGenerator;
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
                viewModel.CalcularPrecioNichoUnAnio();
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
            //quitar la validacion de contrato.NroConcesion
            ModelState.Remove("contrato.NroConcesion");

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

                var pdfBytes = await _pdfGenerator.GenerateFromHtmlAsync(
                       html,
                       new PdfOptionsDto
                       {
                           Landscape = false,
                           MarginTop = "20px",
                           MarginLeft = "30px"
                       });
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
                    Correo = titular.CorreoElectronico
                };
                titulares.Add(persona);
            }

            dto.Titulares = titulares;

            //todos los datos se deben guardar al precionar en contrato firmado, no antes, para evitar que se guarden datos erroneos en la base de datos.
            //recibir el viewmodel con los datos del contrato.
            //Update al contrato con los datos
            //generar el pdf del contrato y guardarlo en el servidor


            //La nota del contrato se genera al momento de confirmar que el contrato esta firmado.
            //nota de tipo recordatorio para el contrato
            //-Guardar el contrato en la carpeta.
            //-Modificar el vencimiento en Progam
            //-Modificar el titular en Program
            //-Generar el cobro en Program (se datalla como)
            return View();
        }
    }
}
