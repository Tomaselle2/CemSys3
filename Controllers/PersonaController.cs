using CemSys3.DTOs.Persona;
using CemSys3.DTOs.SweetAlert;
using CemSys3.Enumerables;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Persona;
using CemSys3.Models;
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

        // Método para registrar nuevo titular para contrato (AJAX)
        [HttpPost]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> RegistrarContribuyenteContrato([FromBody] RegistrarContribuyenteRequestContrato request)
        {
            try
            {
                if (request.Dni == null || string.IsNullOrEmpty(request.Sexo) ||
                    string.IsNullOrEmpty(request.Nombre) || string.IsNullOrEmpty(request.Apellido) || string.IsNullOrEmpty(request.Domicilio))
                {
                    return Json(new { success = false, message = "Todos los campos son obligatorios" });
                }

                // Validar que no exista ya
                bool contribuyenteExistente = await _personaService.PersonaExiste(request.Dni.Value, request.Sexo);
                if (contribuyenteExistente)
                {
                    return Json(new { success = false, message = "El titular ya existe en el sistema" });
                }

                // Crear nuevo titular
                var nuevoTitular = new PersonaDTO
                {
                    Dni = request.Dni.ToString(),
                    Nombre = request.Nombre.Trim(),
                    Apellido = request.Apellido.Trim(),
                    Sexo = request.Sexo,
                    Celular = request.Celular?.Trim(),
                    Correo = request.Correo?.Trim(),
                    Domicilio = request.Domicilio.Trim(),
                    CategoriaPersonaId = (int)CategoriaPersonaEnum.Titular
                };

                // Guardar en base de datos
                int idTitular = await _personaService.Add(nuevoTitular);
                PersonaDTO TitularCreado = await _personaService.Get(idTitular);

                return Json(new
                {
                    success = true,
                    contribuyente = new
                    {
                        id = TitularCreado.Id,
                        nombre = TitularCreado.Nombre,
                        apellido = TitularCreado.Apellido,
                        dni = TitularCreado.Dni, // Usar el DNI del request
                        sexo = TitularCreado.Sexo,
                        celular = TitularCreado.Celular,
                        correo = TitularCreado.Correo,
                        domicilio = TitularCreado.Domicilio
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        // Método para buscar contribuyente (AJAX)
        [HttpPost]
        [AuthorizeRole(RolUsuario.Empleado)]
        public async Task<IActionResult> BuscarContribuyenteContrato([FromBody] BuscarContribuyenteRequest request)
        {
            try
            {
                if (request.Dni == null || string.IsNullOrEmpty(request.Sexo))
                {
                    return Json(new { success = false, message = "DNI y sexo son obligatorios" });
                }

                PersonaDTO contribuyente = await _personaService.GetByDNISexo(request.Dni.Value, request.Sexo);

                if (contribuyente != null)
                {
                    return Json(new
                    {
                        success = true,
                        contribuyente = new
                        {
                            id = contribuyente.Id,
                            nombre = contribuyente.Nombre,
                            apellido = contribuyente.Apellido,
                            dni = contribuyente.Dni, 
                            sexo = contribuyente.Sexo,
                            celular = contribuyente.Celular,
                            correo = contribuyente.Correo,
                            domicilio = contribuyente.Domicilio,

                        }
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        contribuyente = (object)null,
                        dni = request.Dni, // Devolver el DNI aunque no se encuentre el contribuyente
                        sexo = request.Sexo,

                    });
                }
            }
            catch (Exception ex)
            {
                // Si el mensaje indica que no existe, tratarlo como "no encontrado"
                if (ex.Message.Contains("no encontrada") || ex.Message.Contains("not found"))
                {
                    return Json(new { success = true, contribuyente = (object)null });
                }
                return Json(new { success = false, message = ex.Message });
            }
        }
        // Clase para los requests AJAX
        public class RegistrarContribuyenteRequestContrato
        {
            public int? Dni { get; set; }
            public string Sexo { get; set; }
            public string Nombre { get; set; }
            public string Apellido { get; set; }
            public string Domicilio { get; set; }
            public string? Celular { get; set; }
            public string? Correo { get; set; }
        }

        // Clase para los requests AJAX
        public class BuscarContribuyenteRequest
        {
            public int? Dni { get; set; }
            public string Sexo { get; set; }
        }
    }
}
