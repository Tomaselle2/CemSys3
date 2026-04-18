using CemSys3.DTOs.PlantillaTramite;
using CemSys3.Interfaces.PlantillaTramite;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.PlantillaTramite
{
    public class DocumentoTramiteService : IDocumentoTramiteService
    {
        private readonly AppDbContext _context;
        private readonly ITemplateProcessor _processor;

        public DocumentoTramiteService(AppDbContext context, ITemplateProcessor processor)
        {
            _context = context;
            _processor = processor;
        }

        public async Task ActualizarAsync(DocumentoDTO dto)
        {
            var doc = await _context.DocumentosTramites.FindAsync(dto.Id) ?? throw new Exception("Documento no encontrado");

            doc.ContenidoHtml = dto.ContenidoHtml;
            doc.FechaUltimaEdicion = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task<int> CrearDesdePlantillaAsync(
        int plantillaId,
        int tramiteId,
        int usuarioId,
        int? personaId,
        string? parentesco,
        Dictionary<string, string> variables)
        {
            var plantilla = await _context.PlantillasTramites
                .FirstOrDefaultAsync(x => x.Id == plantillaId);

            if (plantilla == null)
                throw new Exception("Plantilla no encontrada");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var contenidoProcesado = _processor.Procesar(plantilla.Contenido, variables);

                var documento = new DocumentosTramite
                {
                    TramiteId = tramiteId,
                    PlantillaId = plantillaId,
                    TipoAutorizacionId = plantilla.TipoAutorizacionId ?? 0,
                    Nombre = plantilla.Nombre,
                    ContenidoHtml = contenidoProcesado,
                    UsuarioId = usuarioId,
                    PersonaId = personaId,
                    Parentesco = parentesco,
                    Version = 1,
                    FechaUltimaEdicion = DateTime.Now,
                    Visibilidad = true
                };

                _context.DocumentosTramites.Add(documento);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return documento.Id;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;

            }
        }

        public async Task Delete(int id)
        {
            Models.DocumentosTramite doc = await _context.DocumentosTramites.FindAsync(id) ?? throw new Exception("Documento no encontrado");

            _context.DocumentosTramites.Remove(doc);

            await _context.SaveChangesAsync();            
        }

        public async Task<DocumentoDTO> ObtenerDocumentoPorId(int id)
        {
            Models.DocumentosTramite doc = await _context.DocumentosTramites.FindAsync(id) ?? throw new Exception("Documento no encontrado");

            return new DocumentoDTO
            {
                Id = doc.Id,
                TramiteId = doc.TramiteId,
                TipoAutorizacionId = doc.TipoAutorizacionId ?? 0,
                Nombre = doc.Nombre,
                ContenidoHtml = doc.ContenidoHtml,
                PersonaId = doc.PersonaId,
                Parentesco = doc.Parentesco
            };
        }

        public async Task<List<DocumentoDTO>> ObtenerPorTramiteAsync(int tramiteId)
        {
            return await _context.DocumentosTramites
                .Where(x => x.TramiteId == tramiteId && x.Visibilidad == true)
                .Select(x => new DocumentoDTO
                {
                    Id = x.Id,
                    TramiteId = x.TramiteId,
                    TipoAutorizacionId = x.TipoAutorizacionId ?? 0,
                    Nombre = x.Nombre,
                    ContenidoHtml = x.ContenidoHtml,
                    PersonaId = x.PersonaId,
                    Parentesco = x.Parentesco
                })
                .ToListAsync();
        }
    }
}
