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



            //consultar los difuntos relacionados a la parcela
            IEnumerable<DifuntoContratoDTO> Difuntos = await _context.ParcelaDifuntos
                .Where(p => p.ParcelaId == concesion.ParcelaId && p.FechaRetiro == null)
                .Select(p => new DifuntoContratoDTO
                {
                    Nombre = p.Difunto.Nombre,
                    Apellido = p.Difunto.Apellido
                }).ToListAsync();



            Models.PreciosTarifaria porcentajeDescuentoRenovacion = await _context.PreciosTarifarias.Where(p => p.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.PorcentajeDescuentoRenovacionConcesionAlDia).FirstOrDefaultAsync() ?? throw new Exception("No se encontro el % fondo ayuda");
            Models.PreciosTarifaria porcentajeFondo = await _context.PreciosTarifarias.Where(p => p.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.PorcentajeFondoAyudaCentroSalud).FirstOrDefaultAsync() ?? throw new Exception("No se encontro el % fondo ayuda");
            
            List<PrecioTarifariaDTO> precios = new List<PrecioTarifariaDTO>();
            //consultar los precios relacionados a la parcela dependiendo del tipo de parcela
            bool esNichoEspecial = concesion.Parcela.TipoNichoId == (int)TipoNichoEnum.Especial;
            bool esNichoUrnario = concesion.Parcela.TipoNichoId == (int)TipoNichoEnum.Urnario;

            if (concesion.TipoParcela == "Nicho")
            {
                var queryPrecios = _context.PreciosTarifarias
                    .Where(t => t.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.ConcesionNicho
                                && t.NroFila == concesion.Parcela.NroFila);

                // Los nichos especiales no pertenecen a ninguna sección real:
                // sus precios están cargados con SeccionId = NULL
                queryPrecios = esNichoEspecial
                    ? queryPrecios.Where(t => t.SeccionId == null)
                    : queryPrecios.Where(t => t.SeccionId == concesion.Parcela.SeccionId);

                precios = await queryPrecios
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

                // Si es urnario, se descuenta el % configurado sobre cada precio antes de seguir
                if (esNichoUrnario)
                {
                    Models.PreciosTarifaria porcentajeUrnario = await _context.PreciosTarifarias
                        .Where(p => p.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.PorcentajePreciosNichosUrnariosConcesionSecc16_18)
                        .FirstOrDefaultAsync() ?? throw new Exception("No se encontró el % de descuento para nichos urnarios.");

                    foreach (var precio in precios)
                    {
                        precio.Precio = Math.Round(precio.Precio * (1 - porcentajeUrnario.Precio), 2);
                    }
                }
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
                parcela = $"Nicho {concesion.Parcela.NroParcela.ToString()} Sección {concesion.Parcela.Seccion.Nombre.ToUpper()} Fila {concesion.Parcela.NroFila.ToString()}";
            }
            else if (concesion.Parcela.TipoParcelaId == (int)TipoParcelaEnum.Fosa)//fosa
            {
                parcela = $"Fosa {concesion.Parcela.NroParcela.ToString()} Sección {concesion.Parcela.Seccion.Nombre.ToUpper()}";
            }
            else if (concesion.Parcela.TipoParcelaId == (int)TipoParcelaEnum.Panteon) //panteon
            {
                parcela = $"Lote {concesion.Parcela.NroParcela.ToString()} Sección {concesion.Parcela.Seccion.Nombre.ToUpper()} (Panteón)";
            }

            string vencimiento = concesion.Vencimiento != null ? concesion.Vencimiento.Value.ToString("dd/MM/yyyy") : "";
            string encabezado = EncabezadoMensaje(concesion.Concesion ?? 0, parcela, vencimiento, Titulares, Difuntos);
            string mensajeImpuestos = MensajeGenericoImpuestos();
            string deudaConcesionVencida = string.Empty;

            if (concesion.Parcela.TipoParcelaId != (int)TipoParcelaEnum.Panteon)
            {
                deudaConcesionVencida = CalculoDeudaConcesionVencida(concesion.Vencimiento, concesion.Parcela.TipoParcelaId ?? 0, precios, porcentajeFondo.Precio, porcentajeDescuentoRenovacion.Precio);
            }

            return encabezado + mensajeImpuestos + deudaConcesionVencida;
        }


        

        private string EncabezadoMensaje(int nroConcesion, string parcela, string vencimiento, IEnumerable<TitularesContratoDTO> Titulares, IEnumerable<DifuntoContratoDTO> Difuntos)
        {
            string encabezado = string.Empty;
            encabezado += $"Buen día. Me comunico del área del cementerio para informarle el estado de su concesión.\n";
            encabezado += $"Concesión *{nroConcesion.ToString("D5") ?? "-----"}*. Vencimiento: {vencimiento}\n";
            encabezado += $"Ubicación: {parcela}\n";
            encabezado += "Difunto/s: ";
            foreach (var difunto in Difuntos)
            {
                encabezado += $"{difunto.Apellido?.ToUpper()} {difunto.Nombre?.ToUpper()}. ";

            }

            encabezado += "\n";

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
            return $"\nAdeuda impuestos del _____ al {DateTime.Now.Year.ToString()}. El monto total de los impuestos es de $______\n";
        }

        private string PreciosRenovacion(List<PrecioTarifariaDTO> precios, decimal fondo)
        {
            string mensaje = "\nPuede renovar la concesión por:\n";

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


        private string CalculoDeudaConcesionVencida(DateOnly? vencimiento, int tipoParcelaId, List<PrecioTarifariaDTO> precios, decimal porcentajeFondo, decimal descuento)
        {
            if (vencimiento == null)
                return string.Empty;

            DateOnly hoy = DateOnly.FromDateTime(DateTime.Now);
            string mensajeFinal = string.Empty;

            // 🟢 CASO 3: No está vencido y NO es del año actual
            if (vencimiento > hoy && vencimiento.Value.Year != hoy.Year)
            {
                return "No posee deuda de concesión.\n";
            }

            // 🟡 CASO 2: No está vencido pero vence este año
            if (vencimiento >= hoy && vencimiento.Value.Year == hoy.Year)
            {
                string preciosRenovacion = PreciosRenovacion(precios, porcentajeFondo);
                mensajeFinal = ObtenerMensajeFinal(vencimiento.Value, hoy, tipoParcelaId, descuento);

                return $"La concesión se vence el {vencimiento.Value:dd/MM/yyyy}.\n{preciosRenovacion}{mensajeFinal}";
            }

            // 🔴 CASO 1: Está vencido → calcular deuda
            decimal precioBase = 0;

            foreach (var precio in precios)
            {
                if (tipoParcelaId == (int)TipoParcelaEnum.Nicho &&
                    precio.AniosConcesionId == (int)AniosConcesionEnum.anio5)
                {
                    precioBase = precio.Precio;
                }

                if (tipoParcelaId == (int)TipoParcelaEnum.Fosa &&
                    precio.AniosConcesionId == (int)AniosConcesionEnum.anio15)
                {
                    precioBase = precio.Precio;
                }
            }

            if (precioBase == 0)
                return "No se encontró precio base.";

            decimal precioAnual = tipoParcelaId == (int)TipoParcelaEnum.Nicho
                ? precioBase / 5
                : precioBase / 15;

            decimal precioMensual = precioAnual / 12;
            decimal precioDiario = precioMensual / 30;

            DateOnly fechaAux = vencimiento.Value;
            int meses = 0;

            while (fechaAux.AddMonths(1) <= hoy)
            {
                fechaAux = fechaAux.AddMonths(1);
                meses++;
            }

            int dias = hoy.DayNumber - fechaAux.DayNumber;

            decimal deudaTotal = (meses * precioMensual) + (dias * precioDiario);
            deudaTotal += deudaTotal * porcentajeFondo;

            mensajeFinal = ObtenerMensajeFinal(vencimiento.Value, hoy, tipoParcelaId, descuento);

            string preciosRenovacionFinal = PreciosRenovacion(precios, porcentajeFondo);

            string mensajeDeuda = $"La deuda de concesión desde {vencimiento.Value:dd/MM/yyyy} hasta {hoy:dd/MM/yyyy} es de *${deudaTotal:N2}*\n";
            mensajeDeuda += preciosRenovacionFinal;
            mensajeDeuda += mensajeFinal;

            return mensajeDeuda;
        }

        private string ObtenerMensajeFinal(DateOnly vencimiento, DateOnly hoy, int tipoParcelaId, decimal descuento)
        {
            if ((vencimiento.Month == hoy.Month || vencimiento >= hoy) && vencimiento.Year == hoy.Year && tipoParcelaId == (int)TipoParcelaEnum.Nicho)
            {
                return $"\nLa renovación se puede abonar en hasta 6 cuotas sin interés.\n" +
                       $"Si renueva durante el mes del vencimiento, accede a un {Math.Round(descuento * 100)}% de descuento abonando en un pago sobre la cantidad de años que elija.\n" +
                       $"Debe llegarse a la municipalidad para realizar el contrato.\n" +
                       "Horario de atención: de 7:00hs a 12:30hs.\n";
            }
            else if (vencimiento.Year != hoy.Year && tipoParcelaId == (int)TipoParcelaEnum.Nicho)
            {
                return $"\nLa renovación se puede abonar en hasta 6 cuotas sin interés.\n" +
                       $"Debe llegarse a la municipalidad para realizar el contrato.\n" +
                       "Horario de atención: de 7:00hs a 12:30hs.\n";
            }
            else if (vencimiento.Year == hoy.Year && tipoParcelaId == (int)TipoParcelaEnum.Nicho)
            {
                return $"\nLa renovación se puede abonar en hasta 6 cuotas sin interés.\n" +
                       $"Debe llegarse a la municipalidad para realizar el contrato.\n" +
                       "Horario de atención: de 7:00hs a 12:30hs.\n";
            }
            else
            {
                return $"\nLa renovación se puede abonar en hasta 4 cuotas sin interés.\n" +
                       $"Debe llegarse a la municipalidad para realizar el contrato.\n" +
                       "Horario de atención: de 7:00hs a 12:30hs.\n";
            }
        }


    }
}
