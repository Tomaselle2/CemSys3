using CemSys3.Business.Ingreso;
using CemSys3.Business.Persona;
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
using CemSys3.Interfaces.Usuario;
using CemSys3.ViewModels.CargaDifunto;
using CemSys3.ViewModels.Ingreso;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class CargaDifuntoController : Controller
    {
        private readonly IUsuario _usuarioService;
        private readonly IPersona _personaService;
        private readonly ICargaDifunto _cargaDifuntoService;

        public CargaDifuntoController(IUsuario usuario, IPersona persona, ICargaDifunto cargaDifunto)
        {
            _usuarioService = usuario;
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
                        viewModel.SweetAlert = new SweetAlertDTO
                        {
                            Titulo = "Verificar",
                            Mensaje = "El difunto que intenta registrar es titular en concesiones",
                            Tipo = "warning"
                        };

                    }
                    else //si es difunto enviar error difunto existente
                    {
                        viewModel.SweetAlert = new SweetAlertDTO
                        {
                            Titulo = "Verificar",
                            Mensaje = "El difunto que intenta ingresar ya existe",
                            Tipo = "warning"
                        };
                    }

                    return View("Index", viewModel);
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
