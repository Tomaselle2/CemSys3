using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.Parcela;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.Seccion;
using CemSys3.DTOs.Tramite;
using CemSys3.Enumerables;
using CemSys3.Interfaces.Parcela;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;


namespace CemSys3.Business.Parcela
{
    public class ParcelaService : IParcela
    {
        private readonly AppDbContext _context;
        public ParcelaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(SeccionRequestDTO dto)
        {
            switch (dto.TipoParcelaId)
            {
                case 1: // Nicho
                    RegistrarNichos(dto);
                    break;
                case 2: //fosa
                    RegistrarFosas(dto);
                    break;
                case 3: //panteon
                    RegistrarPanteones(dto);
                    break;
            }
        }

        private void RegistrarNichos(SeccionRequestDTO dto)
        {
            int filas = dto.Filas;
            int columnas = dto.NroParcelas / filas;
            int nroNichoContador = 1;

            switch (dto.TipoNumeracionParcelaId)
            {
                case 1: //numeracion nueva
                    for (int i = 1; i <= filas; i++)
                    {
                        for (int j = 1; j <= columnas; j++)
                        {
                            Models.Parcela nicho = new Models.Parcela();
                            nicho.NroFila = i;
                            nicho.NroParcela = j;
                            nicho.Visibilidad = true;
                            nicho.CantidadDifuntos = 0;
                            nicho.TipoNichoId = (int)TipoNichoEnum.Feretro; //valor por defecto
                            nicho.SeccionId = dto.Id;
                            nicho.TipoParcelaId = dto.TipoParcelaId;
                            nicho.InformacionAdicional = string.Empty;
                            nicho.NombrePanteon = string.Empty;

                            _context.Parcelas.Add(nicho);
                        }
                    }
                    break;
                case 2: 
                    for (int columna = 1; columna <= columnas; columna++)
                    {
                        for (int fila = 1; fila <= filas; fila++)
                        {
                            Models.Parcela nicho = new Models.Parcela();

                            nicho.NroFila = fila;
                            nicho.NroParcela = nroNichoContador;
                            nicho.Visibilidad = true;
                            nicho.CantidadDifuntos = 0;
                            nicho.TipoNichoId = (int)TipoNichoEnum.Feretro;
                            nicho.SeccionId = dto.Id;
                            nicho.TipoParcelaId = dto.TipoParcelaId;
                            nicho.InformacionAdicional = string.Empty;
                            nicho.NombrePanteon = string.Empty;

                            _context.Parcelas.Add(nicho);

                            nroNichoContador++;
                        }
                    }
                    break;
            }
        }

        private void RegistrarFosas(SeccionRequestDTO dto)
        {
            for (int i = 1; i <= dto.NroParcelas; i++)
            {
                Models.Parcela fosa = new Models.Parcela();
                fosa.NroParcela = i;
                fosa.SeccionId = dto.Id;
                fosa.Visibilidad = true;
                fosa.CantidadDifuntos = 0;
                fosa.NroFila = dto.Filas;
                fosa.TipoParcelaId = dto.TipoParcelaId;
                fosa.InformacionAdicional = string.Empty;
                fosa.NombrePanteon = string.Empty;

                _context.Parcelas.Add(fosa);
            }
        }

        private void RegistrarPanteones(SeccionRequestDTO dto)
        {
            for (int i = 1; i <= dto.NroParcelas; i++)
            {
                Models.Parcela panteon = new Models.Parcela();
                panteon.NroParcela = i;
                panteon.SeccionId = dto.Id;
                panteon.Visibilidad = true;
                panteon.CantidadDifuntos = 0;
                panteon.NroFila = dto.Filas;
                panteon.TipoPanteonId = (int)TipoPanteonEnum.ConNichos;
                panteon.TipoParcelaId = dto.TipoParcelaId;
                panteon.InformacionAdicional = string.Empty;
                panteon.NombrePanteon = string.Empty;

                _context.Parcelas.Add(panteon);
            }
        }

