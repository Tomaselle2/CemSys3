using CemSys3.Business.HistorialEstadoService;
using CemSys3.DTOs.HistorialEstado;
using CemSys3.Enumerables;
using CemSys3.Interfaces.HistorialEstados;
using CemSys3.Interfaces.Tramite;
using CemSys3.Interfaces.TramitesConcesion;
using CemSys3.Models;
using CemSys3.ViewModels.TramiteConcesion;
using iText.Kernel.Pdf.Canvas.Wmf;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.TramiteConcesion
{
    public class CancelarService : ICancelarTramite
    {
        private readonly IHistorialEstados _historialEstadosService;
        private readonly AppDbContext _context;

        public CancelarService(IHistorialEstados historialEstadosService, AppDbContext context)
        {
            _historialEstadosService = historialEstadosService;
            _context = context;
        }

        public async Task CancelarTramite(int tramiteId)
        {
            Models.Tramite tramite = await _context.Tramites.FindAsync(tramiteId) ?? throw new Exception("Trámite no encontrado.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                //switch de tipo tramite
                switch (tramite.TipoTramiteId)
                {
                    case (int)TipoTramiteEnum.CambioTitular:
                        await ActualizarHistorial(tramite, (int)EstadosCambioTitularEnum.Cancelado);
                        break;
                    default:
                        throw new Exception("Tipo de trámite no soportado para cancelación.");
                }

                await transaction.CommitAsync();
            }
            catch (Exception) {
                await transaction.RollbackAsync();
                throw;
            }
            
        }

        private async Task ActualizarHistorial(Models.Tramite tramite, int estado)
        {
            HistorialEstadosDTO historial = new HistorialEstadosDTO
            {
                Fecha = DateTime.Now,
                TramiteId = tramite.Id,
                EstadoTramiteId = estado
            };

            await _historialEstadosService.Add(historial);

            tramite.EstadoActualId = estado;
            await _context.SaveChangesAsync();
        }
    }
}
