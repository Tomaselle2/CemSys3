using CemSys3.DTOs.Tarifaria;
using CemSys3.Enumerables;
using CemSys3.Interfaces.Tarifaria;
using CemSys3.Models;
using Microsoft.EntityFrameworkCore;


namespace CemSys3.Business.Tarifaria
{
    public class TarifariaService : ITarifaria
    {
        private readonly AppDbContext _context;

        public TarifariaService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<NuevoIdDTO>> ActualizarPreciosTarifaria(List<PrecioActualizarDTO> preciosActualizar)
        {
            var nuevosIds = new List<NuevoIdDTO>();

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var paraActualizar = preciosActualizar.Where(p => p.Id > 0).ToList();
                var paraInsertar = preciosActualizar.Where(p => p.Id == 0).ToList();

                // ── UPDATE ──────────────────────────────────────────────────
                if (paraActualizar.Any())
                {
                    var ids = paraActualizar.Select(p => p.Id).ToList();
                    var existentes = await _context.PreciosTarifarias
                        .Where(p => ids.Contains(p.Id))
                        .ToListAsync();

                    foreach (var dto in paraActualizar)
                    {
                        var existente = existentes.FirstOrDefault(p => p.Id == dto.Id);
                        if (existente == null) continue;
                        existente.Precio = dto.Precio;
                    }
                }

                // ── INSERT ──────────────────────────────────────────────────
                if (paraInsertar.Any())
                {
                    foreach (var dto in paraInsertar)
                    {
                        if (dto.ConceptoTarifariaId == 0) continue;

                        var nuevo = new PreciosTarifaria
                        {
                            Precio = dto.Precio,
                            ConceptoTarifariaId = dto.ConceptoTarifariaId,
                            SeccionId = dto.SeccionId,
                            NroFila = dto.NroFila,
                            AniosConcesionId = dto.AniosConcesionId,
                            Visibilidad = true
                        };

                        await _context.PreciosTarifarias.AddAsync(nuevo);
                        await _context.SaveChangesAsync(); // guardar para obtener el Id generado

                        // Calcular los años reales para que el JS pueda ubicar el input
                        int? aniosRaw = dto.AniosConcesionId.HasValue
                            ? _mapaIdAAnios.GetValueOrDefault(dto.AniosConcesionId.Value)
                            : null;

                        nuevosIds.Add(new NuevoIdDTO
                        {
                            Id = nuevo.Id,
                            SeccionId = dto.SeccionId,
                            NroFila = dto.NroFila,
                            AniosRaw = aniosRaw
                        });
                    }
                }
                else
                {
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return nuevosIds;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al actualizar precios: {ex.Message}", ex);
            }
        }

        public async Task AplicarAumentoPorcentual(decimal porcentaje, int decimales)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Excluir el TemaId 7 (Fondo de ayuda) - los maneja el admin manualmente
                const int FONDO_TEMA_ID = 7;

                var preciosActualizar = await _context.PreciosTarifarias
                    .Where(p => p.Visibilidad == true &&
                                p.ConceptoTarifaria.TemaId != FONDO_TEMA_ID)
                    .ToListAsync();

                if (!preciosActualizar.Any())
                    throw new InvalidOperationException("No se encontraron precios para actualizar.");

                decimal factor = 1 + (porcentaje / 100);

                foreach (var precio in preciosActualizar)
                {
                    decimal nuevo = precio.Precio * factor;
                    precio.Precio = Redondear(nuevo, decimales);
                }

                var filas = await _context.SaveChangesAsync();

