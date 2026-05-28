using CemSys3.DTOs.PlantillaTramite;
using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.TramitesConcesion;
using CemSys3.DTOs.TramitesConcesion.PermisoIngreso;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Archivo;
using CemSys3.Interfaces.HistorialEstados;
using CemSys3.Interfaces.PlantillaTramite;
using CemSys3.Interfaces.Tarea;
using CemSys3.Interfaces.TramitesConcesion;
using CemSys3.ViewModels.PermisoIngreso;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class PermisoIngresoController : Controller
    {
        private readonly IDocumentoTramiteService _documentoService;
        private readonly IArchivo _archivoService;
        private readonly IHistorialEstados _historialService;
        private readonly ITarea _tareaService;
        private readonly IStrategyFactory _strategyFactory;
        private readonly IFirmantes _firmantesService;

        public PermisoIngresoController(IArchivo archivoService,
        IHistorialEstados historialService,
        IStrategyFactory strategyFactory,
        IDocumentoTramiteService documentoService,
         ITarea tareaService,
         IFirmantes firmante)
        {
            _archivoService = archivoService;
            _historialService = historialService;
            _strategyFactory = strategyFactory;
            _documentoService = documentoService;
            _tareaService = tareaService;
            _firmantesService = firmante;
        }

        [HttpPost]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> GenerarAutorizaciones(PermisoIngresoVM viewModel, int firmanteId, int tipoAutorizacionId)
        {
            int usuarioId = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

            ITramiteStrategy strategy = _strategyFactory.GetStrategy((int)TipoTramiteEnum.PermisoIngreso);


            // Buscar el firmante específico
            var firmantes = await _firmantesService.GetAllByTramite(viewModel.TramiteId);
            var firmante = firmantes.FirstOrDefault(f => f.Id == firmanteId);

            if (firmante == null)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Advertencia",
                    Mensaje = "Firmante no encontrado",
                    Tipo = "warning"
                });
                return RedirectToAction("Detalle", new { tramiteId = viewModel.TramiteId });
            }


            GeneraStrategyDTO dto = new GeneraStrategyDTO
            {
                TramiteId = viewModel.TramiteId,
                TitularesActuales = viewModel.Dto.TitularesActuales,
                UsuarioId = usuarioId,
                Parentesco = "Titular",
                NroParcela = viewModel.Dto.NroParcela.Value,
                NroFila = viewModel.Dto.NroFila.Value,
                NombreSeccion = viewModel.Dto.NombreSeccion ?? "",
                TipoParcela = viewModel.Dto.TipoParcela,
                Difuntos = viewModel.Dto.Difuntos,
                Firmantes = viewModel.Personas,
                TipoAutorizacionId = tipoAutorizacionId,
                FirmanteId = firmanteId,
                NombreNuevoDifunto = viewModel.NombreDifunto,
                NroConcesion = viewModel.Dto.NroConcesion.Value
            };

            try
            {
                await strategy.GenerarDocumentosAsync(dto);

                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Éxito",
                    Mensaje = "Autorizaciones generadas correctamente",
                    Tipo = "success"
                });

                return RedirectToAction("Detalle", new { tramiteId = viewModel.TramiteId });

            }
            catch (Exception ex)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Ocurrió un error al generar los documentos. " + ex.Message,
                    Tipo = "error"
                });
                return RedirectToAction("Detalle", new { tramiteId = viewModel.TramiteId });

            }
        }

        [HttpPost]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> IniciarTramite(int concesionId, int tipoTramiteId)
        {
            try
            {
                int usuarioId = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

                var strategy = _strategyFactory.GetCreateStrategy(tipoTramiteId);

                CrearTramiteDTO dto = new CrearTramiteDTO
                {
                    TramiteConcesionId = concesionId,
                    UsuarioId = usuarioId,
                };

                int tramiteId = await strategy.CrearAsync(dto);

                return RedirectToAction("Detalle", new { tramiteId });
            }
            catch (Exception ex)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = ex.Message,
                    Tipo = "error"
                });

                return RedirectToAction("Index", "TramiteConcesion", new { tramiteId = concesionId });
            }
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> Detalle(int tramiteId)
        {
            var vm = new PermisoIngresoVM();
            vm.SweetAlert = TempData.GetSweetAlert();

            try
            {
                var strategy = _strategyFactory.GetCreateStrategy((int)TipoTramiteEnum.PermisoIngreso);

                if (strategy is ITramiteCreateStrategy<PermisoIngresoDTO> typedStrategy)
                {
                    vm.Dto = await typedStrategy.ObtenerAsync(tramiteId);
                }
                else
                {
                    throw new Exception("Strategy incorrecta para obtener datos");
                }

                vm.TipoTramiteId = vm.Dto.TipoTramiteId;
                vm.TramiteId = vm.Dto.TramiteId;
                vm.concesionId = vm.Dto.ConcesionId;
                vm.Archivos = await _archivoService.GetAllByTramiteId(vm.TramiteId);
                vm.Historial = await _historialService.GetAllById(vm.TramiteId);
                vm.Documentos = await _documentoService.ObtenerPorTramiteAsync(vm.TramiteId);
                vm.Tareas = await _tareaService.GetAllByTramite(tramiteId);
                vm.Firmantes = await _firmantesService.GetAllByTramite(tramiteId);
                vm.NombreDifunto = vm.Dto.NombreDifuntoNuevo;
                return View("PermisoIngreso", vm);
            }
            catch (Exception ex)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Ocurrió un error al cargar los datos. " + ex.Message,
                    Tipo = "error"
                });

                return RedirectToAction("Index", "TramiteConcesion");
            }
        }


    }
}
