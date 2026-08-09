using CemSys3.DTOs.Tarifaria;
using CemSys3.Enumerables;
using CemSys3.Interfaces.Tarifaria;
using CemSys3.Models;
using CemSys3.Helpers.Enumerable;
using Microsoft.EntityFrameworkCore;

namespace CemSys3.Business.Tarifaria
{
    public class PrecioIngresoService : IPrecioIngresoService
    {
        private readonly AppDbContext _context;

        public PrecioIngresoService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ConceptoIngresoDTO>> GetPreciosAperturas(int tipoParcelaId)
        {
            var query = _context.PreciosTarifarias.AsQueryable();

            // Si es fosa, solo traer apertura de fosa
            if (tipoParcelaId == (int)TipoParcelaEnum.Fosa)
            {
                query = query.Where(p => p.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.AperturaFosa ||
                p.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.PorcentajeFondoAyudaCentroSalud);
            }
            else // Si no es fosa, traer apertura de nicho (con o sin placa)
            {
                query = query.Where(p =>
                    p.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.AperturaNichoConPlaca ||
                    p.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.AperturaNichoSinPlaca ||
                    p.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.PorcentajeFondoAyudaCentroSalud
                );
            }

            return await query.Select(s => new ConceptoIngresoDTO
            {
                ConceptoId = s.ConceptoTarifariaId,
                Nombre = s.ConceptoTarifaria.Nombre,
                PrecioBase = s.Precio
            }).ToListAsync();
        }
        public async Task<IEnumerable<PrecioIngresoDTO>> ObtenerTodasLasReglasAsync()
        {
            var reglas = await _context.ReglasIngresos
            .Where(r => r.Visibilidad)
            .Include(r => r.TipoParcela)
            .ToListAsync();

            var preciosLookup = (await _context.PreciosTarifarias
                .Where(p => p.Visibilidad == true)
                .ToListAsync())
                .ToLookup(p => p.ConceptoTarifariaId);

            var conceptos = await _context.ConceptosTarifaria
                .Include(c => c.Tema)
                .ToListAsync();

            var conceptosDict = conceptos.ToDictionary(c => c.Id);

            var resultado = new List<PrecioIngresoDTO>();

            foreach (var regla in reglas)
            {
                var dto = new PrecioIngresoDTO
                {
                    NombreRegla = regla.NombreRegla,
                    TipoParcelaId = regla.TipoParcelaId
                };

                CalcularInhumacion(dto, regla, preciosLookup, conceptosDict);
                CalcularRegistroCivil(dto, regla, preciosLookup, conceptosDict);
                CalcularDerechoOficina(dto, regla, preciosLookup, conceptosDict);
                AplicarFondoPorTema(dto, regla, preciosLookup, conceptosDict);

                resultado.Add(dto);
            }

            return resultado;
        }

        public async Task<IEnumerable<PrecioIngresoDTO>> GetPreciosIngresoBy(int tipoParcelaId, int estadoDifuntoId)
        {
            var reglas = await _context.ReglasIngresos
            .Where(r => r.Visibilidad &&
            r.TipoParcelaId == tipoParcelaId &&
            r.EstadoDifuntoId == estadoDifuntoId)
            .Include(r => r.TipoParcela)
            .ToListAsync();

            var preciosLookup = (await _context.PreciosTarifarias
                .Where(p => p.Visibilidad == true)
                .ToListAsync())
                .ToLookup(p => p.ConceptoTarifariaId);

            var conceptos = await _context.ConceptosTarifaria
                .Include(c => c.Tema)
                .ToListAsync();

            var conceptosDict = conceptos.ToDictionary(c => c.Id);

            var resultado = new List<PrecioIngresoDTO>();

            foreach (var regla in reglas)
            {
                var dto = new PrecioIngresoDTO
                {
                    NombreRegla = regla.NombreRegla,
                    TipoParcelaId = regla.TipoParcelaId
                };

                CalcularInhumacion(dto, regla, preciosLookup, conceptosDict);
                CalcularRegistroCivil(dto, regla, preciosLookup, conceptosDict);
                CalcularDerechoOficina(dto, regla, preciosLookup, conceptosDict);
                AplicarFondoPorTema(dto, regla, preciosLookup, conceptosDict);

                resultado.Add(dto);
            }

            return resultado;
        }

