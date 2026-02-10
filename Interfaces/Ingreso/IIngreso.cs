using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Ingreso;

namespace CemSys3.Interfaces.Ingreso
{
    public interface IIngreso
    {
        Task<GenericResultDTO> Add(IngresoDTO dto);
        Task<ResumenIngresoDTO> Get(int ingresoId);
        Task FinalizarIngreso (int ingresoId, string cobroIngreso);
    }
}
