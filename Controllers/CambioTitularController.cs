using CemSys3.Business.PlantillaTramite;
using CemSys3.DTOs.PlantillaTramite;
using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.TramiteConcesion;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Archivo;
using CemSys3.Interfaces.HistorialEstados;
using CemSys3.Interfaces.PlantillaTramite;
using CemSys3.Interfaces.TramiteConcesion;
using CemSys3.Models;
using CemSys3.ViewModels.TramiteConcesion;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class CambioTitularController : Controller
    {
        private readonly IPlantillaTramite _planillasService;
        private readonly ICambioTitular _cambioTitular;
        private readonly IArchivo _archivoService;
        private readonly IHistorialEstados _historialService;

        public CambioTitularController(IPlantillaTramite planillasService, ICambioTitular cambioTitular, IArchivo archivoService, IHistorialEstados historialService)
        {
            _planillasService = planillasService;
            _cambioTitular = cambioTitular;
            _archivoService = archivoService;
            _historialService = historialService;
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> CambioTitular(
            int? cambioTitularId,
            int? concesionId)
        {
            CambioTitularVM viewModel = new CambioTitularVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();

            try
            {
                viewModel.PlantillaTramite = await _planillasService.Get((int)PlantillasTramitesEnum.Cambio_Titular_Ambos_Presentes);

                int usuarioId = HttpContext.Session.GetInt32("IdUsuario") ?? 0;

                if (cambioTitularId.HasValue && concesionId.HasValue)
                {
                    //CONTINUAR trámite existente
                    viewModel.Dto = await _cambioTitular.Get(cambioTitularId.Value, concesionId.Value);
                    viewModel.Archivos = await _archivoService.GetAllByTramiteId(cambioTitularId.Value);
                    viewModel.Historial = await _historialService.GetAllById(cambioTitularId.Value);
                    viewModel.Plantillas = await _planillasService.GetByTipoTramite((int)TipoTramiteEnum.CambioTitular);
                }
                else if (concesionId.HasValue)
                {
                    //INICIAR nuevo trámite
                    CambioTitularDTO Dto = await _cambioTitular.AddCambioTitular(concesionId.Value, usuarioId);
                    return RedirectToAction("CambioTitular", new
                    {
                        cambioTitularId = Dto.TramiteId, // o el id correcto
                        concesionId = concesionId.Value
                    });
                }
                else
                {
                    throw new Exception("Parámetros inválidos.");
                }

                return View(viewModel);
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


        //[HttpPost]
        //[AuthorizeRole(RolUsuario.Empleado)]
        //public async Task<IActionResult> GenerarPlantilla([FromBody] PlantillaRequestDTO request)
        //{
        //    var plantilla = await _planillasService.Get(request.PlantillaId);

        //    var builder = _factory.Get(request.TipoTramite);

        //    var variables = builder.Build(request.Data);

        //    var html = _planillasService.Render(plantilla.Contenido, variables);

        //    return Json(new
        //    {
        //        success = true,
        //        contenido = html
        //    });
        //}

        //[HttpPost]
        //public IActionResult GenerarPlantilla([FromBody] GenerarPlantillaRequest request)
        //{
        //    try
        //    {
        //        var builder = _factory.Get(request.TipoTramite);
        //        var contenido = builder.Build(request.Data, request.PlantillaId);

        //        return Json(new
        //        {
        //            success = true,
        //            contenido = contenido
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new
        //        {
        //            success = false,
        //            message = ex.Message
        //        });
        //    }
        //}


    }

    //public class GenerarPlantillaRequest
    //{
    //    public int PlantillaId { get; set; }
    //    public string TipoTramite { get; set; }
    //    public object Data { get; set; }
    //}
}
