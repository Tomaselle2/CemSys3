using CemSys3.DTOs.Generics;
using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Tramite;
using CemSys3.Interfaces.TramitesConcesion;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class TramiteController : Controller
    {
        private readonly ITramite _tramiteService;
        private readonly ICancelarTramite _cancelarService;
        public TramiteController(ITramite tramiteService, ICancelarTramite cancelarService)
        {
            _tramiteService = tramiteService;
            _cancelarService = cancelarService;
        }

        public async Task<IActionResult> IrATramite(int tramiteId)
        {
            try
            {
                var tramite = await _tramiteService.Get(tramiteId);

                switch (tramite.TipoTramiteId)
                {
                    case (int)TipoTramiteEnum.Ingreso:
                        return RedirectToAction("ResumenIngreso", "Ingreso", new { ingresoId = tramiteId }); //tramite ya registrado como ingreso

                    case (int)TipoTramiteEnum.ContratoConcesion:
                        return RedirectToAction("Concesion", "Concesion", new { tramiteId = tramiteId });

                    case (int)TipoTramiteEnum.CambioTitular:
                        return RedirectToAction("Detalle", "CambioTitular", new { tramiteId = tramiteId});

                    case (int)TipoTramiteEnum.AceptacionTitular:
                        return RedirectToAction("Detalle", "AceptacionTitular", new { tramiteId = tramiteId });

                    case (int)TipoTramiteEnum.Cremacion:
                        return RedirectToAction("Detalle", "Cremacion", new { tramiteId = tramiteId });

                    case (int)TipoTramiteEnum.Traslado:
                        return RedirectToAction("Detalle", "Traslado", new { tramiteId = tramiteId });

                    case (int)TipoTramiteEnum.Reduccion:
                        return RedirectToAction("Detalle", "Reduccion", new { tramiteId = tramiteId });

                    case (int)TipoTramiteEnum.PermisoIngreso:
                        return RedirectToAction("Detalle", "PermisoIngreso", new { tramiteId = tramiteId });

                    default:
                        return Content($$"""
                <script>
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'Trámite no encontrado'
                    });
                </script>
            """);
                }

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

        [HttpPost]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> ActualizarInfoAdicionalTramite(
           int tramiteId,
           string informacionAdicionaTramite)
        {
            try
            {
                GenericResultDTO result = await _tramiteService.ActualizarInfoAdicional(
                tramiteId,
                informacionAdicionaTramite);

                if (!result.Success)
                {
                    return Content($"""
                        <script>
                            AlertService.show('Error', '{result.Message}', 'error');
                        </script>
                    """, "text/html");
                }

                return Content("""
                    <script>
                        AlertService.show('Éxito', 'Información adicional actualizada', 'success');
                        document.getElementById('btnActualizar')?.setAttribute('disabled', true);
                    </script>
                """, "text/html");
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
        public async Task<IActionResult> Cancelar(int tramiteId, int concesionId)
        {
            try
            {
                await _cancelarService.CancelarTramite(tramiteId);

                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Éxito",
                    Mensaje = "Trámite cancelado correctamente",
                    Tipo = "success"
                });

            }
            catch (Exception ex)
            {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "El trámite no se ha podido cancelar. " + ex.Message,
                    Tipo = "error"
                });

            }

            return RedirectToAction("Concesion", "Concesion", new { tramiteId = concesionId });

        }
    }
}
