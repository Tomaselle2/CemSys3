using CemSys3.DTOs.Nota;
using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.EmpresaSepelio;
using CemSys3.Interfaces.Notas;
using CemSys3.Interfaces.Parcela;
using CemSys3.Interfaces.Seccion;
using CemSys3.Interfaces.Usuario;
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

        public IngresoController(INotas notasService, IEmpresaSepelio empresaSepelio, IUsuario usuarioService)
        {
            _notaService = notasService;
            _empresaService = empresaSepelio;
            _ususarioService = usuarioService;
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> Index(int notaId)
        {
            IngresoVM viewModel = new IngresoVM();
            try
            {
                viewModel.NotaIngreso = await _notaService.Get(notaId);
                viewModel.ListaEmpresasSepelio = await _empresaService.GetAll();
                viewModel.ListaEmpleados = await _ususarioService.GetAll();
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
    }
}
