using CemSys3.DTOs.TramitesConcesion;
using CemSys3.Interfaces.TramitesConcesion;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.TramiteConcesion
{
    public class FirmantesService : IFirmantes
    {
        private readonly AppDbContext _context;

        public FirmantesService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<FirmantesDTO>> GetAllByTramite(int tramiteId)
        {
            List<FirmantesDTO> firmantes = await _context.FirmantesTramites
               .Where(f => f.TramiteId == tramiteId)
               .Include(p => p.Persona)
               .Select(f => new FirmantesDTO
               {
                   Id = f.Id,
                   TramiteId = f.TramiteId,
                   PersonaId = f.PersonaId,
                   Parentesco = f.Parentesco,
                   EsTitular = f.EsTitular,
                   FechaAlta = f.FechaAlta,
                   Visibilidad = f.Visibilidad,
                   Dni = f.Persona.Dni,
                   Nombre = f.Persona.Nombre,
                   Apellido = f.Persona.Apellido,
                   Sexo = f.Persona.Sexo,
                   Domicilio = f.Persona.Domicilio,
                   Celular = f.Persona.Celular,
                   CorreoElectronico = f.Persona.Correo
               }).ToListAsync();

            return firmantes;
        }

        public async Task Add(int tramiteId, int personaId, string parentesco, bool titular = false)
        {
            Models.FirmantesTramite firmante = new Models.FirmantesTramite
            {
                TramiteId = tramiteId,
                PersonaId = personaId,
                Parentesco = parentesco,
                EsTitular = titular,
                FechaAlta = DateTime.Now,
                Visibilidad = true
            };

            await _context.FirmantesTramites.AddAsync(firmante);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(int firmanteId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                //eliminar firmante
                FirmantesTramite firmante = await _context.FirmantesTramites.FirstOrDefaultAsync(f => f.Id == firmanteId) ?? throw new Exception("Firmante no encontrado");

                _context.FirmantesTramites.Remove(firmante);

                //eliminar documentos relacionados al firmante
                var documentos = await _context.DocumentosTramites.Where(d => d.FirmanteId == firmanteId).ToListAsync();
                _context.DocumentosTramites.RemoveRange(documentos);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
