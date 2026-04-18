using CemSys3.DTOs.PlantillaTramite;

namespace CemSys3.Interfaces.PlantillaTramite
{
    public interface IDocumentoTramiteService
    {
        Task<int> CrearDesdePlantillaAsync(
        int plantillaId,
        int tramiteId,
        int usuarioId,
        int? personaId,
        string? parentesco,
        Dictionary<string, string> variables);

        Task ActualizarAsync(DocumentoDTO dto);


        Task<List<DocumentoDTO>> ObtenerPorTramiteAsync(int tramiteId);
        Task<DocumentoDTO> ObtenerDocumentoPorId(int id);
        Task Delete(int id);


    }
}
