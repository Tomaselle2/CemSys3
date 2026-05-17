using CemSys3.DTOs.Arbol;

namespace CemSys3.Interfaces
{
    public interface IArbol
    {
        Task<ArbolDTO> ObtenerDiagrama(int tramiteId);
        Task GuardarDiagrama(ArbolDTO dto);
    }
}
