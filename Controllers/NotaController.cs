using CemSys3.DTOs.Nota;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.Tarea;
using CemSys3.Enumerables;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.HistorialEstados;
using CemSys3.Interfaces.Notas;
using CemSys3.ViewModels.Nota;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CemSys3.Controllers
{
    public class NotaController : Controller
    {
        private readonly INotas _notaService;
        private readonly IHistorialEstados _historialEstados;

        public NotaController(INotas notasService, IHistorialEstados historialEstados)
        {
            _notaService = notasService;
            _historialEstados = historialEstados;
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> Index(DateOnly? fechaDesde, DateOnly? fechaHasta, int estadoId = (int)EstadosNotaEnum.NotaPendiente, int filtro = 0, int pagina = 1, int porPagina = 10)
        {
            NotaVM viewModel = new NotaVM();

            if (fechaDesde.HasValue && fechaHasta.HasValue)
            {
                if (fechaHasta < fechaDesde)
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

            if (porPagina <= 0)
                porPagina = 10;

            try
            {
                PaginadoResponse<NotaDTO> paginadoNotas = await _notaService.GetPaginadoByTipo(fechaDesde, fechaHasta, estadoId, filtro, pagina, porPagina);
                viewModel.ListaNotas = paginadoNotas.Items;
                viewModel.Paginacion = paginadoNotas.Paginacion;

                // Inicializar parámetros si es null
                viewModel.Paginacion.Parametros ??= new Dictionary<string, string>();

                // Actualizar solo con los valores actuales
                viewModel.Paginacion.Parametros["filtro"] = filtro.ToString();
                viewModel.Paginacion.Parametros["estadoId"] = estadoId.ToString();
                viewModel.Paginacion.Parametros["porPagina"] = porPagina.ToString();
                viewModel.Paginacion.Parametros.Add("fechaDesde", fechaDesde?.ToString("yyyy-MM-dd") ?? "");
                viewModel.Paginacion.Parametros.Add("fechaHasta", fechaHasta?.ToString("yyyy-MM-dd") ?? "");

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
                    new() { Descripcion = "Realizar ingreso en CemSys", Estado = false },
                    new() { Descripcion = "Marcar en el plano", Estado = false },
                    new() { Descripcion = "Completar el libro", Estado = false }

                }
            };

            return PartialView("_ModalNota", vm);
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public IActionResult ModalRecordatorio()
        {
            var vm = new NotaModalVM
            {
                TipoNotaId = (int)TipoNotaEnum.Recordatorio,
                Color = "#F5DADE",
                Descripcion =
@"• Escriba una breve descripción: ",
                Tareas = new List<TareaDTO>
                {
                    new() { Descripcion = "Completar tarea", Estado = false }
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
                    UsurioId = vm.UsuarioId,
                    Tareas = vm.Tareas,
                    EstadoId = vm.EstadoId,
                    FechaFinRecordatorio = vm.FechaFinRecordatorio
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
        public async Task<IActionResult> Visualizar(int id, string? controlador)
        {
            try
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
                    Tareas = nota.Tareas,
                    EstadoId = nota.EstadoId,
                    tramiteVinculadoId = nota.TramiteIngresoId ?? 0,
                    controlador = controlador ?? string.Empty,
                    HistorialEstados = await _historialEstados.GetAllById(nota.Id),
                    FechaCreacion = nota.FechaCreacion,
                    FechaFinRecordatorio = nota.FechaFinRecordatorio
                };

                if(viewModel.EstadoId == (int)EstadosNotaEnum.NotaFinalizado)
                    viewModel.NotaFinalizada = true;
                else
                    viewModel.NotaFinalizada = false;

                return PartialView("_ModalNota", viewModel);
            }
            catch (Exception ex)
            {
                return Content($$"""
                <script>
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: '{{ex.Message.Replace("'", "\\'")}}'
                    })
                    .then(()=> location.reload());
                </script>
                """);
            }
            
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public IActionResult RealizarIngreso(int tramiteId, int notaId)
        {
            if(tramiteId <= 0) //ingreso nuevo
            {
                return RedirectToAction("Index", "Ingreso", new { notaId = notaId });
            }
            else //ya existe el trámite de ingreso
            {
                return RedirectToAction("IrATramite", "Tramite",  new { tramiteId = tramiteId});
            }
        }
    }
}
