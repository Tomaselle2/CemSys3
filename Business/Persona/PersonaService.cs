using CemSys3.DTOs.Ingreso;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.Tramite;
using CemSys3.Enumerables;
using CemSys3.Interfaces.Persona;
using CemSys3.Models;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;

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

        public async Task CambiarCategoria(int personaId, int categoriaId)
        {
            Models.Persona persona = await _context.Personas.FindAsync(personaId) ?? throw new Exception("Persona no encontrada");
            persona.CategoriaPersonaId = categoriaId;

            _context.Personas.Update(persona);
            await _context.SaveChangesAsync();
        }

        public async Task<PersonaDTO> Get(int id)
        {
            Models.Persona persona = await _context.Personas.FindAsync(id) ?? throw new Exception("Persona no encontrada");
            DateTime? fechaIngreso = null;
            if(persona.CategoriaPersonaId == (int)CategoriaPersonaEnum.Fallecido)
            {
                Models.ParcelaDifunto parcelaDifunto = await _context.ParcelaDifuntos.Where(p => p.DifuntoId == persona.Id).FirstOrDefaultAsync() 
                    ?? throw new Exception("Error al obtener la fecha de ingreso");

                fechaIngreso = parcelaDifunto.FechaIngreso;
            }

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
                CategoriaPersonaId = persona.CategoriaPersonaId,
                FechaIngreso = fechaIngreso
            };
        }

        public async Task<PaginadoResponse<PersonaDTO>> GetAllFiltro(
            int dni = 0,
            string nombre = "",
            string apellido = "",
            int pagina = 1,
            int porPagina = 10)
        {
            PaginadoResponse<PersonaDTO> resultado = new();

            IQueryable<Models.Persona> query = _context.Personas.AsNoTracking();

            // Aplicar filtros solo si el usuario ingresó datos
            if (dni > 0)
            {
                query = query.Where(p => p.Dni == dni.ToString("D8"));
            }

            if (!string.IsNullOrWhiteSpace(nombre)) 
            {
                query = query.Where(p => p.Nombre.Contains(nombre));
            }

            if (!string.IsNullOrWhiteSpace(apellido))
            {
                query = query.Where(p => p.Apellido.Contains(apellido));
            }

            // Total de registros filtrados
            var total = await query.CountAsync();

            resultado.Paginacion.TotalPaginas = (int)Math.Ceiling(total / (double)porPagina);
            resultado.Paginacion.PaginaActual = Math.Max(1, pagina);
            resultado.Paginacion.RegistrosPorPagina = porPagina;
            resultado.Paginacion.Accion = "Index";
            resultado.Paginacion.Controlador = "Persona";
            resultado.Paginacion.TotalRegistros = total;

            resultado.Items = await query.AsNoTracking()
                .OrderBy(p => p.Apellido)
                .ThenBy(p => p.Nombre)
                .Skip((resultado.Paginacion.PaginaActual - 1) * porPagina)
                .Take(porPagina)
                .Select(p => new PersonaDTO
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Apellido = p.Apellido,
                    Dni = p.Dni,
                    Sexo = p.Sexo,
                    Visibilidad = p.Visibilidad,
                    CategoriaPersonaId = p.CategoriaPersonaId
                })
                .ToListAsync();

            return resultado;
        }

        public async Task<PersonaDTO> GetByDNISexo(int dni, string sexo)
        {
            string dniString = dni.ToString("D8");
            PersonaDTO persona = await _context.Personas.Where(p => p.Dni == dniString && p.Sexo == sexo).Select(s => new PersonaDTO
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
            }).FirstOrDefaultAsync() ?? throw new Exception("Persona no encontrada");

            DateTime? fechaIngreso = null;
            if (persona.CategoriaPersonaId == (int)CategoriaPersonaEnum.Fallecido)
            {
                Models.ParcelaDifunto parcelaDifunto = await _context.ParcelaDifuntos.Where(p => p.DifuntoId == persona.Id).FirstOrDefaultAsync()
                    ?? throw new Exception("Error al obtener la fecha de ingreso");

                fechaIngreso = parcelaDifunto.FechaIngreso;
            }

            persona.FechaIngreso = fechaIngreso;

            return persona;
        }

        public async Task<DifuntoHistorialParcelaDTO> GetParcelaPorDifuntoId(int difuntoId)
        {
            Models.Persona persona = await _context.Personas.FindAsync(difuntoId) ?? throw new Exception("Persona no encontrada");


            DifuntoHistorialParcelaDTO dto = await _context.ParcelaDifuntos.Where(p => p.DifuntoId == difuntoId && p.FechaRetiro == null).Include(p => p.Parcela).Select(f => new DifuntoHistorialParcelaDTO
            {
                Id = persona.Id,
                FechaIngreso = f.FechaIngreso,
                FechaRetiro = f.FechaRetiro,
                Dni = persona.Dni,
                Nombre = persona.Nombre,
                Apellido = persona.Apellido,
                EstadoDifunto = persona.EstadoDifuntoId,
                IdParcela = f.ParcelaId,
                NroParcela = f.Parcela.NroParcela,
                NroFila = f.Parcela.NroFila,
                NombreSeccion = f.Parcela.Seccion.Nombre,
                TipoParcelaId = f.Parcela.TipoParcelaId,
            }).FirstOrDefaultAsync() ?? throw new Exception("No se encontró la parcela para el difunto");

            Models.Concesione concesionAntigua = await _context.Concesiones.FirstOrDefaultAsync(c => c.ParcelaId == dto.IdParcela && c.FechaFin == null) ?? throw new Exception("Concesión antigua no encontrada.");

            dto.ConcesionId = concesionAntigua.TramiteId;

            return dto;
        }

        public async Task<HistorialPersonaDTO> HistorialPersona(int id)
        {
            HistorialPersonaDTO historial = new HistorialPersonaDTO();
            historial.Persona = await Get(id);

            //historial de tramites persona
            historial.Tramites = await _context.TramitePersonas.Where(p => p.PersonaId == id).OrderByDescending(t => t.FechaRegistro).AsNoTracking().Select(s => new TramiteDTO
            {
                Id = s.TramiteId,
                Visibilidad = s.Tramite.Visibilidad,
                FechaCreacion = s.Tramite.FechaCreacion,
                TipoTramiteId = s.Tramite.TipoTramiteId,
                EstadoActualId = s.Tramite.EstadoActualId
            }).ToListAsync();

            //historial de las parcelas donde estuvo el difunto
            historial.Parcelas = await _context.ParcelaDifuntos.Where(p => p.DifuntoId == id).Include(p=> p.Parcela).AsNoTracking().OrderByDescending(t => t.FechaIngreso).Select(f => new DifuntoHistorialParcelaDTO
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

            if (historial.Persona.CategoriaPersonaId == (int)CategoriaPersonaEnum.Titular)
            {
                historial.ConecesionesActivasTitular = await _context.HistorialTitularesConcesiones.Include(h => h.Concesion).ThenInclude(c => c.Tramite).AsNoTracking()
                    .Where(h =>
                        h.PersonaId == historial.Persona.Id &&
                        h.FechaFin == null &&
                        h.Concesion.FechaFin == null)
                    .Select(h => new DTO_ConcesionTitular
                    {
                        TramiteId = h.Concesion.TramiteId,
                        NroConcesion = h.Concesion.Concesion ?? 0,
                        EstadoId = h.Concesion.Tramite.EstadoActualId,
                        Vencimiento = h.Concesion.Vencimiento,
                        TipoParcela = h.Concesion.TipoParcela
                    })
                    .OrderBy(c => c.NroConcesion)
                    .ToListAsync();
            }

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
            
            if(persona.CategoriaPersonaId == (int)CategoriaPersonaEnum.Fallecido)
            {
                if (dto.FechaIngreso.HasValue)
                {
                    Models.ParcelaDifunto parcelaDifunto = await _context.ParcelaDifuntos.Where(p => p.DifuntoId == persona.Id).FirstOrDefaultAsync() ?? throw new Exception("Error al actualizar la fecha de ingreso");
                    Models.Introduccione introduccion = await _context.Introducciones.Where(p => p.DifuntoId == persona.Id && p.ParcelaId == parcelaDifunto.ParcelaId).FirstOrDefaultAsync();


                    parcelaDifunto.FechaIngreso = dto.FechaIngreso;

                    if(introduccion != null)
                    {
                        introduccion.FechaIngreso = dto.FechaIngreso;
                    }
                }
                
            }
            
            persona.Nombre = dto.Nombre?.Trim();
            persona.Apellido = dto.Apellido?.Trim();

            if (!string.IsNullOrEmpty(dto.Dni))
            {
                persona.Dni = dto.Dni.PadLeft(8, '0');
            }

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

        public async Task<int> UpdateDatosIngresoTitularFallecido(PersonaDTO dto)
        {
            Models.Persona persona = await _context.Personas.FindAsync(dto.Id) ?? throw new Exception("Persona no encontrada");

            if (persona.CategoriaPersonaId == (int)CategoriaPersonaEnum.Fallecido)
            {
                if (dto.FechaIngreso.HasValue)
                {
                    Models.ParcelaDifunto parcelaDifunto = await _context.ParcelaDifuntos.Where(p => p.DifuntoId == persona.Id).FirstOrDefaultAsync() ?? throw new Exception("Error al actualizar la fecha de ingreso");
                    Models.Introduccione introduccion = await _context.Introducciones.Where(p => p.DifuntoId == persona.Id && p.ParcelaId == parcelaDifunto.ParcelaId).FirstOrDefaultAsync();


                    parcelaDifunto.FechaIngreso = dto.FechaIngreso;

                    if (introduccion != null)
                    {
                        introduccion.FechaIngreso = dto.FechaIngreso;
                    }
                }

            }

            persona.Nombre = dto.Nombre?.Trim().ToLower();
            persona.Apellido = dto.Apellido?.Trim().ToLower();

            if (!string.IsNullOrEmpty(dto.Dni))
            {
                persona.Dni = dto.Dni.PadLeft(8, '0');
            }

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

            persona.InformacionAdicional += dto.InformacionAdicional;
            persona.Sexo = dto.Sexo;
            persona.NroActa = dto.NroActa;
            persona.NroFolio = dto.NroFolio;
            persona.NroTomo = dto.NroTomo;
            persona.NroSerie = dto.NroSerie;
            persona.NroAge = dto.NroAge;
            persona.EstadoDifuntoId = dto.EstadoDifuntoId;

            await _context.SaveChangesAsync();

            return persona.Id;
        }

        public async Task<CoincidenciaIngresoDTO> BuscarCoincidenciaParaIngreso(
    int? dni, string? sexo, string nombre, string apellido, bool ignorarCoincidenciaPorNombre = false)
        {
            Models.Persona? persona = null;

            // 1) Buscar por DNI, con el mismo criterio que ya usa PersonaExiste
            if (dni.HasValue && dni.Value > 0)
            {
                string dniString = dni.Value.ToString("D8");

                if (dniString.StartsWith("0"))
                {
                    // DNI antiguo (7 dígitos): se compara también el sexo
                    persona = await _context.Personas
                        .FirstOrDefaultAsync(p => p.Dni == dniString && p.Sexo == sexo && p.Visibilidad);
                }
                else
                {
                    // DNI moderno (8 dígitos): alcanza con el número de documento
                    persona = await _context.Personas
                        .FirstOrDefaultAsync(p => p.Dni == dniString && p.Visibilidad);
                }
            }

            bool coincidenciaPorDni = persona != null;

            // 2) Si no hubo coincidencia por DNI, buscar por nombre y apellido SOLO entre fallecidos.
            //    Esto cubre actas viejas que no tienen DNI cargado.
            //    Si el empleado ya confirmó que es "otra persona", no volvemos a buscar por nombre.
            if (persona == null && !ignorarCoincidenciaPorNombre)
            {
                string n = nombre.Trim().ToLower();
                string a = apellido.Trim().ToLower();

                persona = await _context.Personas.FirstOrDefaultAsync(p =>
                    p.Visibilidad &&
                    p.CategoriaPersonaId == (int)CategoriaPersonaEnum.Fallecido &&
                    p.Nombre.ToLower() == n &&
                    p.Apellido.ToLower() == a);
            }

            if (persona == null)
            {
                return new CoincidenciaIngresoDTO { Existe = false };
            }

            bool esTitular = persona.CategoriaPersonaId == (int)CategoriaPersonaEnum.Titular;
            bool activo = false;

            if (!esTitular)
            {
                // ¿Tiene una asignación de parcela vigente (sin retirar)?
                activo = await _context.ParcelaDifuntos
                    .AnyAsync(pd => pd.DifuntoId == persona.Id && pd.FechaRetiro == null);
            }

            return new CoincidenciaIngresoDTO
            {
                Existe = true,
                EsTitular = esTitular,
                EstaActivoEnCementerio = activo,
                CoincidenciaPorDni = coincidenciaPorDni,
                Persona = await Get(persona.Id)
            };
        }
    }
}