        private void CalcularInhumacion(
            PrecioIngresoDTO dto,
            ReglasIngreso regla,
            ILookup<int, PreciosTarifaria> precios,
            Dictionary<int, ConceptosTarifarium> conceptos)
        {
            var tema = new TemaIngresoDTO
            {
                Tema = EnumHelper.GetDisplayNameByValue<TemaTarifariaEnum>((int)TemaTarifariaEnum.Inhumacion)
            };

            // 🔹 Inhumación (con reglas)
            var conceptoInhumacionId = regla.ConceptoInhumacionId;
            decimal precioBase = ObtenerPrecio(precios, conceptoInhumacionId);

            decimal precioFinal = precioBase;

            if (regla.EstadoDifuntoId == (int)EstadoDifuntoEnum.Cremado)
                precioFinal *= 0.25m;

            if (regla.PorcentajeAumentoOtraLocalidadId.HasValue)
            {
                decimal porcentaje = ObtenerPrecio(precios, regla.PorcentajeAumentoOtraLocalidadId.Value);
                precioFinal += precioFinal * porcentaje;
            }

            tema.Conceptos.Add(new ConceptoIngresoDTO
            {
                ConceptoId = conceptoInhumacionId,
                Nombre = conceptos[conceptoInhumacionId].Nombre,
                PrecioBase = Math.Round(precioFinal, 2)
            });

            // 🔹 CIERRE (precio base, sin aumentos)
            int? cierreId = regla.TipoParcelaId switch
            {
                (int)TipoParcelaEnum.Nicho => regla.CierreNicho,
                (int)TipoParcelaEnum.Fosa => regla.CierreFosa,
                (int)TipoParcelaEnum.Panteon => regla.CierreNicho,
                _ => null
            };

            if (cierreId.HasValue)
            {
                tema.Conceptos.Add(new ConceptoIngresoDTO
                {
                    ConceptoId = cierreId.Value,
                    Nombre = conceptos[cierreId.Value].Nombre,
                    PrecioBase = ObtenerPrecio(precios, cierreId.Value)
                });
            }

            dto.Temas.Add(tema);
        }

        private void CalcularRegistroCivil(
            PrecioIngresoDTO dto,
            ReglasIngreso regla,
            ILookup<int, PreciosTarifaria> precios,
            Dictionary<int, ConceptosTarifarium> conceptos)
        {
            var tema = new TemaIngresoDTO
            {
                Tema = EnumHelper.GetDisplayNameByValue<TemaTarifariaEnum>((int)TemaTarifariaEnum.RegistroCivil)
            };

            // 🔹 Defunción (siempre)
            tema.Conceptos.Add(new ConceptoIngresoDTO
            {
                ConceptoId = regla.ConceptoDefuncionId,
                Nombre = conceptos[regla.ConceptoDefuncionId].Nombre,
                PrecioBase = ObtenerPrecio(precios, regla.ConceptoDefuncionId)
            });

            // 🔹 Transcripción (solo si existe)
            if (regla.ConceptoTranscripcionId.HasValue)
            {
                tema.Conceptos.Add(new ConceptoIngresoDTO
                {
                    ConceptoId = regla.ConceptoTranscripcionId.Value,
                    Nombre = conceptos[regla.ConceptoTranscripcionId.Value].Nombre,
                    PrecioBase = ObtenerPrecio(precios, regla.ConceptoTranscripcionId.Value)
                });
            }

            dto.Temas.Add(tema);
        }



        private void CalcularDerechoOficina(
            PrecioIngresoDTO dto,
            ReglasIngreso regla,
            ILookup<int, PreciosTarifaria> precios,
            Dictionary<int, ConceptosTarifarium> conceptos)
        {
            int conceptoIntroduccionId = regla.ConceptoIntroduccionId;

            decimal precioBaseIntroduccion = ObtenerPrecio(precios, conceptoIntroduccionId);
            decimal precioFinal;

            // 🔹 Base según estado del cuerpo
            if (regla.EstadoDifuntoId == (int)EstadoDifuntoEnum.Cremado)
            {
                // 25%
                precioFinal = precioBaseIntroduccion * 0.25m;
            }
            else
            {
                // Cuerpo completo → 100%
                precioFinal = precioBaseIntroduccion;
            }

            // 🔹 Aumento por otra localidad (22)
            if (regla.PorcentajeAumentoDerechoOficinaId.HasValue)
            {
                decimal porcentaje = ObtenerPrecio(precios, regla.PorcentajeAumentoDerechoOficinaId.Value);
                precioFinal += precioFinal * porcentaje;
            }

            AgregarConcepto(
                dto,
                TemaTarifariaEnum.DerechoDeOficina,
                conceptoIntroduccionId,
                conceptos,
                Math.Round(precioFinal, 2)
            );
        }



        private decimal ObtenerPrecio(
            ILookup<int, PreciosTarifaria> precios,
            int conceptoId)
        {
            return precios[conceptoId].FirstOrDefault()?.Precio ?? 0m;
        }

        private void AgregarConcepto(
            PrecioIngresoDTO dto,
            TemaTarifariaEnum temaEnum,
            int conceptoId,
            Dictionary<int, ConceptosTarifarium> conceptos,
            decimal precioFinal)
        {
            var temaNombre = EnumHelper.GetDisplayNameByValue<TemaTarifariaEnum>((int)temaEnum);

            var tema = dto.Temas.FirstOrDefault(t => t.Tema == temaNombre);
            if (tema == null)
            {
                tema = new TemaIngresoDTO { Tema = temaNombre };
                dto.Temas.Add(tema);
            }

            tema.Conceptos.Add(new ConceptoIngresoDTO
            {
                ConceptoId = conceptoId,
                Nombre = conceptos[conceptoId].Nombre,
                PrecioBase = Math.Round(precioFinal, 2)
            });
        }

