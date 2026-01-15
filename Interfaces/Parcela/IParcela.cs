using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Seccion;

namespace CemSys3.Interfaces.Parcela
{
    public interface IParcela
    {
        Task Add (SeccionRequestDTO dto);

        //para agregar una parcela a una seccion de fosa o panteon
        Task<GenericResultDTO> AddOne(int secccionId);
    }
}
