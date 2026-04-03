using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.Persona;
using CemSys3.DTOs.Tarifaria;
using CemSys3.Enumerables;
using CemSys3.Helpers.Enumerable;
using CemSys3.Interfaces.Concesion;
using CemSys3.Interfaces.Parcela;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.Concesion
{
    public class DeudaConcesionService : IDeudaConcesion
    {
        private readonly AppDbContext _context;

        public DeudaConcesionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> CalculoDeudaConcesion(int tramiteId)
        {
            Models.Concesione concesion = await _context.Concesiones.AsNoTracking()
               .Include(c => c.Tramite)
               .Include(c => c.Parcela)
                   .ThenInclude(p => p.Seccion)
               .FirstOrDefaultAsync(c => c.TramiteId == tramiteId) ?? throw new Exception("Concesión no encontrada.");

            IEnumerable<TitularesContratoDTO> Titulares = await _context.HistorialTitularesConcesiones.Where(h => h.ConcesionId == tramiteId && h.FechaFin == null)
                .Select(h => new TitularesContratoDTO
                {
                    Nombre = h.Persona.Nombre,
                    Apellido = h.Persona.Apellido,
                }).ToListAsync();

            Models.PreciosTarifaria porcentajeDescuentoRenovacion = await _context.PreciosTarifarias.Where(p => p.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.PorcentajeDescuentoRenovacionConcesionAlDia).FirstOrDefaultAsync() ?? throw new Exception("No se encontro el % fondo ayuda");
            Models.PreciosTarifaria porcentajeFondo = await _context.PreciosTarifarias.Where(p => p.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.PorcentajeFondoAyudaCentroSalud).FirstOrDefaultAsync() ?? throw new Exception("No se encontro el % fondo ayuda");
            
            IEnumerable<PrecioTarifariaDTO> precios = new List<PrecioTarifariaDTO>();
            //consultar los precios relacionados a la parcela dependiendo del tipo de parcela
            if (concesion.TipoParcela == "Nicho")
            {
                precios = await _context.PreciosTarifarias
                    .Where(t => t.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.ConcesionNicho && t.SeccionId == concesion.Parcela.SeccionId && t.NroFila == concesion.Parcela.NroFila)
                    .Select(t => new PrecioTarifariaDTO
                    {
                        Id = t.Id,
                        Precio = t.Precio,
                        NroFila = t.NroFila,
                        ConceptoTarifariaId = t.ConceptoTarifariaId,
                        AniosConcesionId = t.AniosConcesionId,
                        SeccionId = t.SeccionId,
                        Visibilidad = t.Visibilidad,
                    }).ToListAsync();
            }
            //consultar los precios relacionados a la parcela dependiendo del tipo de parcela
            if (concesion.TipoParcela == "Fosa")
            {
                precios = await _context.PreciosTarifarias
                    .Where(t => t.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.ConcesionFosa)
                    .Select(t => new PrecioTarifariaDTO
                    {
                        Id = t.Id,
                        Precio = t.Precio,
                        NroFila = t.NroFila,
                        ConceptoTarifariaId = t.ConceptoTarifariaId,
                        AniosConcesionId = t.AniosConcesionId,
                        SeccionId = t.SeccionId,
                        Visibilidad = t.Visibilidad,
                    }).ToListAsync();
            }

            string parcela = string.Empty;

            if (concesion.Parcela.TipoParcelaId == (int)TipoParcelaEnum.Nicho) //nicho
            {
                parcela = $"Nicho {concesion.Parcela.NroParcela.ToString()} Fila {concesion.Parcela.NroFila.ToString()}";
            }
            else if (concesion.Parcela.TipoParcelaId == (int)TipoParcelaEnum.Fosa)//fosa
            {
                parcela = $"Fosa {concesion.Parcela.NroParcela.ToString()}";
            }
            else if (concesion.Parcela.TipoParcelaId == (int)TipoParcelaEnum.Panteon) //panteon
            {
                parcela = $"Lote {concesion.Parcela.NroParcela.ToString()} (Panteón)";
            }
            


            throw new NotImplementedException();
        }


        

        private string EncabezadoMensaje(int nroConcesion, string parcela, DateTime vencimiento, IEnumerable<TitularesContratoDTO> Titulares)
        {
            string encabezado = string.Empty;

            encabezado += $"Concesión *{nroConcesion.ToString("D5") ?? "-----"}*. Vencimiento: {vencimiento.ToString("dd/MM/yyyy")}\n";

            encabezado += "Titular: ";
            foreach (var titular in Titulares)
            {
                encabezado += $"{titular.Apellido?.ToUpper()} {titular.Nombre?.ToUpper()}. ";
            }

            encabezado += "\n";
            return encabezado;
        }

        private string MensajeGenericoImpuestos()
        {
            return $"\nAdeuda impuestos del _____ al {DateTime.Now.Year.ToString()}. El monto total es de $______";
        }

        private string PreciosRenovacion(List<PrecioTarifariaDTO> precios, decimal fondo)
        {
            string mensaje = "Puede renovar la concesión por:\n";

            for(int i=0; i < precios.Count; i++)
            { 
                //remuevo todo lo que el precio sea $0.
                if (precios[i].Precio <= 0)
                {
                    precios.Remove(precios[i]);
                }

                //aumentar el porcentaje de fondo.
                precios[i].Precio += precios[i].Precio * fondo;
            }

            //muestro todos los precios
            foreach(var precio in precios)
            {
                mensaje += $"{EnumHelper.GetDisplayNameByValue<AniosConcesionEnum>(precio.AniosConcesionId.Value)} - ${precio.Precio.ToString("N2")}\n";
            }


            return mensaje;
        }
    }
}
