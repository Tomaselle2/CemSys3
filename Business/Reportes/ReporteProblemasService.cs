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
    }
}
