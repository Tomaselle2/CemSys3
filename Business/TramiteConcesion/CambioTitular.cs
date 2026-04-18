using CemSys3.DTOs.HistorialEstado;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.Tramite;
using CemSys3.DTOs.TramitesConcesion;
using CemSys3.Enumerables;
using CemSys3.Interfaces.HistorialEstados;
using CemSys3.Interfaces.Tramite;
using CemSys3.Interfaces.TramitesConcesion;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.TramiteConcesion
{
    public class CambioTitular : ICambioTitular
    {
        private readonly AppDbContext _context;
        public readonly IHistorialEstados _historialEstadosService;
        public readonly ITramite _tramiteService;

        public CambioTitular(AppDbContext context, ITramite tramiteService, IHistorialEstados historialEstadosService)
        {
            _context = context;
            _tramiteService = tramiteService;
            _historialEstadosService = historialEstadosService;
        }

        public async Task<CambioTitularDTO> AddCambioTitular(int tramiteConcesionId, int usuarioId) //get genera un tramite de cambio de titular
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                Models.Concesione concesion = await _context.Concesiones.AsNoTracking()
                   .Include(c => c.Tramite)
                   .Include(c => c.Parcela)
                       .ThenInclude(p => p.Seccion)
                   .FirstOrDefaultAsync(c => c.TramiteId == tramiteConcesionId) ?? throw new Exception("Concesion no encontrada para inicar el trámite.");

                //1- registrar tramite
                TramiteDTO tramite = new TramiteDTO
                {
                    Visibilidad = true,
                    FechaCreacion = DateTime.Now,
                    TipoTramiteId = (int)TipoTramiteEnum.CambioTitular,
                    UsuarioId = usuarioId,
                    EstadoActualId = (int)EstadosCambioTitularEnum.Iniciado
                };

                int tramiteId = await _tramiteService.Add(tramite);
                await _context.SaveChangesAsync(); //guardar cambios antes de continuar

                //2- registrar Historial del tramite
                HistorialEstadosDTO historial = new HistorialEstadosDTO
                {
                    Fecha = tramite.FechaCreacion,
                    TramiteId = tramiteId,
                    EstadoTramiteId = (int)EstadosCambioTitularEnum.Iniciado
                };
                await _historialEstadosService.Add(historial);

                //3- registrar el tramite de cambio de titularidad
                Models.CambiosTitularidad cambiosTitularidad = new Models.CambiosTitularidad
                {
                    TramiteId = tramiteId,
                    ParcelaId = concesion.ParcelaId,
                    UsuarioId = usuarioId,
                    FechaCreacion = DateTime.Now,
                    InfoAdicional = string.Empty,
                    Visibilidad = true
                };
                await _context.CambiosTitularidads.AddAsync(cambiosTitularidad);

                //4 - relacion de tramite con parcela
                await _historialEstadosService.VincularTramiteAParcela(tramiteId, concesion.ParcelaId);



                CambioTitularDTO dto = new CambioTitularDTO();
                dto.TramiteId = tramiteId;
                dto.EstadoTramiteId = tramite.EstadoActualId;
                dto.ParcelaId = concesion.ParcelaId;
                dto.TipoParcela = concesion.TipoParcela;
                dto.NombreSeccion = concesion.Parcela.Seccion.Nombre;
                dto.NroParcela = concesion.Parcela.NroParcela;
                dto.NroFila = concesion.Parcela.NroFila;
                dto.NroConcesion = concesion.Concesion;

                //Traer Titulares en una sola consulta
                dto.TitularesActuales = await _context.HistorialTitularesConcesiones
                    .Where(h => h.ConcesionId == tramiteConcesionId && h.FechaFin == null)
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

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return dto;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

        public Task<int> CambioTitularPost(CambioTitularDTO dto) //post
        {
            throw new NotImplementedException();
        }

        public async Task<CambioTitularDTO> Get(int cambioTitularId)
        {

            Models.CambiosTitularidad cambioTitularidad = await _context.CambiosTitularidads.AsNoTracking()
                .Include(t => t.Tramite)
                .FirstOrDefaultAsync(ct => ct.TramiteId == cambioTitularId) ?? throw new Exception("Trámite de cambio de titularidad no encontrado.");

            Models.Concesione concesion = await _context.Concesiones.AsNoTracking()
                   .Include(c => c.Tramite)
                   .Include(c => c.Parcela)
                       .ThenInclude(p => p.Seccion)
                   .FirstOrDefaultAsync(c => c.TramiteId == cambioTitularidad.ConcesionId) ?? throw new Exception("Concesion no encontrada para inicar el trámite.");

            CambioTitularDTO dto = new CambioTitularDTO();
            dto.TramiteId = cambioTitularidad.TramiteId;
            dto.EstadoTramiteId = cambioTitularidad.Tramite.EstadoActualId;
            dto.ParcelaId = cambioTitularidad.ParcelaId;
            dto.TipoParcela = concesion.TipoParcela;
            dto.NombreSeccion = concesion.Parcela.Seccion.Nombre;
            dto.NroParcela = concesion.Parcela.NroParcela;
            dto.NroFila = concesion.Parcela.NroFila;
            dto.NroConcesion = concesion.Concesion;
            dto.ConcesionId = concesion.TramiteId;

            dto.TitularesActuales = await _context.HistorialTitularesConcesiones
                    .Where(h => h.ConcesionId == cambioTitularidad.ConcesionId && h.FechaFin == null)
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

            dto.NuevosTitulares = await _context.DocumentosTramites.Where(t => t.TramiteId == cambioTitularidad.TramiteId).Select(h=> new TitularesContratoDTO
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

            //consultar los difuntos relacionados a la parcela
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

    }
}

