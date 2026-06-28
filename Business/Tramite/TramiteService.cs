using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.Tramite;
using CemSys3.Enumerables;
using CemSys3.Interfaces.Tramite;
using CemSys3.Interfaces.TramitesConcesion;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.Tramite
{
    public class TramiteService : ITramite
    {
        private readonly AppDbContext _context;
        private readonly IRequisitos _requisitosService;
        public TramiteService(AppDbContext context, IRequisitos requisitos)
        {
            _context = context;
            _requisitosService = requisitos;
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

                case (int)TipoTramiteEnum.Cremacion:
                    Cremacione cremacion = await _context.Cremaciones.FindAsync(tramiteId) ?? throw new Exception("No se encontro la cremación");
                    cremacion.InfoAdicional = informacionAdicionalTramite;
                    await _context.SaveChangesAsync();

                    result.Message = "Información adicional actualizada correctamente";
                    result.Success = true;
                    result.Id = tramiteId;
                    break;

                case (int)TipoTramiteEnum.Reduccion:
                    Reduccione reduccion = await _context.Reducciones.FindAsync(tramiteId) ?? throw new Exception("No se encontro la reducción");
                    reduccion.InfoAdicional = informacionAdicionalTramite;
                    await _context.SaveChangesAsync();

                    result.Message = "Información adicional actualizada correctamente";
                    result.Success = true;
                    result.Id = tramiteId;
                    break;

                case (int)TipoTramiteEnum.Traslado:
                    Traslado traslado = await _context.Traslados.FindAsync(tramiteId) ?? throw new Exception("No se encontro el traslado");
                    traslado.InfoAdicional = informacionAdicionalTramite;
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

        public async Task<IEnumerable<TramiteDTO>> GetIniciadosYPendientes()
        {
            var tramites = new List<TramiteDTO>();


            var permisosRefacciones = await _context.PermisosRefacciones
              .AsNoTracking()
              .Where(p => p.FechaFinalizacion == null &&
                (
                    p.Tramite.EstadoActualId == (int)EstadosTramiteEnum.Iniciado ||
                    p.Tramite.EstadoActualId == (int)EstadosTramiteEnum.Pendiente
                ))
              .Select(p => new TramiteDTO
              {
                 Id = p.TramiteId,
                 FechaCreacion = p.FechaCreacion,
                 TipoTramiteId = p.Tramite.TipoTramiteId,
                 EstadoActualId = p.Tramite.EstadoActualId,
                  NroConcesion = p.Concesion != null && p.Concesion.ConcesioneTramite != null
                        ? p.Concesion.ConcesioneTramite.Concesion ?? 0
                        : 0
              })
              .ToListAsync();

            var cremaciones = await _context.Cremaciones
                .AsNoTracking()
                .Where(c => c.FechaFinalizacion == null &&
    (
        c.Tramite.EstadoActualId == (int)EstadosTramiteEnum.Iniciado ||
        c.Tramite.EstadoActualId == (int)EstadosTramiteEnum.Pendiente
    ))
                .Select(c => new TramiteDTO
                {
                    Id = c.TramiteId,
                    FechaCreacion = c.FechaCreacion,
                    TipoTramiteId = c.Tramite.TipoTramiteId,
                    EstadoActualId = c.Tramite.EstadoActualId,
                    NroConcesion = c.Concesion != null && c.Concesion.ConcesioneTramite != null
                        ? c.Concesion.ConcesioneTramite.Concesion ?? 0
                        : 0
                })
                .ToListAsync();

            var traslados = await _context.Traslados
                .AsNoTracking()
                .Where(t => t.FechaFinalizacion == null &&
    (
        t.Tramite.EstadoActualId == (int)EstadosTramiteEnum.Iniciado ||
        t.Tramite.EstadoActualId == (int)EstadosTramiteEnum.Pendiente
    ))
                .Select(t => new TramiteDTO
                {
                    Id = t.TramiteId,
                    FechaCreacion = t.FechaCreacion,
                    TipoTramiteId = t.Tramite.TipoTramiteId,
                    EstadoActualId = t.Tramite.EstadoActualId,
                    NroConcesion = t.Concesion != null && t.Concesion.ConcesioneTramite != null
                        ? t.Concesion.ConcesioneTramite.Concesion ?? 0
                        : 0
                })
                .ToListAsync();

            var reducciones = await _context.Reducciones
                .AsNoTracking()
                .Where(r => r.FechaFinalizacion == null && (r.Tramite.EstadoActualId == (int)EstadosTramiteEnum.Iniciado || r.Tramite.EstadoActualId == (int)EstadosTramiteEnum.Pendiente))
                .Select(r => new TramiteDTO
                {
                    Id = r.TramiteId,
                    FechaCreacion = r.FechaCreacion,
                    TipoTramiteId = r.Tramite.TipoTramiteId,
                    EstadoActualId = r.Tramite.EstadoActualId,
                    NroConcesion = r.Concesion != null && r.Concesion.ConcesioneTramite != null
                        ? r.Concesion.ConcesioneTramite.Concesion ?? 0
                        : 0
                })
                .ToListAsync();

            var ingresos = await _context.Introducciones
              .AsNoTracking()
              .Where(p => p.Tramite.EstadoActualId == (int)EstadosIngresoEnum.IngresoRegistrado)
              .Select(p => new TramiteDTO
              {
                  Id = p.TramiteId,
                  FechaCreacion = p.Tramite.FechaCreacion,
                  TipoTramiteId = p.Tramite.TipoTramiteId,
                  EstadoActualId = p.Tramite.EstadoActualId
              })
              .ToListAsync();

            var cambioTitular = await _context.CambiosTitularidads
              .AsNoTracking()
              .Where(p => p.Tramite.EstadoActualId == (int)EstadosTramiteEnum.Iniciado)
              .Select(p => new TramiteDTO
              {
                  Id = p.TramiteId,
                  FechaCreacion = p.FechaCreacion,
                  TipoTramiteId = p.Tramite.TipoTramiteId,
                  EstadoActualId = p.Tramite.EstadoActualId,
                  NroConcesion = p.Concesion != null && p.Concesion.ConcesioneTramite != null
                        ? p.Concesion.ConcesioneTramite.Concesion ?? 0
                        : 0
              })
              .ToListAsync();

            var aceptacion = await _context.AceptacionTitularidads
             .AsNoTracking()
             .Where(p => p.Tramite.EstadoActualId == (int)EstadosTramiteEnum.Iniciado)
             .Select(p => new TramiteDTO
             {
                 Id = p.TramiteId,
                 FechaCreacion = p.FechaCreacion,
                 TipoTramiteId = p.Tramite.TipoTramiteId,
                 EstadoActualId = p.Tramite.EstadoActualId,
                 NroConcesion = p.Concesion != null && p.Concesion.ConcesioneTramite != null
                        ? p.Concesion.ConcesioneTramite.Concesion ?? 0
                        : 0
             })
             .ToListAsync();

            var permisoIngreso = await _context.PermisosIngresos
             .AsNoTracking()
             .Where(p => p.Tramite.EstadoActualId == (int)EstadosTramiteEnum.Iniciado)
             .Select(p => new TramiteDTO
             {
                 Id = p.TramiteId,
                 FechaCreacion = p.FechaCreacion,
                 TipoTramiteId = p.Tramite.TipoTramiteId,
                 EstadoActualId = p.Tramite.EstadoActualId,
                 NroConcesion = p.Concesion != null && p.Concesion.ConcesioneTramite != null
                        ? p.Concesion.ConcesioneTramite.Concesion ?? 0
                        : 0
             })
             .ToListAsync();

            tramites.AddRange(cremaciones);
            tramites.AddRange(traslados);
            tramites.AddRange(reducciones); 
            tramites.AddRange(ingresos);
            tramites.AddRange(cambioTitular);
            tramites.AddRange(aceptacion);
            tramites.AddRange(permisoIngreso);
            tramites.AddRange(permisosRefacciones);

            return tramites.OrderByDescending(t => t.FechaCreacion);
        }

        public async Task<ListadoTramitesDeConcesionDTO> GetListadoTramitesDeConcesion(int concesionId)
        {
            ListadoTramitesDeConcesionDTO dto = new ListadoTramitesDeConcesionDTO();

            // Obtener parcelaId desde la concesión directamente, ya refleja traslados
            Models.Concesione concesion = await _context.Concesiones
                .FirstOrDefaultAsync(c => c.TramiteId == concesionId)
                ?? throw new Exception("Concesión no encontrada.");

            IEnumerable<Models.Tramite> tramites = await _context.TramitesParcelas.OrderByDescending(d=>d.FechaRegistro)
                .Where(tp => tp.ParcelaId == concesion.ParcelaId)
                .Select(tp => tp.Tramite)
                .ToListAsync();

            

            int[] tiposNoPermitidos = new[] { (int)TipoTramiteEnum.Nota, (int)TipoTramiteEnum.Ingreso, (int)TipoTramiteEnum.ContratoConcesion };

            var tramitesFiltrados = tramites
                .Where(t => !tiposNoPermitidos.Contains(t.TipoTramiteId)) 
                .ToList();

            dto.Requisitos = await _requisitosService.GetAll(concesionId);

            dto.ConcesionId = concesionId;
            dto.ParcelaId = concesion.ParcelaId;

            dto.TramitesIniciados = tramitesFiltrados.Select(t => new TramiteDTO
            {
                Id = t.Id,
                Visibilidad = t.Visibilidad,
                FechaCreacion = t.FechaCreacion,
                TipoTramiteId = t.TipoTramiteId,
                UsuarioId = t.UsuarioId,
                EstadoActualId = t.EstadoActualId
            }).ToList();

            //consultar el difuntos relacionados a la parcela para el tramite
            dto.Difuntos = await _context.ParcelaDifuntos
                .Where(p => p.ParcelaId == dto.ParcelaId && p.FechaRetiro == null)
                .Select(p => new DifuntoContratoDTO
                {
                    Id = p.Difunto.Id,
                    DNI = p.Difunto.Dni,
                    Nombre = p.Difunto.Nombre,
                    Apellido = p.Difunto.Apellido,
                    FechaIngreso = p.FechaIngreso,
                    EstadoDifuntoId = p.Difunto.EstadoDifuntoId
                }).ToListAsync();


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
