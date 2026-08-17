using CemSys3.Enumerables;
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
    }
}
