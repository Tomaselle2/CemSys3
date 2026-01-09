using CemSys3.DTOs.Cementerio;
using CemSys3.DTOs.Paginacion;
using CemSys3.Interfaces.Cementerio;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.Cementerio
{
    public class CementerioService : ICementerio
    {
        private readonly AppDbContext _context;

        public CementerioService(AppDbContext context)
        {
            _context = context;
        }

        public async Task Add(CementerioRequestDTO cementerio)
        {
            _context.Cementerios.Add(new Models.Cementerio
            {
                Nombre = cementerio.Nombre,
                Visibilidad = true
            });

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            Models.Cementerio cementerio = await _context.Cementerios.FindAsync(id) ?? throw new KeyNotFoundException("Cementerio no encontrado");
            cementerio.Visibilidad = false;
            await _context.SaveChangesAsync();
        }

        public async Task<PaginadoResponse<CementerioRequestDTO>> GetAllPaginado(string? filtro = null, int pagina = 1, int porPagina = 10)
        {
            PaginadoResponse<CementerioRequestDTO> resultado = new PaginadoResponse<CementerioRequestDTO>();

            var query = _context.Cementerios
                .Where(e => e.Visibilidad);

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
            resultado.Paginacion.Accion = "Index";
            resultado.Paginacion.Controlador = "Cementerio";
            resultado.Paginacion.TotalRegistros = total;

            // Obtener datos paginados
            resultado.Items = await query
                .OrderBy(e => e.Nombre)
                .Skip((resultado.Paginacion.PaginaActual - 1) * porPagina)
                .Take(porPagina)
                .Select(e => new CementerioRequestDTO
                {
                    Id = e.Id,
                    Nombre = e.Nombre
                })
                .ToListAsync();

            return resultado;
        }

        public async Task<CementerioRequestDTO> GetById(int id)
        {
            return await _context.Cementerios.Where(e => e.Id == id).Select(e => new CementerioRequestDTO
            {
                Id = e.Id,
                Nombre = e.Nombre
            }).FirstOrDefaultAsync() ?? throw new KeyNotFoundException("Cementerio no encontrado");
        }

        public async Task Update(CementerioRequestDTO cementerio)
        {
            Models.Cementerio cementerioExistente = await _context.Cementerios.FindAsync(cementerio.Id) ?? throw new KeyNotFoundException("Cementerio no encontrado");
            cementerioExistente.Nombre = cementerio.Nombre.Trim();
            await _context.SaveChangesAsync();
        }
    }
}
