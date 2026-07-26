using CemSys3.Business.Cementerio;
using CemSys3.DTOs.CargaInicial;
using CemSys3.Enumerables;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.CargaIncialCemSys;
using CemSys3.ViewModels.Cementerio;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace CemSys3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CargaInicialController : ControllerBase
    {
        private readonly ICargaInicial _cargaInicialModoPrueba;
        private readonly ICargaInicial _cargaInicialReal;

        // Se registran dos instancias del mismo servicio en Program.cs, una en modo
        // prueba (no persiste nada) y otra en modo real. Ver README para el registro
        // en el contenedor de DI.
        public CargaInicialController(
            [FromKeyedServices("prueba")] ICargaInicial cargaInicialModoPrueba,
            [FromKeyedServices("real")] ICargaInicial cargaInicialReal)
        {
            _cargaInicialModoPrueba = cargaInicialModoPrueba;
            _cargaInicialReal = cargaInicialReal;
        }

        
        /// <summary>
        /// Corre el proceso en modo prueba: valida y genera los archivos de éxito/error
        /// pero NO persiste nada en la base. Pensado para iterar antes de la carga real.
        /// </summary>
        [HttpPost("simular")]
        [RequestSizeLimit(50_000_000)]
        public async Task<IActionResult> Simular(IFormFile archivo)
        {
            var resumen = await _cargaInicialModoPrueba.CargaInicial(archivo);
            return ArmarZip(resumen);
        }

        /// <summary>
        /// Corre el proceso en modo real: esto SÍ persiste en la base de datos.
        /// Usar solo cuando ya se probó varias veces con /simular y el archivo de
        /// errores quedó en un tamaño aceptable para corregir a mano.
        /// </summary>
        [HttpPost("ejecutar")]
        [RequestSizeLimit(50_000_000)]
        public async Task<IActionResult> Ejecutar(IFormFile archivo)
        {
            var resumen = await _cargaInicialReal.CargaInicial(archivo);
            return ArmarZip(resumen);
        }

        private FileContentResult ArmarZip(CemSys3.DTOs.CargaInicial.CargaInicialResumenDTO resumen)
        {
            using var memoryStream = new MemoryStream();
            using (var archivoZip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                AgregarEntradaZip(archivoZip, "exitosos.csv", resumen.ArchivoExitososCsv);
                AgregarEntradaZip(archivoZip, "errores.csv", resumen.ArchivoErroresCsv);
                AgregarEntradaZip(archivoZip, "resumen.txt", System.Text.Encoding.UTF8.GetBytes(
                    $"Modo prueba: {resumen.ModoPrueba}\n" +
                    $"Total filas: {resumen.TotalFilas}\n" +
                    $"Total grupos (concesiones): {resumen.TotalGrupos}\n" +
                    $"Exitosas: {resumen.TotalExitosas}\n" +
                    $"Errores: {resumen.TotalErrores}\n"));
            }

            return new FileContentResult(memoryStream.ToArray(), "application/zip")
            {
                FileDownloadName = $"resultado_carga_inicial_{System.DateTime.Now:yyyyMMdd_HHmmss}.zip"
            };
        }

        private static void AgregarEntradaZip(ZipArchive zip, string nombre, byte[] contenido)
        {
            var entrada = zip.CreateEntry(nombre);
            using var stream = entrada.Open();
            stream.Write(contenido, 0, contenido.Length);
        }
    }
}
