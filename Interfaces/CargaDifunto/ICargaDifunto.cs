using CemSys3.DTOs.CargaDifunto;
using CemSys3.DTOs.Generics;

namespace CemSys3.Interfaces.CargaDifunto
{
    public interface ICargaDifunto
    {
        Task<GenericResultDTO> Add(CargaDifuntoDTO dto);
    }
}
