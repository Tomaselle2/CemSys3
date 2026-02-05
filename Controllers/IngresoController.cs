using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Ingreso;
using CemSys3.DTOs.Nota;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.EmpresaSepelio;
using CemSys3.Interfaces.Ingreso;
using CemSys3.Interfaces.Notas;
using CemSys3.Interfaces.Parcela;
using CemSys3.Interfaces.Persona;
using CemSys3.Interfaces.Seccion;
using CemSys3.Interfaces.Usuario;
using CemSys3.Models;
using CemSys3.ViewModels.Ingreso;
using CemSys3.ViewModels.Nota;
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

        public IngresoController(INotas notasService, IEmpresaSepelio empresaSepelio, 
            IUsuario usuarioService, IPersona personaService,
            IIngreso ingresoService )
        {
            _notaService = notasService;
            _empresaService = empresaSepelio;
            _ususarioService = usuarioService;
            _personaService = personaService;
            _ingresoService = ingresoService;
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

        private async Task CargarListasIngreso(IngresoVM viewModel, int notaId)
        {
            //Cargar listas desplegables si es necesario
            viewModel.NotaIngreso = await _notaService.Get(notaId);
            viewModel.ListaEmpresasSepelio = await _empresaService.GetAll();
            viewModel.ListaEmpleados = await _ususarioService.GetAll();
        }

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
                    Dni = viewModel.Dni?.ToString(),
                    FechaDefuncion = viewModel.FechaDefuncion,
                    Sexo = viewModel.Sexo,
                    EstadoDifuntoId = viewModel.EstadoDifuntoId,
                    FechaNacimiento = viewModel.FechaNacimiento,
                    NroActa = viewModel.NroActa,
                    NroFolio =  viewModel.NroFolio,
                    NroTomo = viewModel.NroTomo,
                    NroSerie = viewModel.NroSerie,
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
                    Visibilidad = true
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

                return RedirectToAction("Index", new { notaId = viewModel.NotaIngreso.Id });
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
    }
}