        private void AplicarFondoPorTema(
            PrecioIngresoDTO dto,
            ReglasIngreso regla,
            ILookup<int, PreciosTarifaria> precios,
            Dictionary<int, ConceptosTarifarium> conceptos)
        {
            if (regla.PorcentajeFondoSaludId == 0 || !regla.MontoMinimoFondoId.HasValue)
                return;

            decimal porcentajeFondo = ObtenerPrecio(precios, regla.PorcentajeFondoSaludId);
            decimal montoMinimo = ObtenerPrecio(precios, regla.MontoMinimoFondoId.Value);

            foreach (var tema in dto.Temas)
            {
                // 1️⃣ Subtotal del tema
                decimal subtotal = tema.Conceptos.Sum(c => c.PrecioBase);

                if (subtotal <= 0)
                    continue;

                // 2️⃣ Fondo porcentual
                decimal fondoCalculado = subtotal * porcentajeFondo;

                // 3️⃣ Aplicar mínimo si corresponde
                decimal fondoFinal = fondoCalculado < montoMinimo
                    ? montoMinimo
                    : fondoCalculado;

                // 4️⃣ Agregar concepto Fondo
                tema.Conceptos.Add(new ConceptoIngresoDTO
                {
                    ConceptoId = regla.PorcentajeFondoSaludId,
                    Nombre = conceptos[regla.PorcentajeFondoSaludId].Nombre,
                    PrecioBase = Math.Round(fondoFinal, 2)
                });
            }
        }



        public async Task<List<CategoriaResumenDTO>> ObtenerResumenGeneralAsync()
        {
            var idsGeneral = new[]
            {
        (int)ConceptosTarifariaEnum.AperturaFosa,
        (int)ConceptosTarifariaEnum.AperturaNichoConPlaca,
        (int)ConceptosTarifariaEnum.AperturaNichoSinPlaca,
        (int)ConceptosTarifariaEnum.Cremacion,
        (int)ConceptosTarifariaEnum.PermisoRefaccion,
        (int)ConceptosTarifariaEnum.PermisoParaColocarPlaca,
        (int)ConceptosTarifariaEnum.Reduccion,
    };

            var idsInhumacion = new[]
            {
        (int)ConceptosTarifariaEnum.CierreFosa,
        (int)ConceptosTarifariaEnum.CierreNicho,
    };

            var conceptos = await _context.ConceptosTarifaria
                .Where(c => c.Visibilidad)
                .ToDictionaryAsync(c => c.Id);

            var precios = (await _context.PreciosTarifarias
                .Where(p => p.Visibilidad == true)
                .ToListAsync())
                .ToLookup(p => p.ConceptoTarifariaId);

            // Valores globales de fondo (ajustar si varían por sección)
            decimal porcentajeFondo = precios[(int)ConceptosTarifariaEnum.PorcentajeFondoAyudaCentroSalud]
                .FirstOrDefault(p => p.SeccionId == null)?.Precio ?? 0m;

            decimal montoMinimo = precios[(int)ConceptosTarifariaEnum.MontoMinimoFondo]
                .FirstOrDefault(p => p.SeccionId == null)?.Precio ?? 0m;

            var resumen = new List<CategoriaResumenDTO>
    {
        ArmarCategoriaResumen("General", idsGeneral, precios, conceptos, porcentajeFondo, montoMinimo),
        ArmarCategoriaResumen("Inhumación", idsInhumacion, precios, conceptos, porcentajeFondo, montoMinimo)
    };

            return resumen;
        }

        private CategoriaResumenDTO ArmarCategoriaResumen(
            string nombreCategoria,
            int[] ids,
            ILookup<int, PreciosTarifaria> precios,
            Dictionary<int, ConceptosTarifarium> conceptos,
            decimal porcentajeFondo,
            decimal montoMinimo)
        {
            var categoria = new CategoriaResumenDTO { Categoria = nombreCategoria };

            foreach (var id in ids)
            {
                if (!conceptos.ContainsKey(id)) continue;

                decimal precioBase = ObtenerPrecio(precios, id);
                decimal fondoCalculado = precioBase * porcentajeFondo;
                decimal fondoFinal = fondoCalculado < montoMinimo ? montoMinimo : fondoCalculado;

                categoria.Conceptos.Add(new ConceptoResumenDTO
                {
                    Nombre = conceptos[id].Nombre,
                    PrecioSinFondo = Math.Round(precioBase, 2),
                    PrecioConFondo = Math.Round(precioBase + fondoFinal, 2)
                });
            }

            return categoria;
        }

    }
}
