using CemSys3.DTOs.ConceptosTarifaria;
using CemSys3.DTOs.Paginacion;
using CemSys3.Interfaces.ConceptoTarifaria;
using CemSys3.Models;
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
                precio.ConceptoTarifariaId = concepto.Id;

                await _context.PreciosTarifarias.AddAsync(precio);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex) {
                await transaction.RollbackAsync();
                throw;
            }
            
        }

        public Task Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ConceptoTarifariaDTO> Get(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PaginadoResponse<ConceptoTarifariaDTO>> GetAllPaginado(string? filtro = null, int pagina = 1, int porPagina = 10)
        {
            throw new NotImplementedException();
        }

        public Task Update(ConceptoTarifariaDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
