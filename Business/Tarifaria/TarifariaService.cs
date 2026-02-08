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

        //trae los precios de los ingresos

        /*
        Regla: "Nicho - Fallecidos en Colonia Tirolesa"
         ├─ Tema: Inhumación
         │   ├─ Inhumación nicho → $xx
         │   ├─ Cierre de nicho → $xx
         ├─ Tema: Registro Civil
         │   ├─ Defunción → $xx
         ├─ Tema: Derecho de Oficina
         │   ├─ Introducción → $xx
         ├─ Tema: Fondo
         │   ├─ % fondo salud → 0.05
         │   ├─ Monto mínimo fondo → $xx
        */

        public async Task<IEnumerable<PrecioIngresoDTO>> GetPreciosIngresos()
        {
            // 1️ Traigo todas las reglas visibles con sus conceptos + temas
            var reglas = await _context.ReglasIngresos
                .Include(r => r.ConceptoInhumacion).ThenInclude(c => c.Tema)
                .Include(r => r.ConceptoDefuncion).ThenInclude(c => c.Tema)
                .Include(r => r.ConceptoTranscripcion).ThenInclude(c => c.Tema)
                .Include(r => r.ConceptoIntroduccion).ThenInclude(c => c.Tema)
                .Include(r => r.CierreNichoNavigation).ThenInclude(c => c.Tema)
                .Include(r => r.CierreFosaNavigation).ThenInclude(c => c.Tema)
                .Include(r => r.PorcentajeFondoSalud).ThenInclude(c => c.Tema)
                .Include(r => r.MontoMinimoFondo).ThenInclude(c => c.Tema)
                .Include(r => r.PorcentajeAumentoOtraLocalidad).ThenInclude(c => c.Tema)
                .Include(r => r.PorcentajeAumentoDerechoOficina).ThenInclude(c => c.Tema)
                .Include(r => r.PorcentajeIntroduccionUrnaDerechoOficnaNavigation).ThenInclude(c => c.Tema)
                .Where(r => r.Visibilidad)
                .ToListAsync();

            // 2️ Traigo TODOS los precios visibles agrupados por concepto
            var preciosLookup = (await _context.PreciosTarifarias
                .Where(p => p.Visibilidad.HasValue)
                .ToListAsync())
                .ToLookup(p => p.ConceptoTarifariaId);

            var resultado = new List<PrecioIngresoDTO>();

            // 3️ Armo el DTO final
            foreach (var regla in reglas)
            {
                var temas = new Dictionary<string, TemaIngresoDTO>();

                void addConcepto(ConceptosTarifarium? concepto)
                {
                    if (concepto == null)
                        return;

                    var preciosDelConcepto = preciosLookup[concepto.Id];

                    // Si no hay precios, igual mostramos el concepto con 0
                    if (!preciosDelConcepto.Any())
                    {
                        AgregarConcepto(temas, concepto, 0);
                        return;
                    }

                    foreach (var precio in preciosDelConcepto)
                    {
                        AgregarConcepto(
                            temas,
                            concepto,
                            precio.Precio
                        );
                    }
                }

                addConcepto(regla.ConceptoInhumacion);
                addConcepto(regla.ConceptoDefuncion);
                addConcepto(regla.ConceptoTranscripcion);
                addConcepto(regla.ConceptoIntroduccion);
                addConcepto(regla.CierreNichoNavigation);
                addConcepto(regla.CierreFosaNavigation);
                addConcepto(regla.PorcentajeFondoSalud);
                addConcepto(regla.MontoMinimoFondo);
                addConcepto(regla.PorcentajeAumentoOtraLocalidad);
                addConcepto(regla.PorcentajeAumentoDerechoOficina);
                addConcepto(regla.PorcentajeIntroduccionUrnaDerechoOficnaNavigation);

                resultado.Add(new PrecioIngresoDTO
                {
                    NombreRegla = regla.NombreRegla,
                    TipoParcelaId = regla.TipoParcelaId,
                    Temas = temas.Values.ToList()
                });
            }

            return resultado;
        }


        private void AgregarConcepto(
    Dictionary<string, TemaIngresoDTO> temas,
    ConceptosTarifarium concepto,
    decimal precio)
        {
            var nombreTema = concepto.Tema.Nombre;

            if (!temas.TryGetValue(nombreTema, out var tema))
            {
                tema = new TemaIngresoDTO
                {
                    Tema = nombreTema
                };
                temas.Add(nombreTema, tema);
            }

            tema.Conceptos.Add(new ConceptoIngresoDTO
            {
                ConceptoId = concepto.Id,
                Nombre = concepto.Nombre,
                PrecioBase = precio
            });
        }


    }
}
