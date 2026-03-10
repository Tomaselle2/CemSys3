using CemSys3.DTOs.SweetAlert;
using CemSys3.Helpers.Mensajes;
using CemSys3.Interfaces.Persona;
using CemSys3.ViewModels.Persona;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class PersonaController : Controller
    {
        private readonly IPersona _personaService;

        public PersonaController(IPersona persona)
        {
            _personaService = persona;
        }

        public async Task<IActionResult> HistorialPersona(int id)
        {
            HistorialPersonaVM viewModel = new HistorialPersonaVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();

            try
            {
                viewModel.persona = await _personaService.Get(id);
            }
            catch (Exception ex) {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = $"No se pudo obtener la información de la persona. Detalles: {ex.Message}",
                    Tipo = "error"
                };
            }

            return View(viewModel);
        }
    }
}
