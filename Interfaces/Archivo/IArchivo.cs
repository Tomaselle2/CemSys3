using CemSys3.DTOs.Archivo;
using CemSys3.DTOs.Generics;

namespace CemSys3.Interfaces.Archivo
{
    public interface IArchivo
    {
        Task<IEnumerable<ArchivoDTO>> GetAllByTramiteId(int tramiteId);
        Task Add(ArchivoDTO dto);
        Task AddDesdeBytes(ArchivoDTO dto);
        Task Update(ArchivoDTO dto);
        Task Delete(Guid archivoId);
        Task<ArchivoDTO> Get(Guid archivoId);

        Task<IEnumerable<ArchivoDTO>> GetDocumentacionSistema();
    }
}
