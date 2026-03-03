using CemSys3.DTOs.Nota;

namespace CemSys3.Interfaces.Notificaciones
{
    public interface INotificaciones
    {
        Task<NotificacionNotaDTO> NotificacionNotasPendientes();
    }
}
