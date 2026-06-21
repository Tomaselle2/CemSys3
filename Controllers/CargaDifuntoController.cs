using CemSys3.DTOs.CargaDifunto;
using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Ingreso;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.CargaDifunto;
using CemSys3.Interfaces.Persona;
using CemSys3.ViewModels.CargaDifunto;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class CargaDifuntoController : Controller
    {
        private readonly IPersona _personaService;
        private readonly ICargaDifunto _cargaDifuntoService;

        public CargaDifuntoController(IPersona persona, ICargaDifunto cargaDifunto)
        {
            _personaService = persona;
            _cargaDifuntoService = cargaDifunto;
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> Index()
        {
            CargaDifuntoVM viewModel = new CargaDifuntoVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();

            return View(viewModel);
        }

        //envia los datos del difunto al servidor
        [HttpPost]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> CargaDifunto(CargaDifuntoVM viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Verificar",
                    Mensaje = "Hay datos incompletos",
                    Tipo = "warning"
                };

                return View("Index", viewModel);
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

                return View("Index", viewModel);
            }

            try
            {
                if ((viewModel.IngresoTitularFallecido || viewModel.ReingresoConfirmado) && !viewModel.PersonaCoincidenciaId.HasValue)
                {
                    viewModel.IngresoTitularFallecido = false;
                    viewModel.ReingresoConfirmado = false;
                }

                if (!viewModel.IngresoTitularFallecido && !viewModel.ReingresoConfirmado)
                {
                    string nombreForm = viewModel.Nombre?.Trim() ?? "";
                    string apellidoForm = viewModel.Apellido?.Trim() ?? "";

                    CoincidenciaIngresoDTO coincidencia = await _personaService.BuscarCoincidenciaParaIngreso(
                        viewModel.Dni, viewModel.Sexo, nombreForm, apellidoForm, viewModel.EsPersonaDistinta);

                    if (coincidencia.Existe)
                    {
                        if (coincidencia.EsTitular)
                        {
                            viewModel.SweetAlert = null;
                            ViewBag.MostrarModalTitular = true;
                            ViewBag.NombreTitular = $"{coincidencia.Persona!.Apellido?.ToUpper()}, {coincidencia.Persona.Nombre}";
                            ViewBag.PersonaCoincidenciaId = coincidencia.Persona.Id;
                            return View("Index", viewModel);
                        }

                        if (coincidencia.EstaActivoEnCementerio)
                        {
                            viewModel.SweetAlert = new SweetAlertDTO
                            {
                                Titulo = "Verificar",
                                Mensaje = "El difunto que intenta cargar ya existe y se encuentra actualmente en el cementerio.",
                                Tipo = "warning"
                            };
                            return View("Index", viewModel);
                        }

                        // Existe pero está retirado -> candidato a reingreso
                        viewModel.SweetAlert = null;
                        ViewBag.MostrarModalReingreso = true;
                        ViewBag.NombreReingreso = $"{coincidencia.Persona!.Apellido?.ToUpper()}, {coincidencia.Persona.Nombre}";
                        ViewBag.PersonaCoincidenciaId = coincidencia.Persona.Id;
                        ViewBag.CoincidenciaPorDni = coincidencia.CoincidenciaPorDni;
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
                    NroFolio = viewModel.NroFolio,
                    NroTomo = viewModel.NroTomo,
                    NroSerie = viewModel.NroSerie?.Trim(),
                    NroAge = viewModel.NroAge,
                    Visibilidad = true,
                    InformacionAdicional = viewModel.InformacionAdicional,
                    FechaIngreso = viewModel.FechaIngreso,
                };

                CargaDifuntoDTO dto = new CargaDifuntoDTO
                {
                    UsuarioLogueadoId = HttpContext.Session.GetInt32("IdUsuario") ?? 0,
                    ParcelaId = viewModel.ParcelaID ?? 0,
                    EstadoDifuntoId = viewModel.EstadoDifuntoId ?? 0,
                    Difunto = difunto,
                    Visibilidad = true,
                    PersonaExistenteId = (viewModel.IngresoTitularFallecido || viewModel.ReingresoConfirmado)
                        ? viewModel.PersonaCoincidenciaId
                        : null
                };

                GenericResultDTO resultado = await _cargaDifuntoService.Add(dto);

                if (resultado.Success)
                {
                    TempData.SetSweetAlert(new SweetAlertDTO
                    {
                        Titulo = "Éxito",
                        Mensaje = resultado.Message,
                        Tipo = "success"
                    });
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = ex.Message,
                    Tipo = "error"
                };
                return View("Index", viewModel);
            }
        }
    }
}