        public async Task<GenericResultDTO> AddOne(int secccionId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Seccione? seccion = await _context.Secciones.FindAsync(secccionId);
                if (seccion == null)
                    throw new Exception("La seccion no existe");

                Models.Parcela parcela = new Models.Parcela();
                parcela.NroParcela = seccion.NroParcelas + 1; //el nro de parcela es el nro de parcelas actual + 1
                parcela.SeccionId = secccionId;
                parcela.Visibilidad = true;
                parcela.CantidadDifuntos = 0;
                parcela.NroFila = seccion.Filas;
                parcela.InformacionAdicional = string.Empty;
                parcela.NombrePanteon = string.Empty;
                parcela.TipoParcelaId = seccion.TipoParcelaId;

                //se actualiza el nro de parcelas de la seccion
                seccion.NroParcelas += 1;

                //si es panteon, se asigna el tipo de panteon por defecto
                if (seccion.TipoParcelaId == (int)TipoParcelaEnum.Panteon)
                {
                    parcela.TipoPanteonId = (int)TipoPanteonEnum.ConNichos;
                }

                await _context.Parcelas.AddAsync(parcela);

                //se guarda todo el contexto. 
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new GenericResultDTO()
                {
                    Success = true,
                    Message = "Parcela agregada correctamente",
                    Id = parcela.Id
                };
            }
            catch (Exception) 
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PaginadoResponse<ParcelaIndexRequestDTO>> GetAllPaginadoBySeccion(int seccionId, int filtro = 0, int pagina = 1, int porPagina = 10)
        {
            PaginadoResponse<ParcelaIndexRequestDTO> resultado = new PaginadoResponse<ParcelaIndexRequestDTO>();

            var query = _context.Parcelas
               .Where(s =>s.SeccionId == seccionId);

            // Filtro por estado de ocupación
            switch (filtro)
            {
                case 1: //ocupados
                    query = query.Where(e => e.CantidadDifuntos > 0);
                    break;
                case 2: //desocupados o libres
                    query = query.Where(e => e.CantidadDifuntos == 0);
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
            resultado.Paginacion.Accion = "Index";
            resultado.Paginacion.Controlador = "Parcela";
            resultado.Paginacion.TotalRegistros = total;

            // Obtener datos paginados
            resultado.Items = await query
                .OrderBy(e => e.Id)
                .Skip((resultado.Paginacion.PaginaActual - 1) * porPagina)
                .Take(porPagina)
                .Select(e => new ParcelaIndexRequestDTO
                {
                    Id = e.Id,
                    Visibilidad = e.Visibilidad,
                    NroParcela = e.NroParcela,
                    NroFila = e.NroFila,
                    CantidadDifuntos = e.CantidadDifuntos,
                    NombrePanteon = e.NombrePanteon,
                    SeccionId = e.SeccionId,
                    TipoNichoId = e.TipoNichoId,
                    TipoPanteonId = e.TipoPanteonId,
                    TipoParcelaId = e.TipoParcelaId,
                }).ToListAsync();

            return resultado;
        }

        //para vista parcial de ingreso
        public async Task<IEnumerable<ParcelaIndexRequestDTO>> GetAllBySeccionId(int seccionId, int estadoDifunto)
        {
            int tipoParcelaId = _context.Secciones
                .Where(s => s.Id == seccionId)
                .Select(s => s.TipoParcelaId)
                .FirstOrDefault();

            var query = _context.Parcelas
               .Where(s => s.SeccionId == seccionId);

            if(estadoDifunto == (int)EstadoDifuntoEnum.CuerpoCompleto && tipoParcelaId == (int)TipoParcelaEnum.Nicho)
            {
                query = query.Where(p => p.CantidadDifuntos == 0 && p.TipoNichoId != (int)TipoNichoEnum.Urnario);
            }

            return await query.Select(e => new ParcelaIndexRequestDTO
            {
                Id = e.Id,
                Visibilidad = e.Visibilidad,
                NroParcela = e.NroParcela,
                NroFila = e.NroFila,
                CantidadDifuntos = e.CantidadDifuntos,
                NombrePanteon = e.NombrePanteon,
                SeccionId = e.SeccionId,
                TipoNichoId = e.TipoNichoId,
                TipoPanteonId = e.TipoPanteonId,
                TipoParcelaId = e.TipoParcelaId,
            }).ToListAsync();
        }

        public async Task AumentarDifunto(int parcelaId)
        {
            Models.Parcela parcela = await _context.Parcelas.FindAsync(parcelaId) ?? throw new Exception("La parcela no existe");
            parcela.CantidadDifuntos += 1;
        }

        public async Task<ParcelaHistorialDTO> HistorialParcela(int parcelaId)
        {
            Models.Parcela parcela = await _context.Parcelas.Where(pa => pa.Id == parcelaId).Include(p => p.Seccion).FirstOrDefaultAsync() ?? throw new Exception("No se encontro la parcela.");
            ParcelaHistorialDTO historial = new ParcelaHistorialDTO();
            historial.Id = parcela.Id;
            historial.NroParcela = parcela.NroParcela;
            historial.NroFila = parcela.NroFila;
            historial.NombreSeccion = parcela.Seccion.Nombre;
            historial.TipoParcelaId = parcela.TipoParcelaId ?? 0;
            historial.TipoNichoId = parcela.TipoNichoId ?? 0;
            historial.TipoPanteonId = parcela.TipoPanteonId ?? 0;
            historial.NombrePanteon = parcela.NombrePanteon;
            historial.infoAdicional = parcela.InformacionAdicional;
            historial.CantidadDifuntosActuales = parcela.CantidadDifuntos;

            //historial de tramites parcelas
            historial.Tramites = await _context.TramitesParcelas.Where(p => p.ParcelaId == parcelaId).OrderByDescending(t=>t.FechaRegistro).Select(s => new TramiteDTO
            {
                Id = s.TramiteId,
                Visibilidad = s.Tramite.Visibilidad,
                FechaCreacion = s.Tramite.FechaCreacion,
                TipoTramiteId = s.Tramite.TipoTramiteId,
                EstadoActualId = s.Tramite.EstadoActualId
            }).ToListAsync();

            //historial de difuntos actuales
            historial.DifuntosActuales = await _context.ParcelaDifuntos
                .Where(p => p.ParcelaId == parcelaId && p.FechaRetiro == null)
                .OrderByDescending(t => t.FechaIngreso)
                .Select(f => new DifuntoHistorialParcelaDTO
                {
                    Id = f.Difunto.Id,
                    FechaIngreso = f.FechaIngreso,
                    FechaRetiro = f.FechaRetiro,
                    Dni = f.Difunto.Dni,
                    Nombre = f.Difunto.Nombre,
                    Apellido = f.Difunto.Apellido,
                    EstadoDifunto = f.Difunto.EstadoDifuntoId
                }).ToListAsync();

            //historial de difuntos historicos
            historial.DifuntosHistoricos = await _context.ParcelaDifuntos.Where(p => p.ParcelaId == parcelaId).OrderByDescending(t => t.FechaIngreso).Select(f => new DifuntoHistorialParcelaDTO
            {
                Id = f.Difunto.Id,
                FechaIngreso = f.FechaIngreso,
                FechaRetiro = f.FechaRetiro,
                Dni = f.Difunto.Dni,
                Nombre = f.Difunto.Nombre,
                Apellido = f.Difunto.Apellido,
                EstadoDifunto = f.Difunto.EstadoDifuntoId
            }).ToListAsync();

            return historial;
        }

        public async Task UpdateParcela(ModificarParcelaDTO dto)
        {
            Models.Parcela parcela = await _context.Parcelas.FindAsync(dto.Id) ?? throw new Exception("La parcela no existe");

            parcela.NombrePanteon = dto.NombrePanteon ?? parcela.NombrePanteon;
            parcela.InformacionAdicional = dto.infoAdicional ?? parcela.InformacionAdicional;
            parcela.TipoNichoId = dto.TipoNichoId ?? parcela.TipoNichoId;
            parcela.TipoPanteonId = dto.TipoPanteonId ?? parcela.TipoPanteonId;
            
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ParcelaDTO>> GetAllNichosDisponibles()
        {
            return await _context.Parcelas.Where(p => p.TipoParcelaId == (int)TipoParcelaEnum.Nicho && p.CantidadDifuntos == 0 && p.Visibilidad).AsNoTracking()
                .Select(p => new ParcelaDTO
                {
                    Id = p.Id,
                    NroParcela = p.NroParcela,
                    NroFila = p.NroFila,
                    NombreSeccion = p.Seccion.Nombre,
                    SeccionId = p.SeccionId,
                    TipoParcelaId = p.TipoParcelaId ?? 0,
                    TipoNichoId = p.TipoNichoId ?? 0
                }).ToListAsync();
        }

        public async Task<bool> ParcelaTieneConcesion(int parcelaId)
        {
            
            Models.Parcela parcela = await _context.Parcelas.FindAsync(parcelaId) ?? throw new Exception("La parcela no existe");

            if(parcela.CantidadDifuntos == 0)
            {
                return false;
            }

            return await _context.Concesiones
                .AnyAsync(c =>
                    c.ParcelaId == parcelaId &&
                    c.Visibilidad == true &&
                    c.TramiteRetiroId == null &&
                    (c.FechaFin == null || c.FechaFin > DateTime.Now)
                );
        }
    }
}
