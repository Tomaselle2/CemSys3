using CemSys3.Interfaces.Backup;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class BackupController : Controller
    {
        private readonly IBackupService _backupService;

        public BackupController(IBackupService backupService)
        {
            _backupService = backupService;
        }

        // GET: /Backup
        public async Task<IActionResult> Index()
        {
            var ejecuciones = await _backupService.ObtenerUltimasEjecucionesAsync(20);
            return View(ejecuciones);
        }

        // POST: /Backup/EjecutarManual
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EjecutarManual()
        {
            var (exito, mensaje) = await _backupService.EjecutarBackupManualAsync();

            TempData[exito ? "Exito" : "Error"] = mensaje;

            return RedirectToAction(nameof(Index));
        }
    }
}
