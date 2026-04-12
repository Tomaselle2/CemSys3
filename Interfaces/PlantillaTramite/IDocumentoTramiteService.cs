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

        Task ActualizarContenidoAsync(int id, string contenidoHtml, int usuarioId);


        Task<List<DocumentoDTO>> ObtenerPorTramiteAsync(int tramiteId);
    }
}
