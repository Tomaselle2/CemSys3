using CemSys3.DTOs.TramitesConcesion;

namespace CemSys3.Interfaces.TramitesConcesion
{
    public interface ITramiteCreateStrategy
    {
        Task<int> CrearAsync(CrearTramiteDTO dto);
    }

    public interface ITramiteCreateStrategy<TResponseDto> : ITramiteCreateStrategy
    {
        Task<TResponseDto> ObtenerAsync(int tramiteId);
    }
}
