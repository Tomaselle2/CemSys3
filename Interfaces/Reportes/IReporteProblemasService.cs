using CemSys3.DTOs.Reportes;

namespace CemSys3.Interfaces.Reportes
{
    public interface IReporteProblemasService
    {
        // Concesiones vigentes (no caducadas) que no tienen ningún difunto activo
        Task<List<ReporteConcesionSinDifuntoDTO>> GetConcesionesSinDifuntos();
    }
}
