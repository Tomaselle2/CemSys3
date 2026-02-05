using CemSys3.Enumerables;
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

                    //case (int)TipoTramiteEnum.ContratoConcesion:
                    //    var contrato = await _concesionesBusiness.ConsultarContratoConcesion(tramiteId);
                    //    return RedirectToAction("ContratoIniciado", "ContratoConcesion", new { nroConcesion = contrato.Concesion, parcelaId = contrato.ParcelaId });

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
    }
}
