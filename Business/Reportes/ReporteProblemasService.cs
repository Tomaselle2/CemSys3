using CemSys3.DTOs.Reportes;
using CemSys3.Interfaces.Reportes;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.Reportes
{
    public class ReporteProblemasService : IReporteProblemasService
    {
        private readonly AppDbContext _context; // ajustá al nombre real de tu DbContext

        public ReporteProblemasService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ReporteConcesionSinDifuntoDTO>> GetConcesionesSinDifuntos()
        {
            return await _context.Concesiones
                .AsNoTracking()
                .Where(c => c.FechaFin == null // No está caducada
                    && !c.Parcela.ParcelaDifuntos.Any(pd => pd.FechaRetiro == null))
                .OrderBy(c => c.Concesion)
                .Select(c => new ReporteConcesionSinDifuntoDTO
                {
                    Concesion = c.Concesion,
                    Seccion = c.Parcela.Seccion.Nombre,
                    NroFila = c.Parcela.NroFila,
                    NroParcela = c.Parcela.NroParcela,
                    Vencimiento = c.Vencimiento
                })
                .ToListAsync();
        }

        public async Task<List<ReporteTitularEsDifuntoDTO>> GetConcesionesConTitularFallecido()
        {
            var query =
                from c in _context.Concesiones.AsNoTracking()
                where c.FechaFin == null
                from h in c.HistorialTitularesConcesiones
                where h.FechaFin == null
                from pd in c.Parcela.ParcelaDifuntos
                where pd.FechaRetiro == null && pd.DifuntoId == h.PersonaId
                select new ReporteTitularEsDifuntoDTO
                {
                    Concesion = c.Concesion,
                    Seccion = c.Parcela.Seccion.Nombre,
                    NroFila = c.Parcela.NroFila,
                    NroParcela = c.Parcela.NroParcela,
                    PersonaId = h.PersonaId!.Value,
                    Nombre = h.Persona.Nombre ?? "",
                    Apellido = h.Persona.Apellido ?? "",
                    Vencimiento = c.Vencimiento
                };

            return await query
                .OrderBy(x => x.Concesion)
                .ToListAsync();
        }

        public async Task<List<ReporteParcelaConMultiplesConcesionesDTO>> GetParcelasConMultiplesConcesiones()
        {
            var parcelasConDuplicados = _context.Concesiones
                .Where(c => c.Visibilidad == true && c.FechaFin == null)
                .GroupBy(c => c.ParcelaId)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key);

            return await _context.Concesiones
                .AsNoTracking()
                .Where(c => c.Visibilidad == true
                    && c.FechaFin == null
                    && parcelasConDuplicados.Contains(c.ParcelaId))
                .OrderBy(c => c.ParcelaId)
                .ThenBy(c => c.TramiteId)
                .Select(c => new ReporteParcelaConMultiplesConcesionesDTO
                {
                    Concesion = c.Concesion,
                    TipoParcelaId = c.Parcela.TipoParcelaId,
                    Seccion = c.Parcela.Seccion.Nombre,
                    NroFila = c.Parcela.NroFila,
                    NroParcela = c.Parcela.NroParcela,
                    Vencimiento = c.Vencimiento
                })
                .ToListAsync();
        }
    }
}
