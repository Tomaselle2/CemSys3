namespace CemSys3.Interfaces.TramitesConcesion
{
    public interface IComplementoTramite<TResponseDto>
    {
        Task UpdateValores(TResponseDto dto);
    }
}
