using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Ingreso;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Archivo;
using CemSys3.Interfaces.EmpresaSepelio;
using CemSys3.Interfaces.HistorialEstados;
using CemSys3.Interfaces.Ingreso;
using CemSys3.Interfaces.Notas;
using CemSys3.Interfaces.Persona;
using CemSys3.Interfaces.Tarifaria;
using CemSys3.Interfaces.Usuario;
using CemSys3.ViewModels.Ingreso;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class IngresoController : Controller
    {
        private readonly INotas _notaService;
        private readonly IEmpresaSepelio _empresaService;
        private readonly IUsuario _ususarioService;
        private readonly IPersona _personaService;
        private readonly IIngreso _ingresoService;
        private readonly IPrecioIngresoService _preciosIngresos;
        private readonly IHistorialEstados _historialEstados;
        private readonly IArchivo _archivoService;

        public IngresoController(INotas notasService, IEmpresaSepelio empresaSepelio, 
            IUsuario usuarioService, IPersona personaService,
            IIngreso ingresoService, IPrecioIngresoService preciosIngresos,
            IHistorialEstados historialEstados, IArchivo archivoService)
        {
            _notaService = notasService;
            _empresaService = empresaSepelio;
            _ususarioService = usuarioService;
            _personaService = personaService;
            _ingresoService = ingresoService;
            _preciosIngresos = preciosIngresos;
            _historialEstados = historialEstados;
            _archivoService = archivoService;
        }

        //vista muestra el listado de ingresos
        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> ListadoIngresos(DateOnly? fechaDesde, DateOnly? fechaHasta, int? tipoParcelaID = null, int? seccionID = null, int? parcelaID = null, int filtro = 0, int pagina = 1, int porPagina = 10)
        {
            ListadoIngresosVM viewModel = new ListadoIngresosVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();

            if (fechaDesde.HasValue && fechaHasta.HasValue)
            {
                if(fechaHasta < fechaDesde)
                {
                    viewModel.SweetAlert = new SweetAlertDTO
                    {
                        Titulo = "Validación",
                        Mensaje = $"La fecha hasta: {fechaHasta?.ToString("dd/MM/yyyy")} es menor a fecha desde: {fechaDesde?.ToString("dd/MM/yyyy")}",
                        Tipo = "warning"
                    };

                    return View(viewModel);
                }
            }

            try
            {
                PaginadoResponse<ListadoIngresosDTO> resultado = await _ingresoService.GetAllPaginadoIngresos(fechaDesde, fechaHasta, pagina, porPagina, filtro, tipoParcelaID, seccionID, parcelaID);
                viewModel.Ingresos = resultado.Items;
                viewModel.Paginacion = resultado.Paginacion;

                viewModel.Paginacion.Parametros = new Dictionary<string, string>();

                viewModel.Paginacion.Parametros.Add("fechaDesde", fechaDesde?.ToString("yyyy-MM-dd") ?? "");
                viewModel.Paginacion.Parametros.Add("fechaHasta", fechaHasta?.ToString("yyyy-MM-dd") ?? "");
                viewModel.Paginacion.Parametros.Add("porPagina", porPagina.ToString());
                viewModel.Paginacion.Parametros.Add("filtro", filtro.ToString());
                viewModel.Paginacion.Parametros.Add("tipoParcelaID", viewModel.TipoParcelaID?.ToString() ?? "");
                viewModel.Paginacion.Parametros.Add("seccionID", viewModel.SeccionID?.ToString() ?? "");
                viewModel.Paginacion.Parametros.Add("parcelaID", viewModel.ParcelaID?.ToString() ?? "");

                viewModel.FechaDesde = fechaDesde;
                viewModel.FechaHasta = fechaHasta;
                viewModel.TipoParcelaID = tipoParcelaID;
                viewModel.SeccionID = seccionID;
                viewModel.ParcelaID = parcelaID;

                if (viewModel.Ingresos.Count() == 0)
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
                    Mensaje = $"Ocurrió un error al cargar los ingresos: {ex.Message}",
                    Tipo = "error"
                };
            }

            return View(viewModel);
        }

        //vista para realizar el ingreso
        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> Index(int notaId)
        {
            IngresoVM viewModel = new IngresoVM();
            try
            {
               await CargarListasIngreso(viewModel, notaId);
            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = ex.Message,
                    Tipo = "error"
                };
            }

            viewModel.SweetAlert = TempData.GetSweetAlert();

            return View(viewModel);
        }

        //envia los datos del ingreso al servidro
        [HttpPost]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> Ingreso(IngresoVM viewModel)
        {
            //ignorar validacion de notaIngreso
            ModelState.Remove("NotaIngreso.Nombre");

            if (!ModelState.IsValid)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Verificar",
                    Mensaje = "Hay datos incompletos",
                    Tipo = "warning"
                };

                await CargarListasIngreso(viewModel, viewModel.NotaIngreso.Id);
                return View("Index", viewModel);
            }

            //la fecha de defuncion no puede ser mayor a la fecha del ingreso
            if (viewModel.FechaDefuncion.HasValue && viewModel.FechaHoraIngreso.HasValue)
            {
                DateOnly fechaIngreso = DateOnly.FromDateTime(viewModel.FechaHoraIngreso.Value);
                if (viewModel.FechaDefuncion.Value > fechaIngreso)
                {
                    ModelState.AddModelError(
                        nameof(viewModel.FechaDefuncion),
                        "La fecha de defunción no puede ser posterior a la fecha de ingreso."
                    );

                    viewModel.SweetAlert = new SweetAlertDTO
                    {
                        Titulo = "Verificar",
                        Mensaje = "La fecha de defunción no puede ser posterior a la fecha de ingreso.",
                        Tipo = "warning"
                    };

                    await CargarListasIngreso(viewModel, viewModel.NotaIngreso.Id);
                    return View("Index", viewModel);
                }
            }

            //la fecha de defuncion no puede ser menor a la fecha de nacimiento
            if (viewModel.FechaDefuncion.HasValue && viewModel.FechaNacimiento.HasValue)
            {
                if (viewModel.FechaDefuncion.Value < viewModel.FechaNacimiento.Value)
                {
                    ModelState.AddModelError(
                        nameof(viewModel.FechaDefuncion),
                        "La fecha de defunción no puede ser anterior a la fecha de nacimiento."
                    );

                    viewModel.SweetAlert = new SweetAlertDTO
                    {
                        Titulo = "Verificar",
                        Mensaje = "La fecha de defunción no puede ser anterior a la fecha de nacimiento.",
                        Tipo = "warning"
                    };
                    await CargarListasIngreso(viewModel, viewModel.NotaIngreso.Id);
                    return View("Index", viewModel);
                }
            }

            if (viewModel.TipoParcelaID == 0 || viewModel.TipoParcelaID == null)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Verificar",
                    Mensaje = "Hay datos incompletos",
                    Tipo = "warning"
                };

                await CargarListasIngreso(viewModel, viewModel.NotaIngreso.Id);
                return View("Index", viewModel);
            }

            try
            {
                bool personaExiste = false;

                if (viewModel.Dni.HasValue)
                {
                    personaExiste = await _personaService.PersonaExiste(viewModel.Dni.Value, viewModel.Sexo ?? "");
                }

                if (personaExiste) //persona ya registrada con (DNI + SEXO)
                {
                    PersonaDTO persona = new PersonaDTO();

                    //consultar el tipo de persona
                    if (viewModel.Dni.HasValue && viewModel.Sexo != null)
                    {
                        persona = await _personaService.GetByDNISexo(viewModel.Dni.Value, viewModel.Sexo);
                    }

                    if (persona.CategoriaPersonaId == (int)CategoriaPersonaEnum.Titular) 
                    {
                        // ¿El usuario ya confirmó que quiere convertirlo?
                        if (viewModel.IngresoTitularFallecido)
                        {
                            // Marcar que la persona existe y es titular → el service lo maneja
                            // Continuar el flujo normal pero pasando el Id de la persona existente
                            // (no crear una nueva persona, sino actualizar la existente)
                        }
                        else
                        {
                            // Primera vez que choca: devolver el form con el flag para mostrar el modal
                            viewModel.IngresoTitularFallecido = false; // todavía no confirmó
                            viewModel.SweetAlert = null; // no usamos SweetAlert, usamos modal propio
                                                         // Señal para la vista de que debe mostrar el modal de confirmación
                            ViewBag.MostrarModalTitular = true;
                            ViewBag.NombreTitular = $"{persona.Apellido?.ToUpper()}, {persona.Nombre}";
                            await CargarListasIngreso(viewModel, viewModel.NotaIngreso.Id);
                            return View("Index", viewModel);
                        }

                    }
                    else //si es difunto enviar error difunto existente
                    {
                        viewModel.SweetAlert = new SweetAlertDTO
                        {
                            Titulo = "Verificar",
                            Mensaje = "El difunto que intenta ingresar ya existe",
                            Tipo = "warning"
                        };

                        await CargarListasIngreso(viewModel, viewModel.NotaIngreso.Id);

                        return View("Index", viewModel);
                    }

                }

                PersonaDTO difunto = new PersonaDTO
                {
                    Nombre = viewModel.Nombre?.Trim(),
                    Apellido = viewModel.Apellido?.Trim(),
                    Dni = viewModel.Dni?.ToString("D8"),
                    FechaDefuncion = viewModel.FechaDefuncion,
                    Sexo = viewModel.Sexo,
                    EstadoDifuntoId = viewModel.EstadoDifuntoId,
                    FechaNacimiento = viewModel.FechaNacimiento,
                    NroActa = viewModel.NroActa,
                    NroFolio =  viewModel.NroFolio,
                    NroTomo = viewModel.NroTomo,
                    NroSerie = viewModel.NroSerie?.Trim(),
                    NroAge = viewModel.NroAge,
                    Visibilidad = true,
                    InformacionAdicional = viewModel.InformacionAdicional
                };

                IngresoDTO ingreso = new IngresoDTO
                {
                    FechaIngreso = viewModel.FechaHoraIngreso,
                    UsuarioLogueadoId = HttpContext.Session.GetInt32("IdUsuario") ?? 0,
                    EmpresaFunebreId = viewModel.EmpresaFunebreID,
                    ParcelaId = viewModel.ParcelaID ?? 0,
                    EstadoDifuntoId = viewModel.EstadoDifuntoId ?? 0,
                    Difunto = difunto,
                    EmpleadoIngresoId = viewModel.EmpleadoID ?? 0,
                    Visibilidad = true,
                    NotaId = viewModel.NotaIngreso.Id,
                    PersonaExistenteId = viewModel.IngresoTitularFallecido
                        ? (await _personaService.GetByDNISexo(viewModel.Dni!.Value, viewModel.Sexo!)).Id
                        : null
                };

                GenericResultDTO resultado = await _ingresoService.Add(ingreso);

                if (resultado.Success)
                {
                    TempData.SetSweetAlert(new SweetAlertDTO { 
                    
                        Titulo = "Éxito",
                        Mensaje = resultado.Message,
                        Tipo = "success"
                    });
                }

                return RedirectToAction("ResumenIngreso", new { ingresoId = resultado.Id});
            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = ex.Message,
                    Tipo = "error"
                };
                await CargarListasIngreso(viewModel, viewModel.NotaIngreso.Id);
                return View("Index", viewModel);
            }
        }

        //pantalla despues de ingreso exitoso
        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> ResumenIngreso(int ingresoId)
        {
            ResumenIngresoVM viewModel = new ResumenIngresoVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();

            try
            {
                viewModel.Resumen = await _ingresoService.Get(ingresoId);
                viewModel.IngresoId = ingresoId;
                viewModel.InformacionAdicionalIngreso = viewModel.Resumen.InformacionAdicional;

                int estadoDifuntoId = viewModel.Resumen.EstadoDifuntoId;
                if (viewModel.Resumen.EstadoDifuntoId == (int)EstadoDifuntoEnum.Reducido)
                {
                    estadoDifuntoId = (int)EstadoDifuntoEnum.CuerpoCompleto;
                }
                viewModel.PreciosIngresos = await _preciosIngresos.GetPreciosIngresoBy(viewModel.Resumen.TipoParcelaId, estadoDifuntoId);
                viewModel.HistorialEstados = await _historialEstados.GetAllById(ingresoId);
                viewModel.PreciosAperturas = await _preciosIngresos.GetPreciosAperturas(viewModel.Resumen.TipoParcelaId);
                viewModel.Archivos = await _archivoService.GetAllByTramiteId(ingresoId);
            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = ex.Message,
                    Tipo = "error"
                };
            }

            return View(viewModel);
        }

        //finaliza el tramite de ingreso
        [HttpPost]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> Finalizar(int ingresoId, string cobroIngreso, string cobroApertura)
        {

            try
            {
               await _ingresoService.FinalizarIngreso(ingresoId, cobroIngreso, cobroApertura);
               TempData.SetSweetAlert(
                    new SweetAlertDTO
                    {
                        Titulo = "Éxito",
                        Mensaje = "Ingreso finalizado correctamente",
                        Tipo = "success"
                    });
                return RedirectToAction("ResumenIngreso", new { ingresoId = ingresoId });
            }
            catch (Exception ex)
            {
                TempData.SetSweetAlert(
                    new SweetAlertDTO
                    {
                        Titulo = "Error",
                        Mensaje = ex.Message,
                        Tipo = "error"
                    });
                return RedirectToAction("ResumenIngreso", new { ingresoId = ingresoId });
            }
        }

        //carga los select, solo para formulario de carga de difunto
        private async Task CargarListasIngreso(IngresoVM viewModel, int notaId)
        {
            //Cargar listas desplegables si es necesario
            viewModel.NotaIngreso = await _notaService.Get(notaId);
            viewModel.ListaEmpresasSepelio = await _empresaService.GetAll();
            viewModel.ListaEmpleados = await _ususarioService.GetAll();
        }

    }
}
