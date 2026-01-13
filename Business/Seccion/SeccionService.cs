using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.Seccion;
using CemSys3.Interfaces.Seccion;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CemSys3.Business.Seccion
{
    public class SeccionService : ISeccion
    {
        private readonly AppDbContext _context;
        public SeccionService(AppDbContext context)
        {
            _context = context;
        }

        //esto se convina con el servicio de parcelas para crearlas automaticamente. y agrega la seccion a la tarifaria vigente en caso de existir. Es una transaccion.
        public async Task<GenericResultDTO> Add(SeccionRequestDTO dto)  
        {
            Models.Seccione seccion = new Models.Seccione();

            seccion.Nombre = dto.Nombre.Trim();
            seccion.Visibilidad = true;
            seccion.Filas = dto.Filas;
            seccion.NroParcelas = dto.NroParcelas;
            seccion.TipoNumeracionParcelaId = dto.TipoNumeracionParcelaId;
            seccion.TipoParcelaId = dto.TipoParcelaId;

            _context.Secciones.Add(seccion);

            await _context.SaveChangesAsync();

            return new GenericResultDTO
            {
                Success = true,
                Message = "Sección registrada correctamente.",
                Id = seccion.Id
            };
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

            // Paginación
            resultado.Paginacion.TotalPaginas = (int)Math.Ceiling(total / (double)porPagina);
            resultado.Paginacion.PaginaActual = Math.Max(1, Math.Min(pagina, resultado.Paginacion.TotalPaginas));
            resultado.Paginacion.RegistrosPorPagina = porPagina;
            resultado.Paginacion.Accion = "Index"; //cambiar
            resultado.Paginacion.Controlador = "EmpresaSepelio"; //cambiar
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
    }
}
