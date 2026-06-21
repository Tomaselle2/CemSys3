using CemSys3.DTOs.Estadistica;
using CemSys3.Enumerables;
using CemSys3.Interfaces.Estadistica;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.Estadistica
{
    public class EstadisticaService : IEstadistica
    {

        private readonly AppDbContext _context; 

        public EstadisticaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EstadisticasDTO> GetEstadisticasGenerales()
        {
            var dto = new EstadisticasDTO();

            // ==========================================
            // 1. Difuntos actuales en el cementerio
            // Se toma el último registro de cada difunto en ParcelaDifuntos
            // (el de mayor Id, que refleja el último movimiento registrado)
            // y se considera "actual" si ese registro no tiene FechaRetiro.
            // ==========================================
            var ultimosMovimientos = _context.ParcelaDifuntos
                .Where(pd => !_context.ParcelaDifuntos.Any(pd2 =>
                    pd2.DifuntoId == pd.DifuntoId && pd2.Id > pd.Id));

            dto.DifuntosActuales = await ultimosMovimientos.CountAsync(pd => pd.FechaRetiro == null);

            // ==========================================
            // 2. Concesiones por estado
            // Los Id de estado son únicos en todo el sistema (EstadosTramiteEnum)
            // ==========================================
            var concesiones = _context.Concesiones.AsQueryable();

            dto.ConcesionesVigentes = await concesiones
                .CountAsync(c => c.Tramite.EstadoActualId == (int)EstadosTramiteEnum.Vigente);

            dto.ConcesionesVencidas = await concesiones
                .CountAsync(c => c.Tramite.EstadoActualId == (int)EstadosTramiteEnum.Vencido);

            dto.ConcesionesCaducadas = await concesiones
                .CountAsync(c => c.Tramite.EstadoActualId == (int)EstadosTramiteEnum.Caducado);

            dto.ConcesionesSinContrato = await concesiones
                .CountAsync(c => c.Tramite.EstadoActualId == (int)EstadosTramiteEnum.SinContrato);

            // ==========================================
            // 3. Ocupación de parcelas (Nichos / Fosas / Panteones)
            // Ocupado = CantidadDifuntos > 0 según lo confirmado
            // ==========================================
            var parcelas = _context.Parcelas.Where(p => p.Visibilidad);

            dto.NichosOcupados = await parcelas.CountAsync(p =>
                p.TipoParcelaId == (int)TipoParcelaEnum.Nicho && p.CantidadDifuntos > 0);

            dto.NichosDesocupados = await parcelas.CountAsync(p =>
                p.TipoParcelaId == (int)TipoParcelaEnum.Nicho && p.CantidadDifuntos == 0);

            dto.FosasOcupadas = await parcelas.CountAsync(p =>
                p.TipoParcelaId == (int)TipoParcelaEnum.Fosa && p.CantidadDifuntos > 0);

            dto.PanteonesOcupados = await parcelas.CountAsync(p =>
                p.TipoParcelaId == (int)TipoParcelaEnum.Panteon && p.CantidadDifuntos > 0);

            return dto;
        }
    }
}