                if (filas == 0)
                    throw new InvalidOperationException("No se guardaron cambios.");

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error al aplicar aumento: {ex.Message}", ex);
            }
        }

        private static decimal Redondear(decimal valor, int decimales)
        {
            if (decimales >= 0)
                return Math.Round(valor, decimales, MidpointRounding.AwayFromZero);

            // decimales negativos: redondear a decenas, centenas, etc.
            decimal factor = (decimal)Math.Pow(10, -decimales);
            return Math.Round(valor / factor, MidpointRounding.AwayFromZero) * factor;
        }

        public async Task<IEnumerable<TarifariaRequestDTO>> GetPrecios()
        {
            var precios = await _context.PreciosTarifarias
                .Where(p => p.Visibilidad == true)
                .Include(p => p.ConceptoTarifaria)
                .AsNoTracking()
                .ToListAsync();                    // ← materializa en memoria primero

            return precios.Select(pre => new TarifariaRequestDTO
            {
                Id = pre.Id,
                Precio = pre.Precio,
                NroFila = pre.NroFila,
                ConceptoTarifariaId = pre.ConceptoTarifariaId,
                AniosConcesionId = pre.AniosConcesionId,
                SeccionId = pre.SeccionId,
                Visibilidad = pre.Visibilidad,
                TemaId = pre.ConceptoTarifaria?.TemaId ?? 0,
                NombreConcepto = pre.ConceptoTarifaria?.Nombre ?? string.Empty
            });
        }


        public async Task<PdfPreciosNichosDTO> ObtenerDatosParaPdfNichosFosasAsync()
        {
            // --- 1. Obtener porcentajes desde la tarifaria ------------------
            var todosLosPrecios = await _context.PreciosTarifarias
                .Where(p => p.Visibilidad == true)
                .Include(p => p.ConceptoTarifaria)
                .AsNoTracking()
                .ToListAsync();

            decimal porcentajeFondo = todosLosPrecios
                .Where(p => p.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.PorcentajeFondoAyudaCentroSalud)
                .Select(p => p.Precio)
                .FirstOrDefault();  // ej: 0.05

            decimal porcentajeOtrasLocalidades = todosLosPrecios
                .Where(p => p.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.PorcentajeAumentoConcesionesOtrasLocalidades)
                .Select(p => p.Precio)
                .FirstOrDefault();  // ej: 0.50

            // --- 2. Obtener secciones de tipo Nicho con sus precios --------
            var seccionesNicho = await _context.Secciones
                .Where(s => s.Visibilidad == true && s.TipoParcelaId == (int)TipoParcelaEnum.Nicho)
                .AsNoTracking()
                .ToListAsync();

            // Años de concesión que nos interesan (ordenados de mayor a menor como en el PDF)
            var aniosOrdenados = new[] { 25, 15, 10, 5 };
            var mapaAniosConcesion = new Dictionary<int, int>
    {
        { 25, (int)AniosConcesionEnum.anio25 },
        { 15, (int)AniosConcesionEnum.anio15 },
        { 10, (int)AniosConcesionEnum.anio10 },
        {  5, (int)AniosConcesionEnum.anio5  }
    };

            // Precios de nichos indexados por (seccionId, nroFila, aniosConcesionId)
            var preciosNicho = todosLosPrecios
                .Where(p => p.ConceptoTarifaria.TemaId == (int)TemaTarifariaEnum.ConcesionNicho
                            && p.SeccionId.HasValue
                            && p.NroFila.HasValue
                            && p.AniosConcesionId.HasValue)
                .ToList();

            // --- 3. Construir estructura por sección -----------------------
            // Para cada sección calculamos: por cada fila, precio base * (1 + fondo)
            // Luego agrupamos secciones que tengan igual cantidad de filas e iguales precios

            var seccionesConDatos = new List<(string Nombre, List<FilaNichoPdfDTO> Filas)>();

            foreach (var seccion in seccionesNicho)
            {
                var filasBruto = preciosNicho
                    .Where(p => p.SeccionId == seccion.Id)
                    .GroupBy(p => p.NroFila!.Value)
                    .OrderByDescending(g => g.Key)   // fila más alta primero (como en el PDF)
                    .ToList();

                if (!filasBruto.Any()) continue;

                // Calcular precios con fondo aplicado
                var filasCalculadas = new List<(int NroFila, Dictionary<int, decimal> PreciosPorAnio)>();
                foreach (var grupo in filasBruto)
                {
                    var preciosPorAnio = new Dictionary<int, decimal>();
                    foreach (var anio in aniosOrdenados)
                    {
                        var precioBase = grupo
                            .FirstOrDefault(p => p.AniosConcesionId == mapaAniosConcesion[anio])
                            ?.Precio ?? 0m;

                        // precio final local = base * (1 + fondo)
                        preciosPorAnio[anio] = AplicarPorcentaje(precioBase, porcentajeFondo);
                    }
                    filasCalculadas.Add((grupo.Key, preciosPorAnio));
                }

                // Agrupar filas contiguas con los mismos precios dentro de esta sección
                var filasAgrupadas = AgruparFilasConMismoPrecio(filasCalculadas);
                seccionesConDatos.Add((seccion.Nombre, filasAgrupadas));
            }

            // --- 4. Agrupar secciones con misma cantidad de filas e iguales precios ---
            var gruposLocales = AgruparSecciones(seccionesConDatos);

            // --- 5. Construir grupos para otras jurisdicciones (misma lógica, diferente %) ---
            // precio_otras = base * (1 + otras) y luego * (1 + fondo)
            // Según lo indicado: 50% se aplica sobre el precio base, luego se aplica el fondo

            var seccionesConDatosOtras = new List<(string Nombre, List<FilaNichoPdfDTO> Filas)>();

            foreach (var seccion in seccionesNicho)
            {
                var filasBruto = preciosNicho
                    .Where(p => p.SeccionId == seccion.Id)
                    .GroupBy(p => p.NroFila!.Value)
                    .OrderByDescending(g => g.Key)
                    .ToList();

                if (!filasBruto.Any()) continue;

                var filasCalculadas = new List<(int NroFila, Dictionary<int, decimal> PreciosPorAnio)>();
                foreach (var grupo in filasBruto)
                {
                    var preciosPorAnio = new Dictionary<int, decimal>();
                    foreach (var anio in aniosOrdenados)
                    {
                        var precioBase = grupo
                            .FirstOrDefault(p => p.AniosConcesionId == mapaAniosConcesion[anio])
                            ?.Precio ?? 0m;

                        // precio otras = base + (base * otras) → luego + fondo
                        var precioConOtras = AplicarPorcentaje(precioBase, porcentajeOtrasLocalidades);
                        preciosPorAnio[anio] = AplicarPorcentaje(precioConOtras, porcentajeFondo);
                    }
                    filasCalculadas.Add((grupo.Key, preciosPorAnio));
                }

                var filasAgrupadas = AgruparFilasConMismoPrecio(filasCalculadas);
                seccionesConDatosOtras.Add((seccion.Nombre, filasAgrupadas));
            }

            var gruposOtras = AgruparSecciones(seccionesConDatosOtras);

            // --- 6bis. Nichos especiales (sin sección) ----------------------
            var preciosEspeciales = todosLosPrecios
                .Where(p => p.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.ConcesionNicho
                            && !p.SeccionId.HasValue
                            && p.NroFila.HasValue
                            && p.AniosConcesionId.HasValue)
                .ToList();

            var nichosEspecialesLocales = ConstruirFilasNichoEspecial(
                preciosEspeciales, aniosOrdenados, mapaAniosConcesion, porcentajeFondo, null);

            var nichosEspecialesOtras = ConstruirFilasNichoEspecial(
                preciosEspeciales, aniosOrdenados, mapaAniosConcesion, porcentajeFondo, porcentajeOtrasLocalidades);

            // --- 6. Fosas --------------------------------------------------
            var preciosFosa = todosLosPrecios
                .Where(p => p.ConceptoTarifariaId == (int)ConceptosTarifariaEnum.ConcesionFosa
                            && p.AniosConcesionId.HasValue)
                .ToList();

            // Años disponibles para fosa: 15 y 25
            var aniosFosa = new[] { 15, 25 };

            var fosasLocales = new List<PrecioFosaPdfDTO>();
            var fosasOtras = new List<PrecioFosaPdfDTO>();

            foreach (var anio in aniosFosa)
            {
                var anioId = mapaAniosConcesion[anio];
                var precioBase = preciosFosa
                    .FirstOrDefault(p => p.AniosConcesionId == anioId)
                    ?.Precio ?? 0m;

                fosasLocales.Add(new PrecioFosaPdfDTO
                {
                    Etiqueta = $"Por {anio} años",
                    Precio = AplicarPorcentaje(precioBase, porcentajeFondo)
                });

                var precioConOtras = AplicarPorcentaje(precioBase, porcentajeOtrasLocalidades);
                fosasOtras.Add(new PrecioFosaPdfDTO
                {
                    Etiqueta = $"Por {anio} años",
                    Precio = AplicarPorcentaje(precioConOtras, porcentajeFondo)
                });
            }

            return new PdfPreciosNichosDTO
            {
                PorcentajeFondo = porcentajeFondo,
                GruposNichosLocales = gruposLocales,
                GruposNichosOtrasJurisdicciones = gruposOtras,
                FosasLocales = fosasLocales,
                FosasOtrasJurisdicciones = fosasOtras,
                NichosEspecialesLocales = nichosEspecialesLocales,   // NUEVO
                NichosEspecialesOtras = nichosEspecialesOtras        // NUEVO
            };
        }

        /// <summary>
        /// Aplica un porcentaje decimal al precio: precio * (1 + porcentaje)
        /// porcentaje = 0.05 → +5 %
        /// </summary>
        private static decimal AplicarPorcentaje(decimal precio, decimal porcentaje)
            => Math.Round(precio * (1 + porcentaje), 3);

        /// <summary>
        /// Agrupa filas contiguas (ordenadas de mayor a menor) que tengan exactamente
        /// los mismos precios en todos los años de concesión.
        /// Ej: fila 2 y fila 3 con iguales precios → "2° y 3° FILA"
        /// </summary>
        private static List<FilaNichoPdfDTO> AgruparFilasConMismoPrecio(
            List<(int NroFila, Dictionary<int, decimal> PreciosPorAnio)> filas)
        {
            var resultado = new List<FilaNichoPdfDTO>();
            if (!filas.Any()) return resultado;

            var grupoActual = new List<int> { filas[0].NroFila };
            var preciosActuales = filas[0].PreciosPorAnio;

            for (int i = 1; i < filas.Count; i++)
            {
                var mismosPrecio = preciosActuales.Keys
                    .All(k => filas[i].PreciosPorAnio.TryGetValue(k, out var v) && v == preciosActuales[k]);

                if (mismosPrecio)
                {
                    grupoActual.Add(filas[i].NroFila);
                }
                else
                {
                    resultado.Add(new FilaNichoPdfDTO
                    {
                        Etiqueta = BuildEtiquetaFila(grupoActual),
                        PreciosPorAnio = preciosActuales
                    });
                    grupoActual = new List<int> { filas[i].NroFila };
                    preciosActuales = filas[i].PreciosPorAnio;
                }
            }

            resultado.Add(new FilaNichoPdfDTO
            {
                Etiqueta = BuildEtiquetaFila(grupoActual),
                PreciosPorAnio = preciosActuales
            });

            return resultado;
        }

        /// <summary>
        /// Construye la etiqueta de fila según la lista de números de fila.
        /// Ej: [1] → "1° FILA (ABAJO)"  |  [2,3] → "2° y 3° FILA"  |  [4,5] → "4° y 5° FILA"
        /// </summary>
        private static string BuildEtiquetaFila(List<int> nrosFilas)
        {
            if (nrosFilas.Count == 1)
            {
                var n = nrosFilas[0];
                var sufijo = n == 1 ? " (ABAJO)" : string.Empty;
                return $"{n}° FILA{sufijo}";
            }

            var partes = nrosFilas.Select(n => $"{n}°").ToList();
            return string.Join(" y ", partes) + " FILA";
        }

        /// <summary>
        /// Agrupa secciones que tienen la misma cantidad de filas Y los mismos precios exactos.
        /// </summary>
        private static List<GrupoSeccionNichoPdfDTO> AgruparSecciones(
            List<(string Nombre, List<FilaNichoPdfDTO> Filas)> secciones)
        {
            var grupos = new List<GrupoSeccionNichoPdfDTO>();
            var usadas = new HashSet<int>();

            for (int i = 0; i < secciones.Count; i++)
            {
                if (usadas.Contains(i)) continue;

                var seccionBase = secciones[i];
                var nombresGrupo = new List<string> { seccionBase.Nombre };

                for (int j = i + 1; j < secciones.Count; j++)
                {
                    if (usadas.Contains(j)) continue;

                    var otra = secciones[j];

                    if (SonSeccionesIguales(seccionBase.Filas, otra.Filas))
                    {
                        nombresGrupo.Add(otra.Nombre);
                        usadas.Add(j);
                    }
                }

                usadas.Add(i);
                grupos.Add(new GrupoSeccionNichoPdfDTO
                {
                    NombreSecciones = string.Join(", ", nombresGrupo),
                    Filas = seccionBase.Filas
                });
            }

            return grupos;
        }

        /// <summary>
        /// Compara dos listas de filas: misma cantidad, mismas etiquetas, mismos precios.
        /// </summary>
        private static bool SonSeccionesIguales(List<FilaNichoPdfDTO> a, List<FilaNichoPdfDTO> b)
        {
            if (a.Count != b.Count) return false;

            for (int i = 0; i < a.Count; i++)
            {
                var fa = a[i];
                var fb = b[i];

                if (fa.PreciosPorAnio.Count != fb.PreciosPorAnio.Count) return false;

                foreach (var kvp in fa.PreciosPorAnio)
                {
                    if (!fb.PreciosPorAnio.TryGetValue(kvp.Key, out var vb) || vb != kvp.Value)
                        return false;
                }
            }

            return true;
        }

        public class NuevoIdDTO
        {
            public int Id { get; set; }
            public int? SeccionId { get; set; }
            public int? NroFila { get; set; }
            public int? AniosRaw { get; set; }  // los años reales (5,10,15,25) para que el JS ubique el input
        }

        // Mapa inverso: AniosConcesionId → años reales
        private static readonly Dictionary<int, int> _mapaIdAAnios = new()
{
    { 5, 25 },
    { 4, 15 },
    { 3, 10 },
    { 2,  5 },
    { 1,  1 }
};

        private List<FilaNichoPdfDTO> ConstruirFilasNichoEspecial(
    List<PreciosTarifaria> preciosEspeciales,
    int[] aniosOrdenados,
    Dictionary<int, int> mapaAniosConcesion,
    decimal porcentajeFondo,
    decimal? porcentajeOtras)
        {
            var filasBruto = preciosEspeciales
                .GroupBy(p => p.NroFila!.Value)
                .OrderByDescending(g => g.Key)
                .ToList();

            var filasCalculadas = new List<(int NroFila, Dictionary<int, decimal> PreciosPorAnio)>();

            foreach (var grupo in filasBruto)
            {
                var preciosPorAnio = new Dictionary<int, decimal>();
                foreach (var anio in aniosOrdenados)
                {
                    var precioBase = grupo
                        .FirstOrDefault(p => p.AniosConcesionId == mapaAniosConcesion[anio])
                        ?.Precio ?? 0m;

                    decimal precioFinal = porcentajeOtras.HasValue
                        ? AplicarPorcentaje(AplicarPorcentaje(precioBase, porcentajeOtras.Value), porcentajeFondo)
                        : AplicarPorcentaje(precioBase, porcentajeFondo);

                    preciosPorAnio[anio] = precioFinal;
                }
                filasCalculadas.Add((grupo.Key, preciosPorAnio));
            }

            return AgruparFilasConMismoPrecio(filasCalculadas);
        }


    }
}
