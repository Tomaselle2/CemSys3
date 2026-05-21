using CemSys3.DTOs.Persona;
using CemSys3.DTOs.Tramite;
using CemSys3.Enumerables;
using CemSys3.Helpers;
using CemSys3.Interfaces.PlantillaTramite;
using CemSys3.Interfaces.TramitesConcesion;
using CemSys3.Models;
using CemSys3.ViewModels.TramiteConcesion;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
namespace CemSys3.Business.TramiteConcesion
{
    public class RequisitosService : IRequisitos
    {
        private readonly AppDbContext _context;
        private readonly ITemplateProcessor templateProcessor;

        public RequisitosService(AppDbContext context, ITemplateProcessor template)
        {
            _context = context;
            templateProcessor = template;
        }
        public async Task<IEnumerable<RequisitosTramiteDTO>> GetAll(int concesionId)
        {
            Models.Concesione concesion = await _context.Concesiones.AsNoTracking()
                   .Include(c => c.Tramite)
                   .Include(c => c.Parcela)
                       .ThenInclude(p => p.Seccion)
                   .FirstOrDefaultAsync(c => c.TramiteId == concesionId) ?? throw new Exception("Concesion no encontrada");

            IEnumerable<TitularesContratoDTO> titulares = await _context.HistorialTitularesConcesiones
                    .Where(h => h.ConcesionId == concesionId && h.FechaFin == null)
                    .Select(h => new TitularesContratoDTO
                    {
                        Id = h.Persona.Id,
                        Dni = h.Persona.Dni,
                        Nombre = h.Persona.Nombre,
                        Apellido = h.Persona.Apellido,
                        Sexo = h.Persona.Sexo,
                        Celular = h.Persona.Celular,
                        CorreoElectronico = h.Persona.Correo,
                        Domicilio = h.Persona.Domicilio
                    }).ToListAsync();

            //consultar el difuntos relacionados a la parcela para el tramite
           IEnumerable<DifuntoContratoDTO> difuntos = await _context.ParcelaDifuntos
                .Where(p => p.ParcelaId == concesion.ParcelaId && p.FechaRetiro == null)
                .Select(p => new DifuntoContratoDTO
                {
                    Id = p.Difunto.Id,
                    DNI = p.Difunto.Dni,
                    Nombre = p.Difunto.Nombre,
                    Apellido = p.Difunto.Apellido,
                    FechaIngreso = p.FechaIngreso,
                    EstadoDifuntoId = p.Difunto.EstadoDifuntoId
                }).ToListAsync();

            Models.PreciosTarifaria porcentajeFondo = await _context.PreciosTarifarias.Where(p => p.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.PorcentajeFondoAyudaCentroSalud).FirstOrDefaultAsync() ?? throw new Exception("No se encontro el % fondo ayuda");
            Models.PreciosTarifaria precioApertura = await _context.PreciosTarifarias.Where(p => p.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.AperturaNichoConPlaca).FirstOrDefaultAsync() ?? throw new Exception("No se encontro precio de apertura");
            Models.PreciosTarifaria precioCremacion = await _context.PreciosTarifarias.Where(p => p.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.Cremacion).FirstOrDefaultAsync() ?? throw new Exception("No se encontro precio de cremacion");
            Models.PreciosTarifaria precioCierreNicho = await _context.PreciosTarifarias.Where(p => p.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.CierreNicho).FirstOrDefaultAsync() ?? throw new Exception("No se encontro precio de cierre de nicho");
            Models.PreciosTarifaria precioCierreFosa = await _context.PreciosTarifarias.Where(p => p.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.CierreFosa).FirstOrDefaultAsync() ?? throw new Exception("No se encontro precio de cierre de fosa");


            List<Models.RequisitosTramite> requisitos = await _context.RequisitosTramites
                .Where(rt => rt.Activo == true).ToListAsync();

            string difuntosFormateados = DifuntoFormatter.FormatearDifuntos(difuntos);

            foreach (var requisito in requisitos)
            {
                var variables = new Dictionary<string, string>
                    {
                        { "TitularesActuales", string.Join(", ", titulares.Select(t => t.Apellido.ToUpper() + " " + t.Nombre.ToUpper())) },
                        {
                            "precioApertura",
                            Math.Round(
                                precioApertura.Precio * (1 + porcentajeFondo.Precio),
                                2
                            ).ToString("0.00", CultureInfo.InvariantCulture)
                        },

                        {
                            "precioCremacion",
                            Math.Round(
                                precioCremacion.Precio * (1 + porcentajeFondo.Precio),
                                2
                            ).ToString("0.00", CultureInfo.InvariantCulture)
                        },
                        { "Difuntos", difuntosFormateados },
                        {
                            "precioCierreNicho",
                            Math.Round(
                                precioCierreNicho.Precio * (1 + porcentajeFondo.Precio),
                                2
                            ).ToString("0.00", CultureInfo.InvariantCulture)
                        },
                         {
                            "precioCierreFosa",
                            Math.Round(
                                precioCierreFosa.Precio * (1 + porcentajeFondo.Precio),
                                2
                            ).ToString("0.00", CultureInfo.InvariantCulture)
                        }
                    };

                requisito.Descripcion = templateProcessor.Procesar(requisito.Descripcion ?? "", variables);
            }

                return requisitos.Select(rt => new RequisitosTramiteDTO
                {
                    Id = rt.Id,
                    TipoTramiteId = rt.TipoTramiteId,
                    Descripcion = rt.Descripcion ?? ""
                }).ToList();
        }

        public async Task<RequisitosTramiteDTO> GetByTipoTramiteId(int tipoTramiteId)
        {
            return await _context.RequisitosTramites
               .Where(rt => rt.Activo == true && rt.TipoTramiteId == tipoTramiteId)
               .Select(rt => new RequisitosTramiteDTO
               {
                   Id = rt.Id,
                   TipoTramiteId = rt.TipoTramiteId,
                   Descripcion = rt.Descripcion ?? ""
               })
               .FirstOrDefaultAsync();
        }

        public async Task Update(int tipoTramiteId, string descripcion)
        {
            Models.RequisitosTramite requisito = await _context.RequisitosTramites.Where(t => t.TipoTramiteId == tipoTramiteId).FirstOrDefaultAsync() ?? throw new Exception("No se encontro el requisito");

            if (!string.IsNullOrEmpty(descripcion))
            {
                requisito.Descripcion = descripcion;
            }

            await _context.SaveChangesAsync();
        }
    }
}
