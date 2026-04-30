using CemSys3.DTOs.Persona;
using CemSys3.DTOs.Tramite;
using CemSys3.Interfaces.PlantillaTramite;
using CemSys3.Interfaces.TramitesConcesion;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;
namespace CemSys3.Business.TramiteConcesion
{
    public class RequisitosService : IRequisitos
    {
        private readonly AppDbContext _context;
        private readonly ITemplateProcessor templateProcessor;

        public RequisitosService(AppDbContext context, ITemplateProcessor template)
        {
            _context = context;
            templateProcessor = template;
        }
        public async Task<IEnumerable<RequisitosTramiteDTO>> GetAll(int concesionId)
        {
            Models.Concesione concesion = await _context.Concesiones.AsNoTracking()
                   .Include(c => c.Tramite)
                   .Include(c => c.Parcela)
                       .ThenInclude(p => p.Seccion)
                   .FirstOrDefaultAsync(c => c.TramiteId == concesionId) ?? throw new Exception("Concesion no encontrada");

            IEnumerable<TitularesContratoDTO> titulares = await _context.HistorialTitularesConcesiones
                    .Where(h => h.ConcesionId == concesionId && h.FechaFin == null)
                    .Select(h => new TitularesContratoDTO
                    {
                        Id = h.Persona.Id,
                        Dni = h.Persona.Dni,
                        Nombre = h.Persona.Nombre,
                        Apellido = h.Persona.Apellido,
                        Sexo = h.Persona.Sexo,
                        Celular = h.Persona.Celular,
                        CorreoElectronico = h.Persona.Correo,
                        Domicilio = h.Persona.Domicilio
                    }).ToListAsync();

            List<Models.RequisitosTramite> requisitos = await _context.RequisitosTramites
                .Where(rt => rt.Activo == true).ToListAsync();

            foreach (var requisito in requisitos)
            {
                var variables = new Dictionary<string, string>
                    {
                        { "TitularesActuales", string.Join(", ", titulares.Select(t => t.Apellido.ToUpper() + " " + t.Nombre.ToUpper())) },
                    };

                requisito.Descripcion = templateProcessor.Procesar(requisito.Descripcion ?? "", variables);
            }

                return requisitos.Select(rt => new RequisitosTramiteDTO
                {
                    Id = rt.Id,
                    TipoTramiteId = rt.TipoTramiteId,
                    Descripcion = rt.Descripcion ?? ""
                }).ToList();
        }

        public async Task<RequisitosTramiteDTO> GetByTipoTramiteId(int tipoTramiteId)
        {
            return await _context.RequisitosTramites
               .Where(rt => rt.Activo == true && rt.TipoTramiteId == tipoTramiteId)
               .Select(rt => new RequisitosTramiteDTO
               {
                   Id = rt.Id,
                   TipoTramiteId = rt.TipoTramiteId,
                   Descripcion = rt.Descripcion ?? ""
               })
               .FirstOrDefaultAsync();
        }

        public async Task Update(int tipoTramiteId, string descripcion)
        {
            Models.RequisitosTramite requisito = await _context.RequisitosTramites.Where(t => t.TipoTramiteId == tipoTramiteId).FirstOrDefaultAsync() ?? throw new Exception("No se encontro el requisito");

            requisito.Descripcion = descripcion;

            await _context.SaveChangesAsync();
        }
    }
}
