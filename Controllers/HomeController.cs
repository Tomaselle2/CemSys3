using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Concesion;
using CemSys3.Interfaces.Notificaciones;
using CemSys3.Models;
using CemSys3.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CemSys3.Controllers
{
    public class HomeController : Controller
    {
        private readonly INotificaciones _notificacionesService;
        private readonly IActualizarConcesionesAutomaticas _concesionesAuto;

        public HomeController(INotificaciones notificaciones, IActualizarConcesionesAutomaticas concesionesAuto)
        {
            _notificacionesService = notificaciones;
            _concesionesAuto = concesionesAuto;
        }

        [AuthorizeRole(RolUsuario.Administrador, RolUsuario.Empleado)]
        public async Task<IActionResult> Index()
        {
            NotificacionHomeVM viewModel = new NotificacionHomeVM();

            try
            {
                int cantidadConcesiones = await _concesionesAuto.ActualizarEstadoConcesionesAsync();
                string mensajeConcesiones = cantidadConcesiones > 0
                    ? $"Se actualizaron {cantidadConcesiones} concesiones automáticamente."
                    : "";

                viewModel.NotificacionNota = await _notificacionesService.NotificacionNotasPendientes();

                bool hayNotasPendientes = viewModel.NotificacionNota.CantidadNotasIngresoPendientes != 0
                    || viewModel.NotificacionNota.CantidadNotasRecordatorioPendientes != 0;

                if (hayNotasPendientes)
                {
                    viewModel.SweetAlert = new SweetAlertDTO
                    {
                        Titulo = "¡Tienes notas pendientes!",
                        Mensaje = $"Tienes {viewModel.NotificacionNota.CantidadNotasIngresoPendientes} notas de ingreso pendientes.\n " +
                            $"Tienes {viewModel.NotificacionNota.CantidadNotasRecordatorioPendientes} notas de recordatorio pendientes.\n " +
                            $"{mensajeConcesiones}",
                        Tipo = "warning"
                    };
                }
                else if (cantidadConcesiones > 0)
                {
                    viewModel.SweetAlert = new SweetAlertDTO
                    {
                        Titulo = "Actualización automática",
                        Mensaje = mensajeConcesiones,
                        Tipo = "success"
                    };
                }
            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error al cargar las notificaciones",
                    Mensaje = ex.Message,
                    Tipo = "error"
                };
            }

            return View(viewModel);
        }
    }
}

