using CemSys3.Business.HistorialEstadoService;
using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.HistorialEstado;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.Tramite;
using CemSys3.Enumerables;
using CemSys3.Helpers.Enumerable;
using CemSys3.Interfaces.CargaIncialCemSys;
using CemSys3.Interfaces.HistorialEstados;
using CemSys3.Interfaces.Parcela;
using CemSys3.Models;
using CemSys3.ViewModels.Tramite;
using Microsoft.EntityFrameworkCore;


namespace CemSys3.Business.CargaInicialCemSys
{
    public class CargaInicialService : ICargaInicial
    {
        private readonly AppDbContext _context;
        private readonly IHistorialEstados _historialEstados;

        public CargaInicialService(AppDbContext context, IHistorialEstados historialEstados)
        {
            _context = context;
            _historialEstados = historialEstados;
        }

        public Task CargaInicial(IFormFile excel)
        {
            throw new NotImplementedException();
        }

        private void CargarFallecido(PersonaDTO fallecido)
        {
            //hay que ver si el fallecido ya existe en la base de datos, si existe no se hace nada, si no existe se crea el fallecido y se guarda en la base de datos.
        }

        private void CargarTitular(PersonaDTO titular)
        {
            //hay que ver si el titular ya existe en la base de datos, si existe no se hace nada, si no existe se crea el titular y se guarda en la base de datos.
        }

        private async Task CrearConcesion(ConcesionDTO concesionDTO)
        {
            //hay que ver si existe una concesion para la parcela, si existe no se hace nada, si no existe se crea la concesion y se guarda en la base de datos.
            Models.Concesione concesion = new Models.Concesione();
            concesion.TramiteId = concesionDTO.TramiteId;
            concesion.Concesion = concesionDTO.Concesion;
            concesion.Precio = concesionDTO.Precio;
            concesion.Visibilidad = true;
            concesion.TipoParcela = concesionDTO.TipoParcela;
            concesion.Vencimiento = concesionDTO.Vencimiento;
            concesion.ParcelaId = concesionDTO.ParcelaId;
            concesion.CantidadAniosId = concesionDTO.CantidadAniosId;
            concesion.CuotaId = concesionDTO.CuotaId;
            concesion.UsuarioId = concesionDTO.UsuarioId;
            concesion.FechaInicio = concesionDTO.FechaInicio ?? DateTime.Now;
            await _context.Concesiones.AddAsync(concesion);
            

            await _context.SaveChangesAsync();
        }

        private void MensajesConcesion(string mensaje)
        {
            //en caso de que la fecha de fallecimiento no exista, debe mostrar un mensaje en info adicional que diga el nombre del fallecido y que la fecha de defuncion es incorrecta y hay que modificarla.
        }

        private void SumarUnDifuntoAParcela(int parcelaId)
        {
            //suma un difunto a la parcela, hay que ver si la parcela existe, si existe se suma un difunto, si no existe no se hace nada.
        }

        private async Task RelacionTramitePersona(int tramiteId, int personaId)
        {
            await _historialEstados.VincularTramiteAPersona(tramiteId, personaId);
        }

        private async Task RelacionTramiteAParcela(int tramiteId, int parcelaId)
        {
            await _historialEstados.VincularTramiteAParcela(tramiteId, parcelaId);
        }

        private async Task RelacionTitularAConcesion(int personaId, int tramiteId)
        {
            await _historialEstados.VincularTitularAConcesion(personaId, tramiteId);
        }

        private async Task HistorialParcelaConcesion(int tramiteId, int parcelaId)
        {
            Models.HistorialParcelasConcesion historialParcela = new Models.HistorialParcelasConcesion
            {
                ConcesionId = tramiteId,
                ParcelaId = parcelaId,
                FechaInicio = DateTime.Now,
                FechaFin = null,          
                TramiteOrigenId = tramiteId
            };
            await _context.HistorialParcelasConcesions.AddAsync(historialParcela);
        }

        private async Task<int> CrearTramite(TramiteDTO dto) //para crear la concesion.
        {

            Models.Tramite tramite = new Models.Tramite
            {
                Id = await ObtenerProximoIdTramite(),
                Visibilidad = true,
                FechaCreacion = dto.FechaCreacion,
                TipoTramiteId = dto.TipoTramiteId,
                UsuarioId = 1,
                EstadoActualId = dto.EstadoActualId
            };

            //se guarda el trámite

            await _context.Tramites.AddAsync(tramite);

            HistorialEstadosDTO historial = new HistorialEstadosDTO
            {
                Fecha = tramite.FechaCreacion,
                TramiteId = tramite.Id,
                EstadoTramiteId = tramite.EstadoActualId
            };
            await _historialEstados.Add(historial);

            return tramite.Id;
        }

        private async Task<int> ObtenerProximoIdTramite()
        {
            int? maxId = await _context.Tramites.MaxAsync(t => (int?)t.Id);
            return (maxId ?? 0) + 1;
        }
    }
}
