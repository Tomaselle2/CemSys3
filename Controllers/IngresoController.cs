using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Ingreso;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.EmpresaSepelio;
using CemSys3.Interfaces.HistorialEstados;
using CemSys3.Interfaces.Ingreso;
using CemSys3.Interfaces.Notas;
using CemSys3.Interfaces.Persona;
using CemSys3.Interfaces.Tarifaria;
using CemSys3.Interfaces.Usuario;
using CemSys3.ViewModels.Ingreso;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CemSys3.Controllers
{
    public class IngresoController : Controller
    {
        public readonly INotas _notaService;
        public readonly IEmpresaSepelio _empresaService;
        public readonly IUsuario _ususarioService;
        public readonly IPersona _personaService;
        public readonly IIngreso _ingresoService;
        public readonly IPrecioIngresoService _preciosIngresos;
        public readonly IHistorialEstados _historialEstados;


        public IngresoController(INotas notasService, IEmpresaSepelio empresaSepelio, 
            IUsuario usuarioService, IPersona personaService,
            IIngreso ingresoService, IPrecioIngresoService preciosIngresos,
            IHistorialEstados historialEstados)
        {
            _notaService = notasService;
            _empresaService = empresaSepelio;
            _ususarioService = usuarioService;
            _personaService = personaService;
            _ingresoService = ingresoService;
            _preciosIngresos = preciosIngresos;
            _historialEstados = historialEstados;
        }

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

            try
            {
                bool personaExiste = false;

                if (viewModel.Dni.HasValue)
                {
                    personaExiste = await _personaService.PersonaExiste(viewModel.Dni.Value, viewModel.Sexo ?? "");
                }

                if (personaExiste) //difunto ya existe
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
                    InformacionAdicional = viewModel.InformacionAdicional,
                    Difunto = difunto,
                    EmpleadoIngresoId = viewModel.EmpleadoID ?? 0,
                    Visibilidad = true,
                    NotaId = viewModel.NotaIngreso.Id
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

            try
            {
                viewModel.Resumen = await _ingresoService.Get(ingresoId);
                viewModel.IngresoId = ingresoId;
                viewModel.InformacionAdicionalIngreso = viewModel.Resumen.InformacionAdicional;
                viewModel.PreciosIngresos = await _preciosIngresos.GetPreciosIngresoBy(viewModel.Resumen.TipoParcelaId, viewModel.Resumen.EstadoDifuntoId);
                viewModel.HistorialEstados = await _historialEstados.GetAllById(ingresoId);
                viewModel.PreciosAperturas = await _preciosIngresos.GetPreciosAperturas(viewModel.Resumen.TipoParcelaId);
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
