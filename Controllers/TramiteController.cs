using CemSys3.DTOs.Generics;
using CemSys3.Enumerables;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Tramite;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class TramiteController : Controller
    {
        private readonly ITramite _tramiteService;
        public TramiteController(ITramite tramiteService)
        {
            _tramiteService = tramiteService;
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
    }
}
