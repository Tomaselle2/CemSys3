using CemSys3.DTOs.Persona;
using CemSys3.DTOs.Tramite;
using CemSys3.Interfaces.Persona;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace CemSys3.Business.Persona
{
    public class PersonaService : IPersona
    {
        private readonly AppDbContext _context;
        public PersonaService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<int> Add(PersonaDTO dto)
        {
            Models.Persona persona = new Models.Persona
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Dni = dto.Dni?.PadLeft(8, '0'),
                Visibilidad = true,
                FechaNacimiento = dto.FechaNacimiento,
                FechaDefuncion = dto.FechaDefuncion,
                Sexo = dto.Sexo,
                Correo = dto.Correo,
                Celular = dto.Celular,
                Domicilio = dto.Domicilio,
                NroActa = dto.NroActa,
                NroFolio = dto.NroFolio,
                NroSerie = dto.NroSerie,
                NroAge = dto.NroAge,
                NroTomo = dto.NroTomo,
                EstadoDifuntoId = dto.EstadoDifuntoId,
                CategoriaPersonaId = dto.CategoriaPersonaId,
                InformacionAdicional = dto.InformacionAdicional
                
            };

            await _context.Personas.AddAsync(persona);
            await _context.SaveChangesAsync();
            return persona.Id;
        }

        public async Task<PersonaDTO> Get(int id)
        {
            Models.Persona persona = await _context.Personas.FindAsync(id) ?? throw new Exception("Persona no encontrada");
            
            return new PersonaDTO
            {
                Id = persona.Id,
                Nombre = persona.Nombre,
                Apellido = persona.Apellido,
                Dni = persona.Dni,
                Visibilidad = persona.Visibilidad,
                FechaNacimiento = persona.FechaNacimiento,
                FechaDefuncion = persona.FechaDefuncion,
                InformacionAdicional = persona.InformacionAdicional,
                Sexo = persona.Sexo,
                Correo = persona.Correo,
                Celular = persona.Celular,
                Domicilio = persona.Domicilio,
                NroActa = persona.NroActa,
                NroFolio = persona.NroFolio,
                NroSerie = persona.NroSerie,
                NroAge = persona.NroAge,
                NroTomo = persona.NroTomo,
                EstadoDifuntoId = persona.EstadoDifuntoId,
                CategoriaPersonaId = persona.CategoriaPersonaId
            };
        }

        public async Task<PersonaDTO> GetByDNISexo(int dni, string sexo)
        {
            string dniString = dni.ToString("D8");
            return await _context.Personas.Where(p=> p.Dni == dniString && p.Sexo == sexo).Select(s => new PersonaDTO
            {
                Id = s.Id,
                Nombre = s.Nombre,
                Apellido = s.Apellido,
                Dni = s.Dni,
                Sexo = s.Sexo,
                Visibilidad = s.Visibilidad,
                CategoriaPersonaId = s.CategoriaPersonaId,
                Domicilio = s.Domicilio,
                Correo = s.Correo,
                Celular = s.Celular,
                FechaNacimiento = s.FechaNacimiento,
                FechaDefuncion = s.FechaDefuncion,
                InformacionAdicional = s.InformacionAdicional,
                NroActa = s.NroActa,
                NroFolio = s.NroFolio,
                NroSerie = s.NroSerie,
                NroAge = s.NroAge,
                NroTomo = s.NroTomo,
                EstadoDifuntoId = s.EstadoDifuntoId
            }).FirstOrDefaultAsync();
        }

        public async Task<HistorialPersonaDTO> HistorialPersona(int id)
        {
            HistorialPersonaDTO historial = new HistorialPersonaDTO();
            historial.Persona = await Get(id);

            //historial de tramites persona
            historial.Tramites = await _context.TramitePersonas.Where(p => p.PersonaId == id).OrderByDescending(t => t.FechaRegistro).Select(s => new TramiteDTO
            {
                Id = s.TramiteId,
                Visibilidad = s.Tramite.Visibilidad,
                FechaCreacion = s.Tramite.FechaCreacion,
                TipoTramiteId = s.Tramite.TipoTramiteId,
                EstadoActualId = s.Tramite.EstadoActualId
            }).ToListAsync();

            //historial de las parcelas donde estuvo el difunto
            historial.Parcelas = await _context.ParcelaDifuntos.Where(p => p.DifuntoId == id).Include(p=> p.Parcela).OrderByDescending(t => t.FechaIngreso).Select(f => new DifuntoHistorialParcelaDTO
            {
                Id = f.Difunto.Id,
                FechaIngreso = f.FechaIngreso,
                FechaRetiro = f.FechaRetiro,
                Dni = f.Difunto.Dni,
                Nombre = f.Difunto.Nombre,
                Apellido = f.Difunto.Apellido,
                EstadoDifunto = f.Difunto.EstadoDifuntoId,
                IdParcela = f.ParcelaId,
                NroParcela = f.Parcela.NroParcela,
                NroFila = f.Parcela.NroFila,
                NombreSeccion = f.Parcela.Seccion.Nombre,
                TipoParcelaId = f.Parcela.TipoParcelaId
            }).ToListAsync();

            return historial;
        }

        public async Task<bool> PersonaExiste(int dni, string sexo)
        {
            string dniString = dni.ToString("D8");

            // DNI antiguo (7 dígitos) → empieza con 0 → se compara sexo
            if (dniString.StartsWith("0"))
            {
                return await _context.Personas.AnyAsync(p =>
                    p.Dni == dniString &&
                    p.Sexo == sexo &&
                    p.Visibilidad
                );
            }

            // DNI moderno (8 dígitos) → NO se compara sexo
            return await _context.Personas.AnyAsync(p =>
                p.Dni == dniString &&
                p.Visibilidad
            );
        }

        public async Task<int> Update(PersonaDTO dto)
        {
            Models.Persona persona = await _context.Personas.FindAsync(dto.Id) ?? throw new Exception("Persona no encontrada");

            persona.Nombre = dto.Nombre?.Trim();
            persona.Apellido = dto.Apellido?.Trim();
            persona.Dni = dto.Dni?.ToString();
            if (dto.FechaNacimiento.HasValue)
            {
                persona.FechaNacimiento = dto.FechaNacimiento.Value;
            }
            else
            {
                persona.FechaNacimiento = null;
            }

            if (dto.FechaDefuncion.HasValue)
            {
                persona.FechaDefuncion = dto.FechaDefuncion.Value;
            }
            else
            {
                persona.FechaDefuncion = null;
            }

            persona.InformacionAdicional = dto.InformacionAdicional;
            persona.Sexo = dto.Sexo;
            persona.Correo = dto.Correo;
            persona.Celular = dto.Celular;
            persona.Domicilio = dto.Domicilio;
            persona.NroActa = dto.NroActa;
            persona.NroFolio = dto.NroFolio;
            persona.NroTomo = dto.NroTomo;
            persona.NroSerie = dto.NroSerie;
            persona.NroAge = dto.NroAge;
            persona.EstadoDifuntoId = dto.EstadoDifuntoId;
            persona.CategoriaPersonaId = dto.CategoriaPersonaId;

            await _context.SaveChangesAsync();

            return persona.Id;
        }
    }
}
