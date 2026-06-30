using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.Generics;
using CemSys3.DTOs.HistorialEstado;
using CemSys3.DTOs.Nota;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.Tarea;
using CemSys3.DTOs.Tarifaria;
using CemSys3.DTOs.Tramite;
using CemSys3.Enumerables;
using CemSys3.Helpers.Enumerable;
using CemSys3.Interfaces.Concesion;
using CemSys3.Interfaces.HistorialEstados;
using CemSys3.Interfaces.Notas;
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
        public readonly INotas _notasService;


        public ConcesionService(ITramite tramiteService, AppDbContext context,
            IHistorialEstados estadoService, IPersona personaService, INotas notas)
        {
            _tramiteService = tramiteService;   
            _context = context;
            _historialEstadosService = estadoService;
            _personaService = personaService;
            _notasService = notas;
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
                concesion.FechaInicio = dto.FechaInicio ?? DateTime.Now;
                concesion.InformacionAdicional += dto.InformacionAdicional;
                await _context.Concesiones.AddAsync(concesion);

                //4 - relacion de tramite con parcela
                await _historialEstadosService.VincularTramiteAParcela(tramiteId, dto.ParcelaId);


                //4.1 - registrar historial de parcela en la concesion
                Models.HistorialParcelasConcesion historialParcela = new Models.HistorialParcelasConcesion
                {
                    ConcesionId = tramiteId,
                    ParcelaId = dto.ParcelaId,
                    FechaInicio = DateTime.Now,
                    FechaFin = null,           // null = parcela actualmente vinculada
                    TramiteOrigenId = tramiteId
                };
                await _context.HistorialParcelasConcesions.AddAsync(historialParcela);


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
                            await _historialEstadosService.VincularTitularAConcesion(personaCargada, tramiteId);
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
                            await _historialEstadosService.VincularTitularAConcesion(personaCargada, tramiteId);
                        }
                    }
                }

                //7- en parcela se modifica el info adicional
                Models.Parcela parcela = await _context.Parcelas.FindAsync(dto.ParcelaId) ?? throw new Exception("Parcela no encontrada.");
                parcela.InformacionAdicional += dto.MensajeParcela;

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
        public async Task<GenericResultDTO> Update(ConcesionDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                //buscar la concesion por el tramiteId
                Models.Concesione concesion = await _context.Concesiones.FirstOrDefaultAsync(c => c.TramiteId == dto.TramiteId) ?? throw new Exception("Concesión no encontrada.");

                //reemplazar los datos de la concesion con el dto.
                concesion.Concesion = dto.Concesion;
                concesion.Precio = dto.Precio;
                concesion.Vencimiento = dto.Vencimiento;
                concesion.CantidadAniosId = dto.CantidadAniosId;
                concesion.CuotaId = dto.CuotaId;
                concesion.UsuarioId = dto.UsuarioId;
                concesion.InformacionAdicional += dto.InformacionAdicional;


                string mensajeContrato = $"\n● El {DateTime.Now:dd/MM/yyyy} se realizó contrato de concesión ({dto.Concesion?.ToString("D5") ?? "-----"}) por {EnumHelper.GetDisplayNameByValue<AniosConcesionEnum>(dto.CantidadAniosId.Value)}. Vencimiento {dto.Vencimiento}.";

                await ProcesarTitularesConHistorial(
                    dto.TramiteId,
                    dto.Titulares ?? new List<PersonaDTO>(),
                    concesion,
                    mensajeContrato
                );

                //actualizo a todos los difuntos el info adicional
                if (dto.Difuntos != null && dto.Difuntos.Count > 0)
                {
                    foreach (var difunto in dto.Difuntos)
                    {
                        PersonaDTO difuntoCargado = await _personaService.Get(difunto.Id);
                        difuntoCargado.InformacionAdicional += mensajeContrato;
                        int id = await _personaService.Update(difuntoCargado);
                        await _historialEstadosService.VincularTramiteAPersona(dto.TramiteId, difuntoCargado.Id);
                    }
                }

                //Actualiar tramite

                TramiteDTO tramite = await _tramiteService.Get(dto.TramiteId);
                tramite.EstadoActualId = dto.EstadoTramiteId; //viene por el dto, depende cada caso

                await _tramiteService.Update(tramite);

                //2- registrar Historial del tramite
                HistorialEstadosDTO historial = new HistorialEstadosDTO
                {
                    Fecha = DateTime.Now,
                    TramiteId = tramite.Id,
                    EstadoTramiteId = tramite.EstadoActualId
                };
                await _historialEstadosService.Add(historial);

                //3- en parcela se modifica el info adicional
                Models.Parcela parcela = await _context.Parcelas.FindAsync(dto.ParcelaId) ?? throw new Exception("Parcela no encontrada.");
                parcela.InformacionAdicional += dto.MensajeParcela;


                //4- generar nota
                string descripcionNota = $"\n● El {DateTime.Now:dd/MM/yyyy} se realizó contrato de concesión ({dto.Concesion?.ToString("D5") ?? "-----"})";
                string nombreNota = $"Para Program (concesión {concesion.Concesion?.ToString("D5") ?? "-----"})";
                string vencimiento = $"Modificar vencimiento a {concesion.Vencimiento}";
                string titularNota = $"El titular debe ser {dto.Titulares?[0].Apellido?.ToUpper()}, {dto.Titulares?[0].Nombre?.ToUpper()} DNI {dto.Titulares?[0].Dni}";
                string pago = $"Generar cuotas concesión";

                await GenerarNotaRecordatorio(descripcionNota, nombreNota, vencimiento, titularNota, dto.UsuarioId ?? 0, pago);


                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new GenericResultDTO
                {
                    Success = true,
                    Message = "Concesión actualizada con éxito.",
                    Id = dto.TramiteId
                };
            }
            catch (Exception) {
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<PaginadoResponse<TablaConcesionDTO>> GellAllPaginado(
    int filtroEstado = 0,
    int pagina = 1,
    int porPagina = 10,
    string nombre = "",
    string apellido = "",
    string nombrePanteon = "",
    int concesion = 0,
    int? tipoParcelaID = null,
    int? seccionID = null,
    int? parcelaID = null,
    DateOnly? fechaDesde = null,
    DateOnly? fechaHasta = null)
        {
            PaginadoResponse<TablaConcesionDTO> resultado = new PaginadoResponse<TablaConcesionDTO>();

            var query = _context.Concesiones
                .Where(v => v.Visibilidad.HasValue)
                .AsQueryable().AsNoTracking();

            switch (filtroEstado)
            {
                case 5: query = query.Where(e => e.Tramite.EstadoActualId == (int)EstadosConcesionEnum.SinContrato); break;
                case 6: query = query.Where(e => e.Tramite.EstadoActualId == (int)EstadosConcesionEnum.Vigente); break;
                case 7: query = query.Where(e => e.Tramite.EstadoActualId == (int)EstadosConcesionEnum.Vencido); break;
                case 8: query = query.Where(e => e.Tramite.EstadoActualId == (int)EstadosConcesionEnum.Caducado); break;
            }

            if (concesion > 0)
                query = query.Where(c => c.Concesion == concesion);

            // Aplicar filtros de fecha si existen
            if (fechaDesde.HasValue)
            {
                query = query.Where(x => x.Vencimiento >= fechaDesde);
            }

            if (fechaHasta.HasValue)
            {
                // Añadir un día para incluir todo el día hasta
                query = query.Where(x => x.Vencimiento < fechaHasta.Value.AddDays(1));
            }

            // Nuevos filtros de parcela (se ignoran si son null)
            if (tipoParcelaID.HasValue)
                query = query.Where(c => c.Parcela.TipoParcelaId == tipoParcelaID.Value);

            if (seccionID.HasValue)
                query = query.Where(c => c.Parcela.SeccionId == seccionID.Value);

            if (parcelaID.HasValue)
                query = query.Where(c => c.ParcelaId == parcelaID.Value);

            if (!string.IsNullOrWhiteSpace(nombrePanteon))
            {
                query = query.Where(c => c.Parcela.NombrePanteon.Contains(nombrePanteon));
            }

            if (!string.IsNullOrWhiteSpace(nombre) || !string.IsNullOrWhiteSpace(apellido))
            {
                query = query.Where(c =>
                    // Buscar en titulares actuales
                    c.HistorialTitularesConcesiones.Any(h =>
                        h.FechaFin == null &&
                        (string.IsNullOrWhiteSpace(nombre) || h.Persona.Nombre.Contains(nombre)) &&
                        (string.IsNullOrWhiteSpace(apellido) || h.Persona.Apellido.Contains(apellido)))
                    ||
                    // Buscar en difuntos solo si la concesión está activa
                    (c.FechaFin == null &&
                     c.Parcela.ParcelaDifuntos.Any(pd =>
                        pd.FechaRetiro == null &&
                        (string.IsNullOrWhiteSpace(nombre) || pd.Difunto.Nombre.Contains(nombre)) &&
                        (string.IsNullOrWhiteSpace(apellido) || pd.Difunto.Apellido.Contains(apellido))))
                );
            }

            var total = await query.CountAsync();
            resultado.Paginacion.TotalPaginas = (int)Math.Ceiling(total / (double)porPagina);
            resultado.Paginacion.PaginaActual = Math.Max(1, Math.Min(pagina, Math.Max(1, resultado.Paginacion.TotalPaginas)));
            resultado.Paginacion.RegistrosPorPagina = porPagina;
            resultado.Paginacion.Accion = "TablaGeneral";
            resultado.Paginacion.Controlador = "Concesion";
            resultado.Paginacion.TotalRegistros = total;

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
                    c.FechaInicio,
                    c.FechaFin,
                    c.TramiteRetiroId,  // ← nueva columna
                    c.Tramite.EstadoActualId,
                    TipoParcelaId = c.Parcela.TipoParcelaId,
                    NombreSeccion = c.Parcela.Seccion.Nombre,
                    c.Parcela.NroParcela,
                    c.Parcela.NroFila
                })
                .ToListAsync();

            var tramiteIds = concesionesPagina.Select(c => c.TramiteId).ToList();
            var parcelaIds = concesionesPagina.Select(c => c.ParcelaId).ToList();

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
                        celular = h.Persona.Celular ?? ""
                    }
                })
                .ToListAsync();

            // Difuntos actuales de la parcela (FechaRetiro == null), sin importar estado de concesión
            var todosDifuntos = await _context.ParcelaDifuntos
                .Where(pd => parcelaIds.Contains(pd.ParcelaId) && pd.FechaRetiro == null)
                .Select(pd => new
                {
                    pd.ParcelaId,
                    Persona = new PersonaTablaGeneral
                    {
                        Id = pd.Difunto.Id,
                        Nombre = pd.Difunto.Nombre ?? "",
                        Apellido = pd.Difunto.Apellido ?? ""
                    }
                })
                .ToListAsync();

            resultado.Items = concesionesPagina
                .Select(c =>
                {
                    var difuntosDeConcesion = todosDifuntos
                        .Where(pd => pd.ParcelaId == c.ParcelaId && 
                        c.FechaFin == null)  // solo mostrar difuntos si la concesión está activa
                        .Select(pd => pd.Persona)
                        .ToList();

                    return new TablaConcesionDTO
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
                        Difuntos = difuntosDeConcesion
                    };
                })
                .ToList();

            return resultado;
        }

        public async Task<GenerarContratoDTO> SolicitarDatosParaGenerarContrato(int idTramite)
        {
            GenerarContratoDTO dto = new GenerarContratoDTO();

            Models.Concesione concesion = await _context.Concesiones.AsNoTracking()
                .Include(c => c.Tramite)
                .Include(c => c.Parcela)
                    .ThenInclude(p => p.Seccion)
                .FirstOrDefaultAsync(c => c.TramiteId == idTramite) ?? throw new Exception("Concesión no encontrada.");

            dto.TramiteId = concesion.TramiteId;
            dto.EstadoTramiteId = concesion.Tramite.EstadoActualId;
            dto.ParcelaId = concesion.ParcelaId;
            dto.TipoParcela = concesion.TipoParcela;
            dto.SeccionId = concesion.Parcela.SeccionId;
            dto.NombreSeccion = concesion.Parcela.Seccion.Nombre;
            dto.NroParcela = concesion.Parcela.NroParcela;
            dto.NroFila = concesion.Parcela.NroFila;
            dto.NroConcesion = concesion.Concesion;

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

            //Traer Titulares en una sola consulta
            dto.Titulares = await _context.HistorialTitularesConcesiones
                .Where(h => h.ConcesionId == idTramite && h.FechaFin == null)
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

            //consultar los precios relacionados a la parcela dependiendo del tipo de parcela
            if (dto.TipoParcela == "Nicho")
            {
                dto.PreciosNichos = await _context.PreciosTarifarias
                    .Where(t => t.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.ConcesionNicho && t.SeccionId == dto.SeccionId && t.NroFila == dto.NroFila)
                    .Select(t => new PrecioTarifariaDTO
                    {
                        Id = t.Id,
                        Precio = t.Precio,
                        NroFila = t.NroFila,
                        ConceptoTarifariaId = t.ConceptoTarifariaId,
                        AniosConcesionId = t.AniosConcesionId,
                        SeccionId = t.SeccionId,
                        Visibilidad = t.Visibilidad,
                    }).ToListAsync();
            }

            //consultar los precios relacionados a la parcela dependiendo del tipo de parcela
            if (dto.TipoParcela == "Fosa")
            {
                dto.PreciosFosas = await _context.PreciosTarifarias
                    .Where(t => t.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.ConcesionFosa)
                    .Select(t => new PrecioTarifariaDTO
                    {
                        Id = t.Id,
                        Precio = t.Precio,
                        NroFila = t.NroFila,
                        ConceptoTarifariaId = t.ConceptoTarifariaId,
                        AniosConcesionId = t.AniosConcesionId,
                        SeccionId = t.SeccionId,
                        Visibilidad = t.Visibilidad,
                    }).ToListAsync();
            }

            Models.PreciosTarifaria porcentajeAumentoOtrasLocalidades = await _context.PreciosTarifarias.Where(p => p.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.PorcentajeAumentoConcesionesOtrasLocalidades).FirstOrDefaultAsync() ?? throw new Exception("No se encontro el % fondo ayuda");
            Models.PreciosTarifaria porcentajeDescuentoRenovacion = await _context.PreciosTarifarias.Where(p => p.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.PorcentajeDescuentoRenovacionConcesionAlDia).FirstOrDefaultAsync() ?? throw new Exception("No se encontro el % fondo ayuda");
            Models.PreciosTarifaria porcentajeFondo = await _context.PreciosTarifarias.Where(p => p.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.PorcentajeFondoAyudaCentroSalud).FirstOrDefaultAsync() ?? throw new Exception("No se encontro el % fondo ayuda");

            dto.PorcentajeDescuentoRenovacionConcesionAlDia = porcentajeDescuentoRenovacion.Precio;
            dto.PorcentajeAumentoConcesionesOtrasLocalidades = porcentajeAumentoOtrasLocalidades.Precio;
            dto.PorcentajeFondoAyudaCentroSalud = porcentajeFondo.Precio;
            
            return dto;
        }

        public async Task<InfoGeneralDTO> InfoGeneral(int idTramite)
        {
            InfoGeneralDTO dto = new InfoGeneralDTO();
            Models.Concesione concesion = await _context.Concesiones.AsNoTracking()
               .Include(c => c.Tramite)
               .Include(c => c.Parcela)
                   .ThenInclude(p => p.Seccion)
               .FirstOrDefaultAsync(c => c.TramiteId == idTramite) ?? throw new Exception("Concesión no encontrada.");

            dto.TramiteId = concesion.TramiteId;
            dto.EstadoTramiteId = concesion.Tramite.EstadoActualId;
            dto.ParcelaId = concesion.ParcelaId;
            dto.TipoParcela = concesion.TipoParcela;
            dto.SeccionId = concesion.Parcela.SeccionId;
            dto.NombreSeccion = concesion.Parcela.Seccion.Nombre;
            dto.NroParcela = concesion.Parcela.NroParcela;
            dto.NroFila = concesion.Parcela.NroFila;
            dto.NroConcesion = concesion.Concesion;
            dto.Vencimiento = concesion.Vencimiento;
            dto.InfoAdicional = concesion.InformacionAdicional ?? "";

            // Solo mostrar difuntos actuales si la concesión está activa
            if (concesion.FechaFin == null)
            {
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
            }
            else
            {
                // Concesión caducada: parcela vacía, no hay difuntos que mostrar
                dto.Difuntos = new List<DifuntoContratoDTO>();
            }

            //Traer Titulares en una sola consulta
            dto.Titulares = await _context.HistorialTitularesConcesiones
                .Where(h => h.ConcesionId == idTramite && h.FechaFin == null)
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

            var hoy = DateTime.Today;

            if (dto.Vencimiento.HasValue)
            {
                var venc = dto.Vencimiento.Value.ToDateTime(TimeOnly.MinValue);

                bool estaVencido = dto.EstadoTramiteId == (int)EstadosConcesionEnum.Vencido;

                bool mismoMes =
                    venc.Month == hoy.Month &&
                    venc.Year == hoy.Year;

                dto.PuedeRenovar = estaVencido || mismoMes;
            }
            else
            {
                dto.PuedeRenovar = false;
            }

            return dto;
        }
        public async Task<InfoGeneralDTO> InfoGeneralMinima(int idTramite)
        {
            InfoGeneralDTO dto = new InfoGeneralDTO();
            Models.Concesione concesion = await _context.Concesiones.AsNoTracking()
               .Include(c => c.Tramite)
               .Include(p=> p.Parcela)
                .ThenInclude(p => p.Seccion)
               .FirstOrDefaultAsync(c => c.TramiteId == idTramite) ?? throw new Exception("Concesión no encontrada.");

            dto.TramiteId = concesion.TramiteId;
            dto.EstadoTramiteId = concesion.Tramite.EstadoActualId;
            dto.NroConcesion = concesion.Concesion;
            dto.Vencimiento = concesion.Vencimiento;
            dto.ParcelaId = concesion.ParcelaId;
            dto.TipoParcela = concesion.TipoParcela;
            dto.SeccionId = concesion.Parcela.SeccionId;
            dto.NombreSeccion = concesion.Parcela.Seccion.Nombre;
            dto.NroParcela = concesion.Parcela.NroParcela;
            dto.NroFila = concesion.Parcela.NroFila;


            return dto;
        }
        public async Task<bool> ExisteNroConcesion(int nroConcesion)
        {
            bool existe = await _context.Concesiones.AnyAsync(c => c.Concesion == nroConcesion);
            return existe;
        }

        //get
        public async Task<ModificarDatosConcesionDTO> ModificarDatosConecesion(int tramiteId)
        {
            ModificarDatosConcesionDTO dto = new ModificarDatosConcesionDTO();

            Models.Concesione concesion = await _context.Concesiones
               .Include(c => c.Tramite)
               .FirstOrDefaultAsync(c => c.TramiteId == tramiteId) ?? throw new Exception("Concesión no encontrada.");

            dto.TramiteId = concesion.TramiteId;
            dto.EstadoTramiteId = concesion.Tramite.EstadoActualId;
            dto.Vencimiento = concesion.Vencimiento;
            dto.NroConcesion = concesion.Concesion;
            dto.FechaInicio = concesion.FechaInicio;

            //Traer Titulares en una sola consulta
            dto.Titulares = await _context.HistorialTitularesConcesiones
                .Where(h => h.ConcesionId == tramiteId && h.FechaFin == null)
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

            return dto;
        }

        //post
        public async Task ModificarDatosConecesion(ModificarDatosConcesionDTO dto)
        {
            Models.Concesione concesion = await _context.Concesiones
               .Include(c => c.Tramite)
               .FirstOrDefaultAsync(c => c.TramiteId == dto.TramiteId) ?? throw new Exception("Concesión no encontrada.");

            Models.Tramite tramite = await _context.Tramites.FindAsync(dto.TramiteId) ?? throw new Exception("Concesión no encontrada");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                concesion.Concesion = dto.NroConcesion;

                int nuevoEstado;


                if (dto.Vencimiento != null && dto.Vencimiento >= DateOnly.FromDateTime(DateTime.Now))
                {
                    nuevoEstado = (int)EstadosConcesionEnum.Vigente;
                }
                else
                {
                    nuevoEstado = (int)EstadosConcesionEnum.Vencido;
                }

                concesion.Vencimiento = dto.Vencimiento;


                // SOLO si cambia el estado
                if (tramite.EstadoActualId != nuevoEstado)
                {
                    tramite.EstadoActualId = nuevoEstado;

                    HistorialEstadosDTO historial = new HistorialEstadosDTO
                    {
                        Fecha = DateTime.Now,
                        TramiteId = tramite.Id,
                        EstadoTramiteId = nuevoEstado
                    };

                    await _historialEstadosService.Add(historial);
                }

                if (dto.TitularesPost != null && dto.TitularesPost.Count > 0)
                {
                    await ProcesarTitularesConHistorial(
                    dto.TramiteId,
                    dto.TitularesPost ?? new List<PersonaDTO>(),
                    concesion,
                    null //importante
                         );
                }

                if (dto.FechaInicio.HasValue)
                {
                    concesion.FechaInicio = dto.FechaInicio.Value;
                }
                



                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
            
        }


      



        private async Task ProcesarTitularesConHistorial(
    int tramiteId,
    List<PersonaDTO> titularesDTO,
    Models.Concesione concesion,
    string? mensajeContrato = null)
        {
            // Titulares actuales activos
            var titularesActuales = await _context.HistorialTitularesConcesiones
                .Where(x => x.ConcesionId == tramiteId && x.FechaFin == null)
                .ToListAsync();

            var idsActuales = titularesActuales
                .Where(x => x.PersonaId.HasValue)
                .Select(x => x.PersonaId!.Value)
                .ToHashSet();

            var idsNuevos = new HashSet<int>();

            if (titularesDTO == null || titularesDTO.Count == 0)
                return;

            foreach (var persona in titularesDTO)
            {
                int dni = int.Parse(persona.Dni);
                PersonaDTO personaDB;

                bool existe = await _personaService.PersonaExiste(dni, persona.Sexo ?? "");

                if (existe)
                {
                    personaDB = await _personaService.GetByDNISexo(dni, persona.Sexo);

                    // actualizar datos
                    personaDB.Dni = persona.Dni?.PadLeft(8, '0');
                    personaDB.Nombre = persona.Nombre;
                    personaDB.Apellido = persona.Apellido;
                    personaDB.Sexo = persona.Sexo;
                    personaDB.Celular = persona.Celular;
                    personaDB.Correo = persona.Correo;
                    personaDB.Domicilio = persona.Domicilio;
                    personaDB.CategoriaPersonaId = (int)CategoriaPersonaEnum.Titular;
                }
                else
                {
                    personaDB = new PersonaDTO
                    {
                        Dni = persona.Dni?.PadLeft(8, '0'),
                        Nombre = persona.Nombre,
                        Apellido = persona.Apellido,
                        Sexo = persona.Sexo,
                        Celular = persona.Celular,
                        Correo = persona.Correo,
                        Domicilio = persona.Domicilio,
                        CategoriaPersonaId = (int)CategoriaPersonaEnum.Titular,
                        InformacionAdicional = ""
                    };

                    personaDB.Id = await _personaService.Add(personaDB);

                    concesion.InformacionAdicional +=
                        $"\n● El {DateTime.Now:dd/MM/yyyy} se asigna como titular a " +
                        $"{personaDB.Apellido?.ToUpper()}, {personaDB.Nombre?.ToUpper()}.";
                }

                bool yaEraTitular = idsActuales.Contains(personaDB.Id);
                bool esNuevoTitular = !yaEraTitular;

                // mensaje contrato (UNA sola vez)
                if (!string.IsNullOrWhiteSpace(mensajeContrato))
                {
                    personaDB.InformacionAdicional += mensajeContrato;
                }

                // mensaje titular nuevo
                if (esNuevoTitular)
                {
                    personaDB.InformacionAdicional +=
                        $"\n● El {DateTime.Now:dd/MM/yyyy} se lo asigna como titular en concesión " +
                        $"({concesion.Concesion?.ToString("D5") ?? "-----"}).";

                    concesion.InformacionAdicional +=
                        $"\n● El {DateTime.Now:dd/MM/yyyy} se agrega como titular a " +
                        $"{personaDB.Apellido?.ToUpper()}, {personaDB.Nombre?.ToUpper()}.";

                    await _historialEstadosService
                        .VincularTitularAConcesion(personaDB.Id, tramiteId);
                }

                // guardar persona UNA sola vez
                await _personaService.Update(personaDB);

                // vincular tramite
                await _historialEstadosService
                    .VincularTramiteAPersona(tramiteId, personaDB.Id);

                idsNuevos.Add(personaDB.Id);
            }

            // cerrar titulares removidos
            foreach (var titularActual in titularesActuales)
            {
                if (titularActual.PersonaId.HasValue &&
                    !idsNuevos.Contains(titularActual.PersonaId.Value))
                {
                    titularActual.FechaFin = DateTime.Now;
                }
            }
        }


        private async Task GenerarNotaRecordatorio(string descripcionNota, string nombreNota, string vencimiento, string titularNota, int usuarioId, string pago)
        {
            NotaDTO nota = new NotaDTO();
            nota.Nombre = nombreNota;
            nota.TipoNotaId = (int)TipoNotaEnum.Recordatorio;
            nota.Descripcion = descripcionNota;
            nota.Color = "#F5DADE";
            nota.Visibilidad = true;
            nota.EstadoId = (int)EstadosNotaEnum.NotaPendiente;
            nota.FechaCreacion = DateTime.Now;
            nota.UsurioId = usuarioId;
            nota.FechaFinRecordatorio = DateTime.Now.AddDays(10);
            nota.Tareas = new List<TareaDTO>
                {
                    new() { Descripcion = vencimiento, Estado = false },
                    new() { Descripcion = titularNota, Estado = false },
                    new() { Descripcion = pago, Estado = false },
                    new() { Descripcion = "Modificar contribuyente en Program", Estado = false },
                    new() { Descripcion = "Modificar deudo en Program", Estado = false },
                    new() { Descripcion = "Modificar celular / mail en Program", Estado = false }

                };

            int tramiteNotaId = await _notasService.GenerarTramiteNota(usuarioId);
            await _notasService.GenerarNotaSinTransaccion(tramiteNotaId, nota);
        }



        public async Task<List<TablaConcesionDTO>> GetAllParaExportar(
    int filtroEstado = 0,
    string nombre = "",
    string apellido = "",
    int concesion = 0,
    int? tipoParcelaID = null,
    int? seccionID = null,
    int? parcelaID = null,
    DateOnly? fechaDesde = null,
    DateOnly? fechaHasta = null)
        {
            var query = _context.Concesiones
                .Where(v => v.Visibilidad.HasValue)
                .AsQueryable().AsNoTracking();

            // ---- mismos filtros que GellAllPaginado ----
            switch (filtroEstado)
            {
                case 5: query = query.Where(e => e.Tramite.EstadoActualId == (int)EstadosConcesionEnum.SinContrato); break;
                case 6: query = query.Where(e => e.Tramite.EstadoActualId == (int)EstadosConcesionEnum.Vigente); break;
                case 7: query = query.Where(e => e.Tramite.EstadoActualId == (int)EstadosConcesionEnum.Vencido); break;
                case 8: query = query.Where(e => e.Tramite.EstadoActualId == (int)EstadosConcesionEnum.Caducado); break;
            }

            if (concesion > 0)
                query = query.Where(c => c.Concesion == concesion);

            if (fechaDesde.HasValue)
                query = query.Where(x => x.Vencimiento >= fechaDesde);

            if (fechaHasta.HasValue)
                query = query.Where(x => x.Vencimiento < fechaHasta.Value.AddDays(1));

            if (tipoParcelaID.HasValue)
                query = query.Where(c => c.Parcela.TipoParcelaId == tipoParcelaID.Value);

            if (seccionID.HasValue)
                query = query.Where(c => c.Parcela.SeccionId == seccionID.Value);

            if (parcelaID.HasValue)
                query = query.Where(c => c.ParcelaId == parcelaID.Value);

            if (!string.IsNullOrWhiteSpace(nombre) || !string.IsNullOrWhiteSpace(apellido))
            {
                query = query.Where(c =>
                    c.HistorialTitularesConcesiones.Any(h =>
                        h.FechaFin == null &&
                        (string.IsNullOrWhiteSpace(nombre) || h.Persona.Nombre.Contains(nombre)) &&
                        (string.IsNullOrWhiteSpace(apellido) || h.Persona.Apellido.Contains(apellido)))
                    ||
                    (c.FechaFin == null &&
                     c.Parcela.ParcelaDifuntos.Any(pd =>
                        pd.FechaRetiro == null &&
                        (string.IsNullOrWhiteSpace(nombre) || pd.Difunto.Nombre.Contains(nombre)) &&
                        (string.IsNullOrWhiteSpace(apellido) || pd.Difunto.Apellido.Contains(apellido))))
                );
            }
            // ---- fin filtros ----

            var datos = await query
                .OrderByDescending(c => c.TramiteId)
                .Select(c => new
                {
                    c.TramiteId,
                    c.Concesion,
                    c.Visibilidad,
                    c.ParcelaId,
                    c.Vencimiento,
                    c.FechaFin,
                    c.Tramite.EstadoActualId,
                    TipoParcelaId = c.Parcela.TipoParcelaId,
                    NombreSeccion = c.Parcela.Seccion.Nombre,
                    c.Parcela.NroParcela,
                    c.Parcela.NroFila
                })
                .ToListAsync();

            var tramiteIds = datos.Select(c => c.TramiteId).ToList();
            var parcelaIds = datos.Select(c => c.ParcelaId).ToList();

            var titulares = await _context.HistorialTitularesConcesiones
                .Where(h => tramiteIds.Contains(h.ConcesionId ?? 0) && h.FechaFin == null)
                .Select(h => new { h.ConcesionId, Nombre = h.Persona.Nombre ?? "", Apellido = h.Persona.Apellido ?? "", h.Persona.Celular, h.Persona.Correo, h.Persona.Sexo })
                .ToListAsync();

            var difuntos = await _context.ParcelaDifuntos
                .Where(pd => parcelaIds.Contains(pd.ParcelaId) && pd.FechaRetiro == null)
                .Select(pd => new { pd.ParcelaId, Nombre = pd.Difunto.Nombre ?? "", Apellido = pd.Difunto.Apellido ?? "" })
                .ToListAsync();

            return datos.Select(c => new TablaConcesionDTO
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
                    .Select(t => new PersonaTablaGeneral { Nombre = t.Nombre, Apellido = t.Apellido, celular = t.Celular ?? "", correo = t.Correo ?? "", sexo = t.Sexo ?? "" })
                    .ToList(),
                Difuntos = c.FechaFin == null
                    ? difuntos
                        .Where(d => d.ParcelaId == c.ParcelaId)
                        .Select(d => new PersonaTablaGeneral { Nombre = d.Nombre, Apellido = d.Apellido })
                        .ToList()
                    : new()
            }).ToList();
        }

        public async Task TrasladarDifuntoManualmente(int difuntoId, int parcelaNuevaId, int parcelaAntiguaId, int concesionNuevaId, int conesionAntiguaId, DateTime? fechaInicio)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Models.Persona difunto = await _context.Personas.FirstOrDefaultAsync(p => p.Id == difuntoId) ?? throw new Exception("Difunto no encontrado.");
                Models.Parcela parcelaNueva = await _context.Parcelas.Include(s => s.Seccion).FirstOrDefaultAsync(p => p.Id == parcelaNuevaId) ?? throw new Exception("Parcela nueva no encontrada.");
                Models.Parcela parcelaAntigua = await _context.Parcelas.Include(s => s.Seccion).FirstOrDefaultAsync(p => p.Id == parcelaAntiguaId) ?? throw new Exception("Parcela antigua no encontrada.");
                Models.Concesione concesionAntigua = await _context.Concesiones.FirstOrDefaultAsync(c => c.TramiteId == conesionAntiguaId) ?? throw new Exception("Concesión antigua no encontrada.");
                Models.Concesione concesionNueva = await _context.Concesiones.FirstOrDefaultAsync(c => c.TramiteId == concesionNuevaId) ?? throw new Exception("Concesión nueva no encontrada.");

                Models.ParcelaDifunto parcelaDifunto = await _context.ParcelaDifuntos
                        .FirstOrDefaultAsync(pd => pd.ParcelaId == parcelaAntigua.Id && pd.DifuntoId == difunto.Id && pd.FechaRetiro == null) ?? throw new Exception("Registro de parcela-difunto no encontrado.");

                parcelaDifunto.FechaRetiro = DateTime.Now;
                parcelaAntigua.CantidadDifuntos -= 1;


                //5.1 pasos por si queda la parcela vacia.
                if (parcelaAntigua.CantidadDifuntos == 0)
                {
                    // cancelar la concesion.
                    concesionAntigua.FechaFin = DateTime.Now;

                    Models.Tramite tramiteConcesion = await _context.Tramites.FirstOrDefaultAsync(t => t.Id == concesionAntigua.TramiteId) ?? throw new Exception("Trámite no encontrado.");


                    tramiteConcesion.EstadoActualId = (int)EstadosTramiteEnum.Caducado;
                    concesionAntigua.Vencimiento = null;
                    tramiteConcesion.FechaFinalizacion = DateTime.Now;
                    HistorialEstadosDTO historialConcesion = new HistorialEstadosDTO
                    {
                        Fecha =  DateTime.Now,
                        TramiteId = tramiteConcesion.Id,
                        EstadoTramiteId = (int)EstadosTramiteEnum.Caducado
                    };
                    await _historialEstadosService.Add(historialConcesion);

                    concesionAntigua.InformacionAdicional += $"\n● La concesión ({concesionAntigua.Concesion?.ToString("D5")}) ha sido cancelada/caducada automáticamente por no tener más difuntos asociados.";

                    // 1. Titulares actuales activos
                    var titularesActuales = await _context.HistorialTitularesConcesiones
                        .Where(p => p.ConcesionId == concesionAntigua.TramiteId && p.FechaFin == null)
                        .ToListAsync();

                    // 3. Cerrar titulares
                    foreach (var titularActual in titularesActuales)
                    {
                        titularActual.FechaFin = DateTime.Now;
                    }
                }

                //7 - generar nuevo registro de parcela-difunto con la nueva parcela destino.
                Models.ParcelaDifunto nuevoParcelaDifunto = new Models.ParcelaDifunto
                {
                    DifuntoId = difunto.Id,
                    ParcelaId = parcelaNueva.Id,
                    FechaIngreso = fechaInicio ?? DateTime.Now,
                    TramiteIngresoId = null
                };
                await _context.ParcelaDifuntos.AddAsync(nuevoParcelaDifunto);

                //8 - actualizar la cantidad de difuntos en la parcela destino.
                parcelaNueva.CantidadDifuntos += 1;
                difunto.CategoriaPersonaId = (int)CategoriaPersonaEnum.Fallecido;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

        public async Task CaducarConcesion(int concesionId)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                Models.Concesione concesion = await _context.Concesiones
                   .FirstOrDefaultAsync(c => c.TramiteId == concesionId) ?? throw new Exception("Concesion no encontrada.");

                Models.Parcela parcela = await _context.Parcelas.FirstOrDefaultAsync(p => p.Id == concesion.ParcelaId) ?? throw new Exception("Parcela no encontrada.");

                concesion.FechaFin = DateTime.Now;

                Models.Tramite tramiteConcesion = await _context.Tramites.FirstOrDefaultAsync(t => t.Id == concesion.TramiteId) ?? throw new Exception("Trámite no encontrado.");


                tramiteConcesion.EstadoActualId = (int)EstadosTramiteEnum.Caducado;
                concesion.Vencimiento = null;
                tramiteConcesion.FechaFinalizacion = DateTime.Now;

                HistorialEstadosDTO historialConcesion = new HistorialEstadosDTO
                {
                    Fecha = DateTime.Now,
                    TramiteId = tramiteConcesion.Id,
                    EstadoTramiteId = (int)EstadosTramiteEnum.Caducado
                };
                await _historialEstadosService.Add(historialConcesion);

                concesion.InformacionAdicional += $"\n●El {DateTime.Now.ToString("dd/MM/yyyy HH:mm")} la concesión ({concesion.Concesion?.ToString("D5")}) ha sido cancelada/caducada manualmente.";

                // 1. Titulares actuales activos
                var titularesActuales = await _context.HistorialTitularesConcesiones
                    .Where(p => p.ConcesionId == concesion.TramiteId && p.FechaFin == null)
                    .ToListAsync();

                // 3. Cerrar titulares
                foreach (var titularActual in titularesActuales)
                {
                    titularActual.FechaFin = DateTime.Now;
                }

                // 4. Retirar difuntos asociados a la parcela
                var parcelaDifuntos = await _context.ParcelaDifuntos
                    .Where(pd => pd.ParcelaId == concesion.ParcelaId && pd.FechaRetiro == null)
                    .ToListAsync();

                foreach (var pd in parcelaDifuntos)
                {
                    pd.FechaRetiro = DateTime.Now;
                    pd.TramiteRetiroId = concesion.TramiteId;
                }

                parcela.CantidadDifuntos = 0;

                // 4. Cerrar el historial de parcela de la concesión
                var historialParcela = await _context.HistorialParcelasConcesions
                    .FirstOrDefaultAsync(h =>
                        h.ConcesionId == concesion.TramiteId &&
                        h.FechaFin == null
                    );

                if (historialParcela != null)
                {
                    historialParcela.FechaFin = DateTime.Now;
                    historialParcela.TramiteOrigenId = concesion.TramiteId; // el trámite que generó el cierre
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Error al caducar la concesión: " + ex.Message);
            }
        }

        public async Task<GenericResultDTO> AddManualmente(ConcesionDTO dto)
        {
            try
            {
                Models.Parcela parcela = await _context.Parcelas.FirstOrDefaultAsync(p => p.Id == dto.ParcelaId) ?? throw new Exception("Parcela no encontrada.");

                //1- registrar tramite
                TramiteDTO tramite = new TramiteDTO
                {
                    Visibilidad = true,
                    FechaCreacion = DateTime.Now,
                    TipoTramiteId = (int)TipoTramiteEnum.ContratoConcesion,
                    UsuarioId = dto.UsuarioId ?? 0,
                    EstadoActualId = parcela.TipoParcelaId == (int)TipoParcelaEnum.Panteon
                                ? (int)EstadosConcesionEnum.Vigente
                                : (int)EstadosConcesionEnum.SinContrato,
                };

                int tramiteId = await _tramiteService.Add(tramite);
                await _context.SaveChangesAsync(); //guardar cambios antes de continuar

                //2- registrar Historial del tramite
                HistorialEstadosDTO historial = new HistorialEstadosDTO
                {
                    Fecha = tramite.FechaCreacion,
                    TramiteId = tramiteId,
                    EstadoTramiteId = tramite.EstadoActualId
                };
                await _historialEstadosService.Add(historial);

                //3 - registrar el contrato de concesion
                Models.Concesione concesion = new Models.Concesione();
                concesion.TramiteId = tramiteId;
                concesion.Concesion = dto.Concesion;
                concesion.Precio = dto.Precio;
                concesion.Visibilidad = true;
                concesion.TipoParcela = EnumHelper.GetDisplayNameByValue<TipoParcelaEnum>(parcela.TipoParcelaId ?? 0);
                concesion.Vencimiento = dto.Vencimiento;
                concesion.ParcelaId = dto.ParcelaId;
                concesion.CantidadAniosId = dto.CantidadAniosId;
                concesion.CuotaId = dto.CuotaId;
                concesion.UsuarioId = dto.UsuarioId;
                concesion.FechaInicio = dto.FechaInicio ?? DateTime.Now;
                concesion.InformacionAdicional += dto.InformacionAdicional;
                await _context.Concesiones.AddAsync(concesion);

                //4 - relacion de tramite con parcela
                await _historialEstadosService.VincularTramiteAParcela(tramiteId, dto.ParcelaId);

                //4.1 - registrar historial de parcela en la concesion
                Models.HistorialParcelasConcesion historialParcela = new Models.HistorialParcelasConcesion
                {
                    ConcesionId = tramiteId,
                    ParcelaId = dto.ParcelaId,
                    FechaInicio = DateTime.Now,
                    FechaFin = null,           // null = parcela actualmente vinculada
                    TramiteOrigenId = tramiteId
                };
                await _context.HistorialParcelasConcesions.AddAsync(historialParcela);


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
    }
}
