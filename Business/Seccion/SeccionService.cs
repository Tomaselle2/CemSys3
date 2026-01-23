
using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.Seccion;
using CemSys3.Enumerables;
using CemSys3.Helpers.Enumerable;
using CemSys3.Interfaces.Parcela;
using CemSys3.Interfaces.Seccion;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CemSys3.Business.Seccion
{
    public class SeccionService : ISeccion, ISeccionNichoTarifaria
    {
        private readonly AppDbContext _context;
        private readonly IParcela _parcelaService;
        public SeccionService(AppDbContext context, IParcela parcelaService)
        {
            _context = context;
            _parcelaService = parcelaService;
        }

        //esto se convina con el servicio de parcelas para crearlas automaticamente. y agrega los precios a la tarifaria. Es una transaccion.
        public async Task<GenericResultDTO> Add(SeccionRequestDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                Models.Seccione seccion = new Models.Seccione();

                seccion.Nombre = dto.Nombre.Trim();
                seccion.Visibilidad = true;
                seccion.Filas = dto.Filas;
                seccion.NroParcelas = dto.NroParcelas;
                seccion.TipoNumeracionParcelaId = dto.TipoNumeracionParcelaId;
                seccion.TipoParcelaId = dto.TipoParcelaId;

                //se crea la seccion
                _context.Secciones.Add(seccion);

                await _context.SaveChangesAsync();

                //se obtiene el id de la seccion creada
                dto.Id = seccion.Id;

                //se crean las parcelas automaticamente
                await _parcelaService.Add(dto);

                //agrego los precios de la seccion a la tarifaria
                await AgregarPreciosConcesionNicho(seccion);

                //se guarda todo el contexto. 
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return new GenericResultDTO
                {
                    Success = true,
                    Message = "Sección registrada correctamente.",
                    Id = seccion.Id
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        //agrega los precios a la tarifaria de una nueva seccion
        private async Task AgregarPreciosConcesionNicho(Seccione seccion)
        {
            //obtiene todos los valores de los id de los años
            var valoresAnios = Enum.GetValues(typeof(AniosConcesionEnum));

            //obtengo el id de concepto tarifaria de concesion nicho
            int conceptoId = await _context.ConceptosTarifaria
                .Where(c => c.TemaId == (int)TemaTarifariaEnum.ConcesionNicho)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();            
            
            //recorro todas las filas
            for (int i = 0; i < seccion.Filas; i++)
            {
                //recorro todas las cantidades de años disponibles
                for (int j = 0; j < valoresAnios.Length; j++) {

                    PreciosTarifaria precio = new PreciosTarifaria
                    {
                        Precio = 0.00m,
                        NroFila = i+1,
                        ConceptoTarifariaId = conceptoId,
                        AniosConcesionId = j+1,
                        SeccionId = seccion.Id,
                        Visibilidad = true
                    };

                    await _context.PreciosTarifarias.AddAsync(precio);
                }
            }
        }

        //hay que recorrer todas las parcelas y asegurarse de que esten vacias antes de eliminar la seccion. Y colocar en false la visibilidad de todas las parcelas de esa seccion.
        public async Task Delete(int id) 
        {
            Seccione seccion = await _context.Secciones.FindAsync(id) ?? throw new KeyNotFoundException("Sección no encontrada");
            seccion.Visibilidad = false;
            await _context.SaveChangesAsync();
        }

        public async Task<SeccionRequestDTO> Get(int id)
        {
            return await _context.Secciones.Where(s => s.Id == id).Select(s => new SeccionRequestDTO
            {
                Id = s.Id,
                Nombre = s.Nombre,
                Visibilidad = s.Visibilidad,
                Filas = s.Filas,
                NroParcelas = s.NroParcelas,
                TipoNumeracionParcelaId = s.TipoNumeracionParcelaId,
                TipoParcelaId = s.TipoParcelaId
            }).FirstOrDefaultAsync();
        }

        public async Task<PaginadoResponse<SeccionRequestDTO>> GetAllByTipoPaginado(int tipoId, string? filtro = null, int pagina = 1, int porPagina = 10)
        {
            PaginadoResponse<SeccionRequestDTO> resultado = new PaginadoResponse<SeccionRequestDTO>();

            var query = _context.Secciones.Where(s => s.TipoParcelaId == tipoId);

            // Filtros
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(e => e.Nombre.Contains(filtro));
            }

            // Total de registros
            var total = await query.CountAsync();

            string vista = tipoId switch
            {
                (int)TipoParcelaEnum.Nicho => "IndexSeccionesNichos",
                (int)TipoParcelaEnum.Fosa => "IndexSeccionesFosas",
                (int)TipoParcelaEnum.Panteon => "IndexSeccionesPanteones",
                _ => "IndexSeccionesNichos"
            };

            // Paginación
            resultado.Paginacion.TotalPaginas = (int)Math.Ceiling(total / (double)porPagina);
            resultado.Paginacion.PaginaActual = Math.Max(1, Math.Min(pagina, resultado.Paginacion.TotalPaginas));
            resultado.Paginacion.RegistrosPorPagina = porPagina;
            resultado.Paginacion.Accion = vista; 
            resultado.Paginacion.Controlador = "Seccion"; 
            resultado.Paginacion.TotalRegistros = total;

            // Obtener datos paginados
            resultado.Items = await query
                .OrderBy(e => e.Nombre)
                .Skip((resultado.Paginacion.PaginaActual - 1) * porPagina)
                .Take(porPagina)
                .Select(e => new SeccionRequestDTO
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Visibilidad = e.Visibilidad,
                    Filas = e.Filas,
                    NroParcelas = e.NroParcelas,
                    TipoNumeracionParcelaId = e.TipoNumeracionParcelaId,
                    TipoParcelaId = e.TipoParcelaId
                })
                .ToListAsync();

            return resultado;
        }

        public async Task Update(SeccionRequestDTO dto) //solo se puede modificar el nombre de la seccion
        {
            Seccione seccion = await _context.Secciones.FindAsync(dto.Id) ?? throw new KeyNotFoundException("Sección no encontrada");
            seccion.Nombre = dto.Nombre.ToLower().Trim();
            await _context.SaveChangesAsync();
        }



        //-----------------------------------ISeccionNichoTarifaria----------------------------------------------------
        public async Task<IEnumerable<SeccionNichoTarifariaDTO>> GetAllSeccionesNichosParaTarifaria()
        {
            return await _context.Secciones.Where(s => s.Visibilidad == true && s.TipoParcelaId == (int)TipoParcelaEnum.Nicho).Select(sec => new SeccionNichoTarifariaDTO
            {
                Id = sec.Id,
                Nombre = sec.Nombre.ToUpper(),
                Filas = sec.Filas
            }).ToListAsync();
        }
    }
}
