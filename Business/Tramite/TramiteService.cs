using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Tramite;
using CemSys3.Enumerables;
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

        public async Task<GenericResultDTO> ActualizarInfoAdicional(int tramiteId, string informacionAdicionalTramite)
        {
            Models.Tramite tramite = await _context.Tramites.FindAsync(tramiteId) ?? throw new Exception("No se encontro el trámite");

            GenericResultDTO result = new GenericResultDTO
            {
                Message = "No se pudo actualizar la información adicional",
                Success = false,
                Id = null
            };

            switch (tramite.TipoTramiteId)
            {
                case (int)TipoTramiteEnum.Ingreso:
                    Introduccione ingreso = await _context.Introducciones.FindAsync(tramiteId) ?? throw new Exception("No se encontro el ingreso");
                    ingreso.InformacionAdicional = informacionAdicionalTramite;
                    await _context.SaveChangesAsync();

                    result.Message = "Información adicional actualizada correctamente";
                    result.Success = true;
                    result.Id = tramiteId;
                    break;
                case (int)TipoTramiteEnum.ContratoConcesion:
                    Concesione concesion = await _context.Concesiones.FindAsync(tramiteId) ?? throw new Exception("No se encontro la concesión");
                    concesion.InformacionAdicional = informacionAdicionalTramite;
                    await _context.SaveChangesAsync();

                    result.Message = "Información adicional actualizada correctamente";
                    result.Success = true;
                    result.Id = tramiteId;
                    break;
            }

            return result;
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
        }

        public async Task<TramiteDTO> Get(int id)
        {
            Models.Tramite tramite = await _context.Tramites.FindAsync(id) ?? throw new Exception("No se encontro el trámite");

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

        public async Task<ListadoTramitesDeConcesionDTO> GetListadoTramitesDeConcesion(int concesionId)
        {
            ListadoTramitesDeConcesionDTO dto = new ListadoTramitesDeConcesionDTO();

            int parcelaId = await _context.TramitesParcelas
                .Where(tp => tp.TramiteId == concesionId)
                .Select(tp => tp.ParcelaId)
                .FirstOrDefaultAsync();

            IEnumerable<Models.Tramite> tramites = await _context.TramitesParcelas.OrderByDescending(d=>d.FechaRegistro)
                .Where(tp => tp.ParcelaId == parcelaId)
                .Select(tp => tp.Tramite)
                .ToListAsync();

            int[] tiposNoPermitidos = new[] { (int)TipoTramiteEnum.Nota, (int)TipoTramiteEnum.Ingreso, (int)TipoTramiteEnum.ContratoConcesion };

            var tramitesFiltrados = tramites
                .Where(t => !tiposNoPermitidos.Contains(t.TipoTramiteId)) 
                .ToList();

            dto.Requisitos = await _context.RequisitosTramites
                .Where(rt => rt.Activo == true)
                .Select(rt => new RequisitosTramiteDTO
                {
                    Id = rt.Id,
                    TipoTramiteId = rt.TipoTramiteId,
                    Descripcion = rt.Descripcion ?? ""
                })
                .ToListAsync();

            dto.ConcesionId = concesionId;
            dto.ParcelaId = parcelaId;

            dto.TramitesIniciados = tramitesFiltrados.Select(t => new TramiteDTO
            {
                Id = t.Id,
                Visibilidad = t.Visibilidad,
                FechaCreacion = t.FechaCreacion,
                TipoTramiteId = t.TipoTramiteId,
                UsuarioId = t.UsuarioId,
                EstadoActualId = t.EstadoActualId
            }).ToList();


          return dto;
        }



        public async Task Update(TramiteDTO dto)
        {
            Models.Tramite tramite = await _context.Tramites.FindAsync(dto.Id) ?? throw new Exception("No se encontro el trámite");

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
