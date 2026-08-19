using CemSys3.DTOs.Reportes;

namespace CemSys3.Interfaces.Reportes
{
    public interface IReporteProblemasService
    {
        // Concesiones vigentes (no caducadas) que no tienen ningún difunto activo
        Task<List<ReporteConcesionSinDifuntoDTO>> GetConcesionesSinDifuntos();

        // Concesiones donde el titular activo es, a la vez, un difunto activo en esa misma parcela
        Task<List<ReporteTitularEsDifuntoDTO>> GetConcesionesConTitularFallecido();

        // Parcelas con más de una concesión vigente simultáneamente
        Task<List<ReporteParcelaConMultiplesConcesionesDTO>> GetParcelasConMultiplesConcesiones();
    }
}
