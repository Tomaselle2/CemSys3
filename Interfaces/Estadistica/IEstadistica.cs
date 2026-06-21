using CemSys3.DTOs.Estadistica;

namespace CemSys3.Interfaces.Estadistica
{
    public interface IEstadistica
    {
        Task<EstadisticasDTO> GetEstadisticasGenerales();
    }
}
