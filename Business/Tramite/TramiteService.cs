using CemSys3.DTOs.Tramite;
using CemSys3.Interfaces.Tramite;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.Tramite
{
    public class TramiteService : ITramite
    {
        private readonly AppDbContext _context;
        
        public TramiteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Add(TramiteDTO dto)
        {
           
            Models.Tramite tramite = new Models.Tramite
            {
                Id = await ObtenerProximoIdTramite(),
                Visibilidad = true,
                FechaCreacion = dto.FechaCreacion,
                TipoTramiteId = dto.TipoTramiteId,
                UsuarioId = dto.UsuarioId,
                EstadoActualId = dto.EstadoActualId
            };

            //se guarda el trámite
            
            await _context.Tramites.AddAsync(tramite);

            return tramite.Id;

            //el historial depende de cada negocio
        }

        public async Task<TramiteDTO> Get(int id)
        {
            Models.Tramite tramite = await _context.Tramites.FindAsync(id) ?? throw new Exception("NO se encontro el trámite");

            TramiteDTO dto = new TramiteDTO
            {
                Id = tramite.Id,
                Visibilidad = tramite.Visibilidad,
                FechaCreacion = tramite.FechaCreacion,
                TipoTramiteId = tramite.TipoTramiteId,
                UsuarioId = tramite.UsuarioId,
                EstadoActualId = tramite.EstadoActualId
            };

            return dto;
        }

        public async Task Update(TramiteDTO dto)
        {
            Models.Tramite tramite = await _context.Tramites.FindAsync(dto.Id) ?? throw new Exception("NO se encontro el trámite");

            tramite.EstadoActualId = dto.EstadoActualId;

            _context.Update(tramite);
        }

        private async Task<int> ObtenerProximoIdTramite()
        {
            int? maxId = await _context.Tramites.MaxAsync(t => (int?)t.Id);
            return (maxId ?? 0) + 1;
        }
    }
}
