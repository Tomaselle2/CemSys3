using CemSys3.DTOs.Tarifaria;
using CemSys3.Interfaces.Tarifaria;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace CemSys3.Business.Tarifaria
{
    public class TarifariaService : ITarifaria
    {
        private readonly AppDbContext _context;

        public TarifariaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task ActualizarPreciosTarifaria(List<PrecioActualizarDTO> preciosActualizar)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Obtener los IDs de los precios a actualizar
                var idsPrecios = preciosActualizar.Select(p => p.Id).ToList();

                // Verificar que todos los precios existan
                var preciosExistentes = await _context.PreciosTarifarias
                    .Where(p => idsPrecios.Contains(p.Id))
                    .ToListAsync();

                if (preciosExistentes.Count != preciosActualizar.Count)
                {
                    var idsEncontrados = preciosExistentes.Select(p => p.Id).ToList();
                    var idsNoEncontrados = idsPrecios.Except(idsEncontrados).ToList();

                    throw new ArgumentException($"Los siguientes precios no existen: {string.Join(", ", idsNoEncontrados)}");
                }

                // Actualizar cada precio
                foreach (var precioDto in preciosActualizar)
                {
                    var precioExistente = preciosExistentes.First(p => p.Id == precioDto.Id);

                    // Verificar que el ConceptoTarifariaId coincida (seguridad adicional)
                    if (precioExistente.ConceptoTarifariaId != precioDto.ConceptoTarifariaId)
                    {
                        throw new ArgumentException($"El ConceptoTarifariaId no coincide para el precio {precioDto.Id}");
                    }

                    // Actualizar el precio
                    precioExistente.Precio = precioDto.Precio;

                }

                // Guardar todos los cambios
                var filasAfectadas = await _context.SaveChangesAsync();

                if (filasAfectadas == 0)
                {
                    throw new InvalidOperationException("No se pudieron guardar los cambios.");
                }

                await transaction.CommitAsync();

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al actualizar precios de tarifaria: {ex.Message}", ex);
            }
        }

        //trae los precios sin paginar de todo menos los nichos
        public async Task<IEnumerable<TarifariaRequestDTO>> GetPrecios()
        {
            return await _context.PreciosTarifarias.Where(p => p.Visibilidad == true).Select(pre => new TarifariaRequestDTO
            {
                Id = pre.Id,
                Precio = pre.Precio,
                NroFila = pre.NroFila,
                ConceptoTarifariaId = pre.ConceptoTarifariaId,
                AniosConcesionId = pre.AniosConcesionId,
                SeccionId = pre.SeccionId,
                Visibilidad = pre.Visibilidad,
                TemaId = pre.ConceptoTarifaria.TemaId,
                NombreConcepto = pre.ConceptoTarifaria.Nombre
            }).ToListAsync();
        }

    }
}
