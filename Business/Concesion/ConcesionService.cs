using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.Generics;
using CemSys3.DTOs.HistorialEstado;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.Tramite;
using CemSys3.Enumerables;
using CemSys3.Interfaces.Concesion;
using CemSys3.Interfaces.HistorialEstados;
using CemSys3.Interfaces.Persona;
using CemSys3.Interfaces.Tramite;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.Concesion
{
    public class ConcesionService : IConcesion
    {
        public readonly ITramite _tramiteService;
        public readonly AppDbContext _context;
        public readonly IHistorialEstados _historialEstadosService;
        public readonly IPersona _personaService;


        public ConcesionService(ITramite tramiteService, AppDbContext context,
            IHistorialEstados estadoService, IPersona personaService)
        {
            _tramiteService = tramiteService;   
            _context = context;
            _historialEstadosService = estadoService;
            _personaService = personaService;
        }

        public async Task<GenericResultDTO> Add(ConcesionDTO dto)
        {
            try
            {
                //1- registrar tramite
                TramiteDTO tramite = new TramiteDTO
                {
                    Visibilidad = true,
                    FechaCreacion = DateTime.Now,
                    TipoTramiteId = (int)TipoTramiteEnum.ContratoConcesion,
                    UsuarioId = dto.UsuarioId ?? 0,
                    EstadoActualId = dto.EstadoTramiteId //viene por el dto, depende cada caso
                };

                int tramiteId = await _tramiteService.Add(tramite);
                await _context.SaveChangesAsync(); //guardar cambios antes de continuar

                //2- registrar Historial del tramite
                HistorialEstadosDTO historial = new HistorialEstadosDTO
                {
                    Fecha = tramite.FechaCreacion,
                    TramiteId = tramiteId,
                    EstadoTramiteId = dto.EstadoTramiteId
                };
                await _historialEstadosService.Add(historial);

                //3 - registrar el contrato de concesion
                Models.Concesione concesion = new Models.Concesione();
                concesion.TramiteId = tramiteId;
                concesion.Concesion = dto.Concesion;
                concesion.Precio = dto.Precio;
                concesion.Visibilidad = true;
                concesion.TipoParcela = dto.TipoParcela;
                concesion.Vencimiento = dto.Vencimiento;
                concesion.ParcelaId = dto.ParcelaId;
                concesion.CantidadAniosId = dto.CantidadAniosId;
                concesion.CuotaId = dto.CuotaId;
                concesion.UsuarioId = dto.UsuarioId;
                concesion.InformacionAdicional += dto.InformacionAdicional;
                await _context.Concesiones.AddAsync(concesion);

                //4 - relacion de tramite con parcela
                await _historialEstadosService.VincularTramiteAParcela(tramiteId, dto.ParcelaId);

                //5 - relacion de titulares con concesiones(si existe)
                if (dto.Titulares != null && dto.Titulares.Count > 0)
                {
                    //se busca la persona si existe en la bd
                    foreach (var persona in dto.Titulares)
                    {
                        int dni;
                        int.TryParse(persona.Dni, out dni);

                        bool existe = await _personaService.PersonaExiste(dni, persona.Sexo ?? "");

                        //si existe actualizo
                        if (existe) {
                            PersonaDTO personaExistente = new PersonaDTO();
                            personaExistente.Dni = persona.Dni?.PadLeft(8, '0');
                            personaExistente.Nombre = persona.Nombre;
                            personaExistente.Apellido = persona.Apellido;
                            personaExistente.Sexo = persona.Sexo;
                            personaExistente.Celular = persona.Celular;
                            personaExistente.Correo = persona.Correo;
                            personaExistente.Domicilio = persona.Domicilio;

                            int personaCargada = await _personaService.Update(personaExistente);

                            //6 - relacion de titulares con tramite
                            await _historialEstadosService.VincularTramiteAPersona(tramiteId, personaCargada);
                        }
                        else //si no existe creo una nueva persona
                        {
                            PersonaDTO personaNueva = new PersonaDTO();
                            personaNueva.Dni = persona.Dni?.PadLeft(8, '0');
                            personaNueva.Nombre = persona.Nombre;
                            personaNueva.Apellido = persona.Apellido;
                            personaNueva.Sexo = persona.Sexo;
                            personaNueva.Celular = persona.Celular;
                            personaNueva.Correo = persona.Correo;
                            personaNueva.Domicilio = persona.Domicilio;

                            int personaCargada = await _personaService.Add(personaNueva);

                            //6 - relacion de titulares con tramite
                            await _historialEstadosService.VincularTramiteAPersona(tramiteId, personaCargada);
                        }
                    }
                }

                //7- en parcela se modifica el info adicional
                Models.Parcela parcela = await _context.Parcelas.FindAsync(dto.ParcelaId) ?? throw new Exception("Parcela no encontrada.");
                parcela.InformacionAdicional += dto.MensajeParcela;

                //8- en concesion se modifica el info adicional

                await _context.SaveChangesAsync();
                return new GenericResultDTO
                {
                    Success = true,
                    Message = "Concesión registrada con éxito.",
                    Id = tramiteId
                };
            }
            catch (Exception)
            {
                throw;
            }
            
        }

        public async Task<PaginadoResponse<TablaConcesionDTO>> GellAllPaginado(int filtroEstado = 0, int pagina = 1, int porPagina = 10)
        {
            PaginadoResponse<TablaConcesionDTO> resultado = new PaginadoResponse<TablaConcesionDTO>();

            var query = _context.Concesiones.Where(v => v.Visibilidad == true).AsQueryable();

            // Filtro por estado del tramite
            switch (filtroEstado)
            {
                case 5: //sin contrato
                    query = query.Where(e => e.Tramite.EstadoActualId == (int)EstadosConcesionEnum.SinContrato);
                    break;
                case 6: //vigente
                    query = query.Where(e => e.Tramite.EstadoActualId == (int)EstadosConcesionEnum.Vigente);
                    break;
                case 7://vencido
                    query = query.Where(e => e.Tramite.EstadoActualId == (int)EstadosConcesionEnum.Vencido);
                    break;
                case 8://caducado
                    query = query.Where(e => e.Tramite.EstadoActualId == (int)EstadosConcesionEnum.Caducado);
                    break;
                case 0: //todos
                default:
                    // No aplicar filtro
                    break;
            }


            // Total de registros
            var total = await query.CountAsync();

            // Paginación
            resultado.Paginacion.TotalPaginas = (int)Math.Ceiling(total / (double)porPagina);
            resultado.Paginacion.PaginaActual = Math.Max(1, Math.Min(pagina, resultado.Paginacion.TotalPaginas));
            resultado.Paginacion.RegistrosPorPagina = porPagina;
            resultado.Paginacion.Accion = "TablaGeneral";
            resultado.Paginacion.Controlador = "Concesion";
            resultado.Paginacion.TotalRegistros = total;

            //solo traigo las concesiones paginadas
            var concesionesPagina = await query
                .OrderByDescending(c => c.TramiteId)
                .Skip((resultado.Paginacion.PaginaActual - 1) * porPagina)
                .Take(porPagina)
                .Select(c => new
                {
                    c.TramiteId,
                    c.Concesion,
                    c.Visibilidad,
                    c.ParcelaId,
                    c.Vencimiento,
                    c.Tramite.EstadoActualId,
                    TipoParcelaId = c.Parcela.TipoParcelaId,
                    NombreSeccion = c.Parcela.Seccion.Nombre,
                    c.Parcela.NroParcela,
                    c.Parcela.NroFila
                })
                .ToListAsync();

            //obtengo los Ids de las concesiones que traigo
            List<int> tramiteIds = concesionesPagina.Select(c => c.TramiteId).ToList();
            List<int> parcelaIds = concesionesPagina.Select(c => c.ParcelaId).ToList();

            //Traer Titulares en una sola consulta
            var titulares = await _context.HistorialTitularesConcesiones
                .Where(h => tramiteIds.Contains(h.ConcesionId ?? 0) && h.FechaFin == null)
                .Select(h => new
                {
                    h.ConcesionId,
                    Persona = new PersonaTablaGeneral
                    {
                        Id = h.Persona.Id,
                        Nombre = h.Persona.Nombre ?? "",
                        Apellido = h.Persona.Apellido ?? "",
                    }
                }).ToListAsync();

            //Traer Difuntos en una sola consulta
            var difuntos = await _context.ParcelaDifuntos
                .Where(p => parcelaIds.Contains(p.ParcelaId) && p.FechaRetiro == null)
                .Select(p => new
                {
                    p.ParcelaId,
                    Persona = new PersonaTablaGeneral
                    {
                        Id = p.Difunto.Id,
                        Nombre = p.Difunto.Nombre ?? "",
                        Apellido = p.Difunto.Apellido ?? "",
                    }
                }).ToListAsync();

            resultado.Items = concesionesPagina.Select(c => new TablaConcesionDTO
            {
                TramiteId = c.TramiteId,
                Concesion = c.Concesion,
                Visibilidad = c.Visibilidad,
                TipoParcelaId = c.TipoParcelaId,
                Vencimiento = c.Vencimiento,
                EstadoTramiteId = c.EstadoActualId,
                NombreSeccion = c.NombreSeccion,
                NroParcela = c.NroParcela,
                NroFila = c.NroFila,

                Titulares = titulares
                    .Where(t => t.ConcesionId == c.TramiteId)
                    .Select(t => t.Persona)
                    .ToList(),

                Difuntos = difuntos
                    .Where(d => d.ParcelaId == c.ParcelaId)
                    .Select(d => d.Persona)
                    .ToList()

            }).ToList();

            return resultado;
        }
    }
}
