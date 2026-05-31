using CemSys3.DTOs.PlantillaTramite;

namespace CemSys3.Interfaces.TramitesConcesion
{
    public interface ITramiteStrategy
    {
        //genera la plantilla clone y reemplaza las variables por los datos del tramite, luego guarda el documento generado
        Task GenerarDocumentosAsync(GeneraStrategyDTO dto);
        Task FinalizarAsync(int tramiteId, int usuarioId);
        Task<int> AvanzarEstadoAsync(int tramiteId, int nuevoEstado, int usuarioId);
    }
}

