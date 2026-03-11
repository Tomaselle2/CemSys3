using CemSys3.DTOs.Persona;
using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
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

        [HttpGet]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> HistorialPersona(int id)
        {
            HistorialPersonaVM viewModel = new HistorialPersonaVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();

            try
            {
                viewModel.persona = await _personaService.HistorialPersona(id);
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

        [HttpPost]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> Guardar(HistorialPersonaVM viewModel)
        {
            try
            {
                await _personaService.Update(viewModel.persona.Persona);
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Éxito",
                    Mensaje = "Se ha actualizado correctamente.",
                    Tipo = "success"
                });
            }
            catch (Exception ex) {
                TempData.SetSweetAlert(new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Problema al actualizar los datos. " + ex.Message,
                    Tipo = "error"
                });
            }

            return RedirectToAction("HistorialPersona", new { id = viewModel.persona.Persona.Id });
        }
    }
}
