using CemSys3.DTOs.ConceptosTarifaria;
using CemSys3.DTOs.Paginacion;
using CemSys3.Interfaces.ConceptoTarifaria;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace CemSys3.Business.ConceptoTarifaria
{
    public class ConceptoTarifariaService : IConceptoTarifaria
    {
        private readonly AppDbContext _context;

        public ConceptoTarifariaService(AppDbContext context)
        {
            _context = context;   
        }
        public async Task Add(ConceptoTarifariaDTO dto) //Se debe agregar el concepto a la tarifaria
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                ConceptosTarifarium concepto = new ConceptosTarifarium();

                concepto.Nombre = dto.Nombre.Trim();
                concepto.Visibilidad = true;
                concepto.TemaId = dto.TemaId;

                //agrego el concepto nuevo
                await _context.AddAsync(concepto);

                await _context.SaveChangesAsync();

                //inicializo en $0 el nuevo concepto en la tarifaria
                PreciosTarifaria precio = new PreciosTarifaria();
                precio.Precio = 0m;
                precio.Visibilidad = true;
                precio.ConceptoTarifariaId = concepto.Id;

                await _context.PreciosTarifarias.AddAsync(precio);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception) {
                await transaction.RollbackAsync();
                throw;
            }
            
        }
        //cuando se elimina un concepto, se deben ocultar de la tarifaria todos los precios con ese concepto.
        public async Task Delete(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                //oculta el concepto
                ConceptosTarifarium concepto = await _context.ConceptosTarifaria.FindAsync(id) ?? throw new Exception("Concepto de la tarifaria no encontrado");
                concepto.Visibilidad = false;

                //recorre todos los precios con este concepto y los desabilita
                await _context.PreciosTarifarias.Where(p => p.Visibilidad == true && p.ConceptoTarifariaId == concepto.Id)
                    .ExecuteUpdateAsync(s => s.SetProperty(x => x.Visibilidad, false));

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }catch(Exception)
            {
                await transaction.RollbackAsync(); 
                throw; 
            }
            
        }

        public async Task<ConceptoTarifariaDTO> Get(int id)
        {
            return await _context.ConceptosTarifaria.Where(ct => ct.Id == id).Select(c => new ConceptoTarifariaDTO
            {
                Id = id,
                Nombre = c.Nombre,
                Visibilidad = c.Visibilidad,
                TemaId = c.TemaId
            }).FirstOrDefaultAsync();   
        }

        public async Task<PaginadoResponse<ConceptoTarifariaDTO>> GetAllPaginado(string? filtro = null, int pagina = 1, int porPagina = 10)
        {
            PaginadoResponse<ConceptoTarifariaDTO> resultado = new PaginadoResponse<ConceptoTarifariaDTO>();

            IQueryable<ConceptosTarifarium> query = _context.ConceptosTarifaria.AsQueryable();

            // Filtros
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(e => e.Nombre.Contains(filtro));
            }

            // Total de registros
            int total = await query.CountAsync();

            // Paginación
            resultado.Paginacion.TotalPaginas = (int)Math.Ceiling(total / (double)porPagina);
            resultado.Paginacion.PaginaActual = Math.Max(1, Math.Min(pagina, resultado.Paginacion.TotalPaginas));
            resultado.Paginacion.RegistrosPorPagina = porPagina;
            resultado.Paginacion.Accion = "Index";
            resultado.Paginacion.Controlador = "ConceptoTarifaria";
            resultado.Paginacion.TotalRegistros = total;

            resultado.Items = await query
                .OrderBy(e => e.TemaId)
                .Skip((resultado.Paginacion.PaginaActual - 1) * porPagina)
                .Take(porPagina)
                .Select(e => new ConceptoTarifariaDTO
                {
                    Id = e.Id,
                    Nombre = e.Nombre,
                    Visibilidad = e.Visibilidad,
                    TemaId = e.TemaId,
                }).ToListAsync();

            return resultado;
        }

        public async Task Update(ConceptoTarifariaDTO dto)
        {
            ConceptosTarifarium concepto = await _context.ConceptosTarifaria.FindAsync(dto.Id) ?? throw new Exception("Concepto de la tarifaria no encontrado");
            concepto.Nombre = dto.Nombre.Trim();
            concepto.TemaId = dto.TemaId;
            await _context.SaveChangesAsync();
        }
    }
}
