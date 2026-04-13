using CemSys3.DTOs.PlantillaTramite;
using CemSys3.Interfaces.PlantillaTramite;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class DocumentoController : Controller
    {
        private readonly IDocumentoTramiteService _service;

        public DocumentoController(IDocumentoTramiteService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Editar(int id)
        {
            var doc = await _service.ObtenerPorTramiteAsync(id);

            if (doc == null)
                return NotFound();

            return View(doc);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(DocumentoDTO dto)
        {
            await _service.ActualizarAsync(dto);

            return RedirectToAction("Index", "CambioTitular", new { tramiteId = dto.TramiteId });
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
