using CemSys3.Enumerables;
using CemSys3.Helpers.Enumerable;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Reportes;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class ReporteProblemasController : Controller
    {
        private readonly IReporteProblemasService _reporteProblemasService;

        public ReporteProblemasController(IReporteProblemasService reporteProblemasService)
        {
            _reporteProblemasService = reporteProblemasService;
        }

        [AuthorizeRole(RolUsuario.Administrador)]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // ─── Exportar Excel: Concesiones sin difuntos ──────────────────────
        [AuthorizeRole(RolUsuario.Administrador)]
        [HttpGet]
        public async Task<IActionResult> ExportarExcelConcesionesSinDifuntos()
        {
            var datos = await _reporteProblemasService.GetConcesionesSinDifuntos();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("SinDifuntos");

            string[] headers = { "Concesión", "Parcela", "Sección", "Fila",  "Vencimiento" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#2E75B6");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            int fila = 2;
            foreach (var item in datos)
            {
                ws.Cell(fila, 1).Value = item.Concesion?.ToString("D5") ?? "---";
                ws.Cell(fila, 2).Value = item.NroParcela?.ToString() ?? "";
                ws.Cell(fila, 3).Value = item.Seccion?.ToUpper() ?? "";
                ws.Cell(fila, 4).Value = item.NroFila?.ToString() ?? "";
                ws.Cell(fila, 5).Value = item.Vencimiento?.ToString("dd/MM/yyyy") ?? "";
                fila++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            string fileName = $"ConcesionesSinDifuntos_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        // ─── Exportar Excel: Titular que también es difunto en la misma parcela ───
        [AuthorizeRole(RolUsuario.Administrador)]
        [HttpGet]
        public async Task<IActionResult> ExportarExcelTitularesFallecidos()
        {
            var datos = await _reporteProblemasService.GetConcesionesConTitularFallecido();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("TitularEsFallecido");

            string[] headers = { "Concesión", "Parcela", "Sección", "Fila", "Persona (Titular y Fallecido)", "Vencimiento" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#2E75B6");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            int fila = 2;
            foreach (var item in datos)
            {
                ws.Cell(fila, 1).Value = item.Concesion?.ToString("D5") ?? "---";
                ws.Cell(fila, 2).Value = item.NroParcela?.ToString() ?? "";
                ws.Cell(fila, 3).Value = item.Seccion?.ToUpper() ?? "";
                ws.Cell(fila, 4).Value = item.NroFila?.ToString() ?? "";
                ws.Cell(fila, 5).Value = $"{item.Apellido.ToUpper()}, {item.Nombre.ToUpper()}";
                ws.Cell(fila, 6).Value = item.Vencimiento?.ToString("dd/MM/yyyy") ?? "";
                fila++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            string fileName = $"TitularesFallecidos_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        // ─── Exportar Excel: Parcelas con múltiples concesiones activas ───────────
        [AuthorizeRole(RolUsuario.Administrador)]
        [HttpGet]
        public async Task<IActionResult> ExportarExcelParcelasConMultiplesConcesiones()
        {
            var datos = await _reporteProblemasService.GetParcelasConMultiplesConcesiones();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("MultiplesConcesiones");

            string[] headers = { "Concesión", "Tipo Parcela", "Sección", "Fila", "Parcela", "Vencimiento" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#2E75B6");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            int fila = 2;
            foreach (var item in datos)
            {
                string tipoParcela = item.TipoParcelaId.HasValue
                    ? EnumHelper.GetDisplayNameByValue<TipoParcelaEnum>(item.TipoParcelaId.Value)
                    : "---";

                ws.Cell(fila, 1).Value = item.Concesion?.ToString("D5") ?? "---";
                ws.Cell(fila, 2).Value = tipoParcela;
                ws.Cell(fila, 3).Value = item.Seccion?.ToUpper() ?? "";
                ws.Cell(fila, 4).Value = item.NroFila?.ToString() ?? "";
                ws.Cell(fila, 5).Value = item.NroParcela?.ToString() ?? "";
                ws.Cell(fila, 6).Value = item.Vencimiento?.ToString("dd/MM/yyyy") ?? "";
                fila++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            string fileName = $"ParcelasMultiplesConcesiones_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        // ─── Exportar Excel: Difuntos en más de una concesión activa ──────────────
        [AuthorizeRole(RolUsuario.Administrador)]
        [HttpGet]
        public async Task<IActionResult> ExportarExcelDifuntosMultiplesConcesiones()
        {
            var datos = await _reporteProblemasService.GetDifuntosEnMultiplesConcesionesActivas();

            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("DifuntosMultiplesConcesiones");

            string[] headers = { "Difunto", "Concesión", "Tipo Parcela", "Sección", "Fila", "Parcela", "Vencimiento" };
            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#2E75B6");
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            }

            int fila = 2;
            foreach (var item in datos)
            {
                string tipoParcela = item.TipoParcelaId.HasValue
                    ? EnumHelper.GetDisplayNameByValue<TipoParcelaEnum>(item.TipoParcelaId.Value)
                    : "---";

                ws.Cell(fila, 1).Value = $"{item.Apellido.ToUpper()}, {item.Nombre.ToUpper()}";
                ws.Cell(fila, 2).Value = item.Concesion?.ToString("D5") ?? "---";
                ws.Cell(fila, 3).Value = tipoParcela;
                ws.Cell(fila, 4).Value = item.Seccion?.ToUpper() ?? "";
                ws.Cell(fila, 5).Value = item.NroFila?.ToString() ?? "";
                ws.Cell(fila, 6).Value = item.NroParcela?.ToString() ?? "";
                ws.Cell(fila, 7).Value = item.Vencimiento?.ToString("dd/MM/yyyy") ?? "";
                fila++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            string fileName = $"DifuntosMultiplesConcesiones_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
    }
}
