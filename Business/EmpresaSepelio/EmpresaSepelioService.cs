using CemSys3.DTOs.EmpresaSepelio;
using CemSys3.DTOs.Paginacion;
using CemSys3.Interfaces.EmpresaSepelio;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.EmpresaSepelio
{
    public class EmpresaSepelioService : IEmpresaSepelio
    {
        private readonly AppDbContext _context;

        public EmpresaSepelioService(AppDbContext context)
        {
            _context = context;
        }
        public async Task Add(EmpresaSepelioRequestDTO empresa)
        {
            _context.EmpresasFunebres.Add(new Models.EmpresasFunebre
            {
                Nombre = empresa.Nombre,
                Visibilidad = true
            });

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            EmpresasFunebre empresa = await _context.EmpresasFunebres.FindAsync(id) ?? throw new KeyNotFoundException("Empresa no encontrada");
            empresa.Visibilidad = false;
            await _context.SaveChangesAsync();
        }

        public async Task<PaginadoResponse<EmpresaSepelioRequestDTO>> GetAllPaginado(string? filtro = null, int pagina = 1, int porPagina = 10)
        {
            PaginadoResponse<EmpresaSepelioRequestDTO> resultado = new PaginadoResponse<EmpresaSepelioRequestDTO>();

            var query = _context.EmpresasFunebres
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
            resultado.Paginacion.Controlador = "EmpresaSepelio";
            resultado.Paginacion.TotalRegistros = total;

            // Obtener datos paginados
            resultado.Items = await query
                .OrderBy(e => e.Nombre)
                .Skip((resultado.Paginacion.PaginaActual - 1) * porPagina)
                .Take(porPagina)
                .Select(e => new EmpresaSepelioRequestDTO
                {
                    Id = e.Id,
                    Nombre = e.Nombre
                })
                .ToListAsync();

            return resultado;
        }

        public async Task<EmpresaSepelioRequestDTO> GetById(int id)
        {
            return await _context.EmpresasFunebres.Where(e => e.Id == id).Select(e => new EmpresaSepelioRequestDTO
            {
                Id = e.Id,
                Nombre = e.Nombre
            }).FirstOrDefaultAsync() ?? throw new KeyNotFoundException("Empresa no encontrada");
                
        }

        public async Task Update(EmpresaSepelioRequestDTO empresa)
        {
            EmpresasFunebre empresaSepelio = await _context.EmpresasFunebres.FindAsync(empresa.Id) ?? throw new KeyNotFoundException("Empresa no encontrada");
            empresaSepelio.Nombre = empresa.Nombre.Trim();
            await _context.SaveChangesAsync();
        }
    }
}
