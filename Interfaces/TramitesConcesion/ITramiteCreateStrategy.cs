namespace CemSys3.Interfaces.TramitesConcesion
{
    public interface ITramiteCreateStrategy<TCreateDto, TResponseDto>
    {
        Task<int> CrearAsync(TCreateDto dto);
        Task<TResponseDto> ObtenerAsync(int tramiteId);
    }
}
