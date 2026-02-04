using CemSys3.DTOs.Nota;
using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Notas;
using CemSys3.Interfaces.Parcela;
using CemSys3.Interfaces.Seccion;
using CemSys3.ViewModels.Ingreso;
using CemSys3.ViewModels.Nota;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CemSys3.Controllers
{
    public class IngresoController : Controller
    {
        public readonly INotas _notaService;
        public readonly ISeccion _seccionService;
        public readonly IParcela _parcelaService;

        public IngresoController(INotas notasService, ISeccion seccionService, IParcela parcelaService)
        {
            _notaService = notasService;
            _seccionService = seccionService;
            _parcelaService = parcelaService;
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> Index(int notaId)
        {
            IngresoVM viewModel = new IngresoVM();
            try
            {
                viewModel.NotaIngreso = await _notaService.Get(notaId);
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

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> ObtenerSeccionesPorTipo(int tipoParcelaId)
        {
            try
            {
                var secciones = await _seccionService.GetAllByTipo(tipoParcelaId);
                return Json(secciones);
            }
            catch (Exception ex)
            {
                return Content($$"""
                <script>
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: '{{ex.Message.Replace("'", "\\'")}}'
                    });
                </script>
            """);
            }
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> ObtenerParcelasPorSeccion(int seccionId, int estadoDifuntoId)
        {
            try
            {
                var parcelas = await _parcelaService.GetAllBySeccionId(seccionId, estadoDifuntoId);
                return Json(parcelas);
            }
            catch (Exception ex)
            {
                return Content($$"""
                <script>
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: '{{ex.Message.Replace("'", "\\'")}}'
                    });
                </script>
            """);
            }
        }
    }
}
