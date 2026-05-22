using CemSys3.DTOs.PlantillaTramite;
using CemSys3.Interfaces.PlantillaTramite;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.PlantillaTramite
{
    public class PlantillaTramiteService : IPlantillaTramite
    {
        private readonly AppDbContext _context;

        public PlantillaTramiteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> ActualizarAsync(PlantillaTramiteDTO dto)
        {
            var entity = await _context.PlantillasTramites
                .FirstOrDefaultAsync(x => x.Id == dto.PlantillaId);

            if (entity == null)
                throw new Exception("Plantilla no encontrada");

            entity.Nombre = dto.Nombre;
            entity.Contenido = dto.Contenido;
            entity.TipoAutorizacionId = dto.TipoAutorizacionId;
            entity.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<int> CrearAsync(PlantillaTramiteDTO dto)
        {
            var entity = new PlantillasTramite
            {
                TipoTramiteId = dto.TipoTramiteId,
                TipoAutorizacionId = dto.TipoAutorizacionId,
                Nombre = dto.Nombre,
                Contenido = dto.Contenido,
                Activo = true,
                FechaModificacion = DateTime.Now
            };

            _context.PlantillasTramites.Add(entity);
            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task EliminarAsync(int id)
        {
            var entity = await _context.PlantillasTramites
            .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return;

            entity.Activo = false;

            await _context.SaveChangesAsync();
        }

        public async Task<PlantillaTramiteDTO?> ObtenerPorIdAsync(int id)
        {
            return await _context.PlantillasTramites
             .Where(x => x.Id == id)
             .Select(x => new PlantillaTramiteDTO
             {
                 PlantillaId = x.Id,
                 Nombre = x.Nombre,
                 Contenido = x.Contenido,
                 TipoTramiteId = x.TipoTramiteId,
                 TipoAutorizacionId = x.TipoAutorizacionId ?? 0
             })
             .FirstOrDefaultAsync();
        }

        public async Task<PlantillaTramiteDTO?> ObtenerPorTipoAutorizacionIdAsync(int tipoAutorizacionId)
        {
            return await _context.PlantillasTramites
             .Where(x => x.TipoAutorizacionId == tipoAutorizacionId)
             .Select(x => new PlantillaTramiteDTO
             {
                 PlantillaId = x.Id,
                 Nombre = x.Nombre,
                 Contenido = x.Contenido,
                 TipoTramiteId = x.TipoTramiteId,
                 TipoAutorizacionId = x.TipoAutorizacionId ?? 0
             })
             .FirstOrDefaultAsync();
        }

        public async Task<List<PlantillaTramiteDTO>> ObtenerPorTipoTramiteAsync(int tipoTramiteId)
        {
            return await _context.PlantillasTramites
                .Where(x => x.TipoTramiteId == tipoTramiteId && x.Activo == true)
                .Select(x => new PlantillaTramiteDTO
                {
                    PlantillaId = x.Id,
                    TipoTramiteId = x.TipoTramiteId,
                    TipoAutorizacionId = x.TipoAutorizacionId ?? 0,
                    Nombre = x.Nombre,
                    Contenido = x.Contenido
                })
                .ToListAsync();
        }

    }
}
