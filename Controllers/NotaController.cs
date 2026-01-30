using CemSys3.DTOs.Nota;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.Tarea;
using CemSys3.Enumerables;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Notas;
using CemSys3.ViewModels.Nota;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CemSys3.Controllers
{
    public class NotaController : Controller
    {
        private readonly INotas _notaService;

        public NotaController(INotas notasService)
        {
            _notaService = notasService;
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> Index(int estadoId = (int)EstadosNotaEnum.NotaPendiente, int filtro = 0, int pagina = 1, int porPagina = 10)
        {
            NotaVM viewModel = new NotaVM();

            if (porPagina <= 0)
                porPagina = 10;

            try
            {
                PaginadoResponse<NotaDTO> paginadoNotas = await _notaService.GetPaginadoByTipo(estadoId, filtro, pagina, porPagina);
                viewModel.ListaNotas = paginadoNotas.Items;
                viewModel.Paginacion = paginadoNotas.Paginacion;

                // Inicializar parámetros si es null
                viewModel.Paginacion.Parametros ??= new Dictionary<string, string>();

                // Actualizar solo con los valores actuales
                viewModel.Paginacion.Parametros["filtro"] = filtro.ToString();
                viewModel.Paginacion.Parametros["estadoId"] = estadoId.ToString();
                viewModel.Paginacion.Parametros["porPagina"] = porPagina.ToString();

                // Mantener otros parámetros si los hubiera
                viewModel.Paginacion.Parametros["pagina"] = pagina.ToString();
            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Ocurrió un error al cargar las notas: " + ex.Message,
                    Tipo = "error"
                };
            }
            return View(viewModel);
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public IActionResult ModalIngreso()
        {
            var vm = new NotaModalVM
            {
                TipoNotaId = (int)TipoNotaEnum.Ingreso,
                Color = "#e3f2fd",
                Descripcion =
@"• Fecha y hora: 
• Ubicación: 
• Contacto: 
• Empresa: ",
                Tareas = new List<TareaDTO>
                {
                    new() { Descripcion = "Recibir acta de defunción", Estado = false },
                    new() { Descripcion = "Cargar difunto en Progam", Estado = false },
                    new() { Descripcion = "Realizar contrato de concesión", Estado = false },
                    new() { Descripcion = "Cobrar ingreso", Estado = false },
                    new() { Descripcion = "Realizar ingreso en CemSys", Estado = false }
                }
            };

            return PartialView("_ModalNota", vm);
        }

        [HttpPost]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> Guardar(NotaModalVM vm)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_ModalNota", vm); // NO CIERRA MODAL
            }

            try
            {
                var dto = new NotaDTO
                {
                    Id = vm.Id ?? 0,
                    Nombre = vm.Nombre.Trim(),
                    Descripcion = vm.Descripcion,
                    Color = vm.Color,
                    TipoNotaId = vm.TipoNotaId,
                    Tareas = vm.Tareas
                };


                // Asignar estado según si la nota está finalizada o no
                if (vm.NotaFinalizada == true)
                    dto.EstadoId = (int)EstadosNotaEnum.NotaFinalizado;
                else
                    dto.EstadoId = (int)EstadosNotaEnum.NotaPendiente; //ya viene por defecto como pendiente pero por las dudas

                if (vm.Id.HasValue)
                    await _notaService.Update(dto);
                else
                    await _notaService.Add(dto);

                return Content("""
            <script>
                Swal.fire('Éxito','Nota guardada','success')
                    .then(()=> location.reload());
            </script>
        """);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return PartialView("_ModalNota", vm);
            }
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> Visualizar(int id)
        {
            var nota = await _notaService.Get(id);

            if (nota == null)
            {
                return Content("""
            <script>
                Swal.fire('Error','No se encontro la nota','error')
                    .then(()=> location.reload());
            </script>
        """);
            }

            NotaModalVM viewModel = new NotaModalVM
            {
                Id = nota.Id,
                Nombre = nota.Nombre,
                Descripcion = nota.Descripcion ?? "",
                Color = nota.Color ?? "#e3f2fd",
                TipoNotaId = nota.TipoNotaId,
                Tareas = nota.Tareas
            };

            return PartialView("_ModalNota", viewModel);
        }
    }
}
