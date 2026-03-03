using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Roles_Autenticacion;
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

        public HomeController(INotificaciones notificaciones)
        {
            _notificacionesService = notificaciones;
        }

        [AuthorizeRole(RolUsuario.Administrador, RolUsuario.Empleado)]
        public async Task<IActionResult> Index()
        {
            NotificacionHomeVM viewModel = new NotificacionHomeVM();

            try
            {
                viewModel.NotificacionNota = await _notificacionesService.NotificacionNotasPendientes();
                if (viewModel.NotificacionNota.CantidadNotasIngresoPendientes != 0 || viewModel.NotificacionNota.CantidadNotasRecordatorioPendientes != 0)
                {
                    viewModel.SweetAlert = new SweetAlertDTO
                    {
                        Titulo = "¡Tienes notas pendientes!",
                        Mensaje = $"Tienes {viewModel.NotificacionNota.CantidadNotasIngresoPendientes} notas de ingreso pendientes y {viewModel.NotificacionNota.CantidadNotasRecordatorioPendientes} notas de recordatorio pendientes.",
                        Tipo = "warning"
                    };
                }
            }
            catch (Exception ex) {
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
