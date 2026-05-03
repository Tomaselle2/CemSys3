using CemSys3.DTOs.PlantillaTramite;
using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.TramitesConcesion;
using CemSys3.DTOs.TramitesConcesion.CambioTitular;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Archivo;
using CemSys3.Interfaces.HistorialEstados;
using CemSys3.Interfaces.PlantillaTramite;
using CemSys3.Interfaces.Tarea;
using CemSys3.Interfaces.TramitesConcesion;
using CemSys3.ViewModels.TramiteConcesion;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class CambioTitularController : Controller
    {
        private readonly IDocumentoTramiteService _documentoService;
        private readonly IArchivo _archivoService;
        private readonly IHistorialEstados _historialService;
        private readonly ITarea _tareaService;
        private readonly IStrategyFactory _strategyFactory;


        public CambioTitularController(IArchivo archivoService,
        IHistorialEstados historialService,
        IStrategyFactory strategyFactory,
        IDocumentoTramiteService documentoService,
         ITarea tareaService)
        {
            _archivoService = archivoService;
            _historialService = historialService;
            _strategyFactory = strategyFactory;
            _documentoService = documentoService;
            _tareaService = tareaService;
        }

        [HttpPost]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> GenerarAutorizaciones(CambioTitularVM viewModel)
        {
            int usuarioId = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

            ITramiteStrategy strategy = _strategyFactory.GetStrategy((int)TipoTramiteEnum.CambioTitular);


            if (viewModel.Personas == null || viewModel.Personas.Count == 0)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Advertencia",
                    Mensaje = "Seleccione un nuevo titular",
                    Tipo = "warning"
                });

                return RedirectToAction("Detalle", new { tramiteId = viewModel.TramiteId });
            }

            GeneraStrategyDTO dto = new GeneraStrategyDTO
            {
                TramiteId = viewModel.TramiteId,
                NuevosTitulares = viewModel.Personas,
                TitularesActuales = viewModel.Dto.TitularesActuales,
                UsuarioId = usuarioId,
                Parentesco = "Titular",
                NroParcela = viewModel.Dto.NroParcela.Value,
                NroFila = viewModel.Dto.NroFila.Value,
                NombreSeccion = viewModel.Dto.NombreSeccion,
                TipoParcela = viewModel.Dto.TipoParcela,
                Difuntos = viewModel.Dto.Difuntos,
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
            var vm = new CambioTitularVM();
            vm.SweetAlert = TempData.GetSweetAlert();

            try
            {
                var strategy = _strategyFactory.GetCreateStrategy((int)TipoTramiteEnum.CambioTitular);

                if (strategy is ITramiteCreateStrategy<CambioTitularDTO> typedStrategy)
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

                return View("CambioTitular", vm);
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
