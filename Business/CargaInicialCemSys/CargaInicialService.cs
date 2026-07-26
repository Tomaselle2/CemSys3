using CemSys3.DTOs.CargaInicial;
using CemSys3.DTOs.HistorialEstado;
using CemSys3.Enumerables;
using CemSys3.Helpers.CargaInicial;
using CemSys3.Interfaces.CargaIncialCemSys;
using CemSys3.Interfaces.HistorialEstados;
using CemSys3.Models;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CemSys3.Business.CargaInicialCemSys
{
    public class CargaInicialService : ICargaInicial
    {
        private readonly AppDbContext _context;
        private readonly IHistorialEstados _historialEstados;

        /// <summary>
        /// Si es true, todo se procesa igual (validaciones, lookups, constraints de EF)
        /// pero cada grupo se revierte al final -> no queda nada persistido en la base.
        /// Pensado para poder correr el proceso las veces que haga falta antes de la
        /// carga real con el cliente.
        /// </summary>
        private readonly bool _modoPrueba;

        // Los csv exportados desde Excel/sistemas viejos en Argentina suelen venir en
        // Windows-1252 (Latin1), no en UTF-8. Si ves que las tildes/ñ salen mal, este es
        // el primer lugar para revisar.
        private static readonly Encoding EncodingCsvEntrada = Encoding.GetEncoding(1252);

        private static readonly DateOnly FechaPorDefecto1900 = new(1900, 1, 1);
        private static readonly DateOnly FechaVtoPanteon = new(9999, 12, 30);

        public CargaInicialService(AppDbContext context, IHistorialEstados historialEstados, bool modoPrueba)
        {
            _context = context;
            _historialEstados = historialEstados;
            _modoPrueba = modoPrueba;
        }

        public async Task<CargaInicialResumenDTO> CargaInicial(IFormFile excel)
        {
            var filas = LeerFilasCsv(excel);

            var resultados = new List<ResultadoFilaCarga>(filas.Count);

            var grupos = filas
                .GroupBy(f => (f.Concesion ?? string.Empty).Trim())
                .ToList();

            foreach (var grupo in grupos)
            {
                await ProcesarGrupoAsync(grupo.Key, grupo.ToList(), resultados);
            }

            var resumen = new CargaInicialResumenDTO
            {
                TotalFilas = filas.Count,
                TotalGrupos = grupos.Count,
                TotalExitosas = resultados.Count(r => r.Exito),
                TotalErrores = resultados.Count(r => !r.Exito),
                ModoPrueba = _modoPrueba,
                ArchivoExitososCsv = GenerarCsvExitosos(resultados.Where(r => r.Exito)),
                ArchivoErroresCsv = GenerarCsvErrores(resultados.Where(r => !r.Exito))
            };

            return resumen;
        }

        // ---------------------------------------------------------------
        // Lectura del csv
        // ---------------------------------------------------------------

        private List<CargaInicialCsvRow> LeerFilasCsv(IFormFile excel)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true,
                MissingFieldFound = null,
                BadDataFound = null,
                HeaderValidated = null,
                TrimOptions = TrimOptions.Trim,
                // El encabezado termina con una coma extra -> columna fantasma al final, se ignora sola.
            };

            using var stream = excel.OpenReadStream();
            using var reader = new StreamReader(stream, EncodingCsvEntrada);
            using var csv = new CsvReader(reader, config);

            csv.Read();
            csv.ReadHeader();

            var filas = new List<CargaInicialCsvRow>();
            int numeroFila = 1; // 1-based, sin contar encabezado

            while (csv.Read())
            {
                var fila = csv.GetRecord<CargaInicialCsvRow>();
                fila.NumeroFilaOriginal = numeroFila;
                filas.Add(fila);
                numeroFila++;
            }

            return filas;
        }

        // ---------------------------------------------------------------
        // Procesamiento de un grupo (= una concesión, 1 o más difuntos)
        // ---------------------------------------------------------------

        private async Task ProcesarGrupoAsync(string concesionCruda, List<CargaInicialCsvRow> filasGrupo, List<ResultadoFilaCarga> resultados)
        {
            void MarcarTodasError(string motivo)
            {
                foreach (var f in filasGrupo)
                {
                    resultados.Add(new ResultadoFilaCarga { Fila = f, Exito = false, Motivo = motivo });
                }
            }

            var filaBase = filasGrupo[0];

            // --- Validaciones de encabezado del grupo ---

            if (!int.TryParse(concesionCruda, NumberStyles.Integer, CultureInfo.InvariantCulture, out int numeroConcesion))
            {
                MarcarTodasError($"Número de concesión inválido: '{concesionCruda}'");
                return;
            }

            string tipoCsv = filaBase.Tipo?.Trim() ?? string.Empty;
            int tipoParcelaId;
            try
            {
                tipoParcelaId = ParcelaCodeParser.TipoParcelaIdDesdeCsv(tipoCsv);
            }
            catch (Exception ex)
            {
                MarcarTodasError(ex.Message);
                return;
            }

            var parcelaParse = ParcelaCodeParser.Parse(filaBase.Parcela, tipoCsv);
            if (!parcelaParse.EsValido)
            {
                MarcarTodasError($"PARCELA '{filaBase.Parcela}': {parcelaParse.Error}");
                return;
            }

            var seccion = await _context.Secciones.FirstOrDefaultAsync(s =>
                s.Nombre == parcelaParse.NombreSeccion && s.TipoParcelaId == tipoParcelaId);

            if (seccion == null)
            {
                MarcarTodasError(
                    $"No existe una Sección '{parcelaParse.NombreSeccion}' de tipo '{tipoCsv}' en el sistema nuevo. " +
                    "Revisar manualmente (posible sección renombrada/consolidada).");
                return;
            }

            var parcela = await _context.Parcelas.FirstOrDefaultAsync(p =>
                p.SeccionId == seccion.Id && p.NroParcela == parcelaParse.NroParcela && p.NroFila == parcelaParse.NroFila);

            if (parcela == null)
            {
                MarcarTodasError(
                    $"No existe la Parcela Nº {parcelaParse.NroParcela}, Fila {parcelaParse.NroFila} en la Sección " +
                    $"'{parcelaParse.NombreSeccion}' (SeccionId={seccion.Id}).");
                return;
            }

            // Guarda de idempotencia: si esta concesión ya se cargó para esta parcela
            // (ej. se corrió el proceso sin borrar la base antes), no se duplica.
            bool yaExiste = await _context.Concesiones.AnyAsync(c =>
                c.Concesion == numeroConcesion && c.ParcelaId == parcela.Id);

            if (yaExiste)
            {
                foreach (var f in filasGrupo)
                {
                    resultados.Add(new ResultadoFilaCarga
                    {
                        Fila = f,
                        Exito = true,
                        Motivo = "La concesión ya existía para esta parcela, no se volvió a cargar."
                    });
                }
                return;
            }

            // --- Fechas y estado de la concesión ---

            var activo = (filaBase.Activo ?? "").Trim();
            var fechaInicio = ParsearFecha(filaBase.FechaInicio, FechaPorDefecto1900);

            DateOnly fechaVto;
            if (tipoCsv.Equals("Panteon", StringComparison.OrdinalIgnoreCase) ||
                tipoCsv.Equals("Panteón", StringComparison.OrdinalIgnoreCase))
            {
                fechaVto = FechaVtoPanteon; // se ignora lo que venga en el csv
            }
            else
            {
                fechaVto = ParsearFecha(filaBase.FechaVto, FechaPorDefecto1900);
            }

            int estadoActualId;
            if (activo == "0")
            {
                estadoActualId = (int)EstadosConcesionEnum.Caducado;
            }
            else
            {
                var hoy = DateOnly.FromDateTime(DateTime.Today);
                estadoActualId = fechaVto > hoy
                    ? (int)EstadosConcesionEnum.Vigente
                    : (int)EstadosConcesionEnum.Vencido;
            }

            // Una concesión Caducada (ACTIVO=0) es, en el sistema real, una concesión que
            // ya se "cerró": se le quitan los difuntos, la parcela queda libre, el titular
            // deja de tener el historial abierto y el vencimiento se anula. Replicamos ese
            // cierre acá mismo en la carga inicial (no hay una fecha real de cierre en el
            // csv, así que se usa FechaInicio + 1 día, solo para que quede algo posterior
            // a la fecha de inicio y no genere inconsistencias de fechas).
            bool esCaducada = estadoActualId == (int)EstadosConcesionEnum.Caducado;
            DateTime? fechaCierre = esCaducada
                ? fechaInicio.AddDays(1).ToDateTime(TimeOnly.MinValue)
                : null;

            // --- Titular (ENCARGADO_PAGO) ---

            var nombreApellidoTitular = DocumentoHelper.SepararNombreApellido(filaBase.EncargadoPago);
            var dniTitular = DocumentoHelper.ExtraerDni(filaBase.TipoDocEncargadoPago, filaBase.DocumentoEncargadoPago);

            if (string.IsNullOrWhiteSpace(dniTitular))
            {
                MarcarTodasError("El titular (ENCARGADO_PAGO) no tiene un documento válido.");
                return;
            }

            dniTitular = dniTitular.PadLeft(8, '0');

            // Una única transacción para TODO el grupo (concesión + todos sus difuntos).
            // Así el tramiteId creado acá sigue siendo visible durante todo el procesamiento,
            // incluso en modo prueba (donde se revierte recién al final, una sola vez).
            using var transaccionGrupo = await _context.Database.BeginTransactionAsync();
            int tramiteId;
            int titularId;

            try
            {
                titularId = await BuscarOCrearPersonaAsync(
                    dni: dniTitular,
                    nombre: nombreApellidoTitular.Nombre,
                    apellido: nombreApellidoTitular.Apellido,
                    sexoCsv: filaBase.SexoEncargadoCsv,
                    correo: NuloSiVacio(filaBase.MailEncargado),
                    celular: NuloSiVacio(filaBase.NumCelular),
                    categoriaPersonaId: (int)CategoriaPersonaEnum.Titular,
                    fechaDefuncion: null,
                    estadoDifuntoId: null);

                tramiteId = await ObtenerProximoIdTramiteAsync();

                var tramite = new Models.Tramite
                {
                    Id = tramiteId,
                    Visibilidad = true,
                    FechaCreacion = fechaInicio.ToDateTime(TimeOnly.MinValue),
                    TipoTramiteId = (int)TipoTramiteEnum.ContratoConcesion,
                    UsuarioId = 1,
                    EstadoActualId = estadoActualId
                };
                await _context.Tramites.AddAsync(tramite);
                await _context.SaveChangesAsync();

                await _historialEstados.Add(new HistorialEstadosDTO
                {
                    Fecha = tramite.FechaCreacion,
                    TramiteId = tramiteId,
                    EstadoTramiteId = estadoActualId
                });

                var concesion = new Models.Concesione
                {
                    TramiteId = tramiteId,
                    Concesion = numeroConcesion,
                    Visibilidad = true,
                    TipoParcela = tipoCsv,
                    Vencimiento = esCaducada ? (DateOnly?)null : fechaVto,
                    FechaFin = fechaCierre,
                    ParcelaId = parcela.Id,
                    FechaInicio = fechaInicio.ToDateTime(TimeOnly.MinValue),
                    UsuarioId = 1,
                    InformacionAdicional = esCaducada
                        ? $"\n● Carga inicial: concesión Nº {numeroConcesion:00000} importada desde el sistema anterior (caducada, parcela liberada)."
                        : $"\n● Carga inicial: concesión Nº {numeroConcesion:00000} importada desde el sistema anterior."
                };
                await _context.Concesiones.AddAsync(concesion);

                await _historialEstados.VincularTramiteAParcela(tramiteId, parcela.Id);

                await _context.HistorialParcelasConcesions.AddAsync(new Models.HistorialParcelasConcesion
                {
                    ConcesionId = tramiteId,
                    ParcelaId = parcela.Id,
                    FechaInicio = fechaInicio.ToDateTime(TimeOnly.MinValue),
                    FechaFin = fechaCierre, // null si no es caducada -> parcela "actual" para esta concesión
                    TramiteOrigenId = tramiteId
                });

                await _historialEstados.VincularTramiteAPersona(tramiteId, titularId);
                await _historialEstados.VincularTitularAConcesion(titularId, tramiteId);

                if (esCaducada)
                {
                    // El método VincularTitularAConcesion no permite pasar FechaFin, así que
                    // buscamos el registro recién creado y lo cerramos acá.
                    // OJO: "Models.HistorialTitularesConcesione" es un nombre asumido siguiendo
                    // el mismo patrón de tu scaffolding (Concesiones -> Concesione). Si no
                    // compila, ajustar el nombre de la clase por el real.
                    var historialTitular = await _context.HistorialTitularesConcesiones
                        .Where(h => h.ConcesionId == tramiteId && h.PersonaId == titularId && h.FechaFin == null)
                        .OrderByDescending(h => h.Id)
                        .FirstOrDefaultAsync();

                    if (historialTitular != null)
                        historialTitular.FechaFin = fechaCierre;
                }

                parcela.InformacionAdicional += esCaducada
                    ? $"\n● Carga inicial: concesión Nº {numeroConcesion:00000} (caducada) importada y cerrada; parcela liberada."
                    : $"\n● Carga inicial: se generó la concesión Nº {numeroConcesion:00000}.";

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await transaccionGrupo.RollbackAsync();
                _context.ChangeTracker.Clear();
                MarcarTodasError($"Error creando la concesión: {ex.Message}");
                return;
            }

            _context.ChangeTracker.Clear();

            // --- Difuntos: cada fila usa un savepoint dentro de la MISMA transacción del grupo ---
            // Si uno falla, se revierte solo hasta su savepoint (no afecta a los demás ni a la
            // concesión ya creada), pero el tramiteId sigue existiendo dentro de la transacción
            // para los difuntos siguientes.

            foreach (var fila in filasGrupo)
            {
                var savepoint = $"sp_{fila.NumeroFilaOriginal}";
                bool savepointCreado = false;

                try
                {
                    await transaccionGrupo.CreateSavepointAsync(savepoint);
                    savepointCreado = true;

                    var nombreApellidoDifunto = DocumentoHelper.SepararNombreApellido(fila.Fallecido);
                    var dniDifunto = DocumentoHelper.ExtraerDni(fila.TipoDocumentoFallecido, fila.DocumentoFallecido);

                    if (string.IsNullOrWhiteSpace(dniDifunto))
                        throw new InvalidOperationException($"El fallecido '{fila.Fallecido}' no tiene un documento válido.");

                    dniDifunto = dniDifunto.PadLeft(8, '0');

                    DateOnly? fechaFallecimiento = ParsearFechaNullable(fila.FechaFallecimiento);

                    var difuntoId = await BuscarOCrearPersonaAsync(
                        dni: dniDifunto,
                        nombre: nombreApellidoDifunto.Nombre,
                        apellido: nombreApellidoDifunto.Apellido,
                        sexoCsv: fila.SexoFallecidoCsv,
                        correo: null,
                        celular: null,
                        categoriaPersonaId: (int)CategoriaPersonaEnum.Fallecido,
                        fechaDefuncion: fechaFallecimiento,
                        estadoDifuntoId: (int)EstadoDifuntoEnum.CuerpoCompleto);

                    await _historialEstados.VincularTramiteAPersona(tramiteId, difuntoId);

                    await _context.ParcelaDifuntos.AddAsync(new Models.ParcelaDifunto
                    {
                        ParcelaId = parcela.Id,
                        DifuntoId = difuntoId,
                        FechaIngreso = fechaInicio.ToDateTime(TimeOnly.MinValue),
                        TramiteIngresoId = null,
                        // Si la concesión ya está caducada, este difunto también se marca
                        // como retirado (no queda ocupando la parcela).
                        FechaRetiro = esCaducada ? fechaCierre : null
                    });

                    if (!esCaducada)
                    {
                        var parcelaFresca = await _context.Parcelas.FindAsync(parcela.Id);
                        if (parcelaFresca != null)
                            parcelaFresca.CantidadDifuntos += 1;
                    }

                    if (fechaFallecimiento == null)
                    {
                        var concesionFresca = await _context.Concesiones.FindAsync(tramiteId);
                        var nombreCompleto = $"{nombreApellidoDifunto.Apellido} {nombreApellidoDifunto.Nombre}".Trim();
                        if (concesionFresca != null)
                        {
                            concesionFresca.InformacionAdicional +=
                                $"\n● ATENCIÓN: falta la fecha de fallecimiento de {nombreCompleto.ToUpperInvariant()}, corregir manualmente.";
                        }
                    }

                    await _context.SaveChangesAsync();

                    resultados.Add(new ResultadoFilaCarga
                    {
                        Fila = fila,
                        Exito = true,
                        Motivo = fechaFallecimiento == null
                            ? "Cargado OK (falta fecha de fallecimiento, revisar)"
                            : "Cargado OK",
                        TramiteConcesionId = tramiteId,
                        DifuntoPersonaId = difuntoId,
                        TitularPersonaId = titularId
                    });
                }
                catch (Exception ex)
                {
                    if (savepointCreado)
                        await transaccionGrupo.RollbackToSavepointAsync(savepoint);

                    resultados.Add(new ResultadoFilaCarga
                    {
                        Fila = fila,
                        Exito = false,
                        Motivo = $"Error cargando al difunto: {ex.Message}",
                        TramiteConcesionId = tramiteId
                    });
                }
                finally
                {
                    _context.ChangeTracker.Clear();
                }
            }

            // Recién acá se decide el destino de TODO el grupo (concesión + difuntos que
            // hayan quedado bien tras sus savepoints).
            if (_modoPrueba)
                await transaccionGrupo.RollbackAsync();
            else
                await transaccionGrupo.CommitAsync();
        }

        // ---------------------------------------------------------------
        // Personas: buscar por dni o crear
        // ---------------------------------------------------------------

        private async Task<int> BuscarOCrearPersonaAsync(
            string dni,
            string nombre,
            string apellido,
            string? sexoCsv,
            string? correo,
            string? celular,
            int categoriaPersonaId,
            DateOnly? fechaDefuncion,
            int? estadoDifuntoId)
        {
            var existente = await _context.Personas.FirstOrDefaultAsync(p => p.Dni == dni);
            if (existente != null)
                return existente.Id;

            var nombreCompleto = $"{nombre} {apellido}".Trim();
            var sexo = SexoDetector.DesdeCsvOHeuristica(sexoCsv, nombre);

            var nueva = new Models.Persona
            {
                Nombre = nombre,
                Apellido = apellido,
                Dni = dni,
                Visibilidad = true,
                Sexo = sexo,
                Correo = correo,
                Celular = celular,
                FechaDefuncion = fechaDefuncion,
                CategoriaPersonaId = categoriaPersonaId,
                EstadoDifuntoId = estadoDifuntoId
            };

            await _context.Personas.AddAsync(nueva);
            await _context.SaveChangesAsync();
            return nueva.Id;
        }

        // ---------------------------------------------------------------
        // Utilidades
        // ---------------------------------------------------------------

        private async Task<int> ObtenerProximoIdTramiteAsync()
        {
            int? maxId = await _context.Tramites.MaxAsync(t => (int?)t.Id);
            return (maxId ?? 0) + 1;
        }

        private static string? NuloSiVacio(string? valor) =>
            string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();

        private static DateOnly ParsearFecha(string? crudo, DateOnly porDefecto)
        {
            return ParsearFechaNullable(crudo) ?? porDefecto;
        }

        private static DateOnly? ParsearFechaNullable(string? crudo)
        {
            if (string.IsNullOrWhiteSpace(crudo))
                return null;

            if (DateOnly.TryParseExact(crudo.Trim(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
                return fecha;

            return null;
        }

        // ---------------------------------------------------------------
        // Generación de los archivos de salida (éxito / error)
        // ---------------------------------------------------------------

        private byte[] GenerarCsvExitosos(IEnumerable<ResultadoFilaCarga> filas)
        {
            var registros = filas.Select(r => new
            {
                r.Fila.Nro,
                r.Fila.Parcela,
                r.Fila.Concesion,
                r.Fila.Tipo,
                r.Fila.EncargadoPago,
                r.Fila.Fallecido,
                TramiteConcesionId = r.TramiteConcesionId,
                TitularPersonaId = r.TitularPersonaId,
                DifuntoPersonaId = r.DifuntoPersonaId,
                Observacion = r.Motivo
            });

            return EscribirCsv(registros);
        }

        private byte[] GenerarCsvErrores(IEnumerable<ResultadoFilaCarga> filas)
        {
            var registros = filas.Select(r => new
            {
                r.Fila.Nro,
                r.Fila.Parcela,
                r.Fila.Concesion,
                r.Fila.Tipo,
                r.Fila.EncargadoPago,
                r.Fila.Fallecido,
                Motivo = r.Motivo
            });

            return EscribirCsv(registros);
        }

        private static byte[] EscribirCsv<T>(IEnumerable<T> registros)
        {
            using var memoryStream = new MemoryStream();
            using (var writer = new StreamWriter(memoryStream, new UTF8Encoding(true), leaveOpen: true))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(registros);
            }

            return memoryStream.ToArray();
        }
    }
}