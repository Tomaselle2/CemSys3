using CemSys3.DTOs.Arbol;
using CemSys3.Interfaces;
using CemSys3.Models;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class ArbolController : Controller
    {
        private readonly IArbol _arbolService;

        public ArbolController(IArbol arbolService)
        {
            _arbolService = arbolService;
        }

        [HttpPost]
        public async Task<IActionResult> GuardarDiagrama([FromBody] ArbolDTO dto)
        {
            try
            {
                await _arbolService.GuardarDiagrama(dto);
                return Ok();
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        public async Task<IActionResult> ObtenerDiagrama(int tramiteId)
        {
           
                var diagrama = await _arbolService.ObtenerDiagrama(tramiteId);
                return Json(diagrama);
            
        }
    }
}
