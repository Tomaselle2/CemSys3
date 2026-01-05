using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.Usuario;
using CemSys3.Enumerables;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Usuario;
using CemSys3.ViewModels.Usuario;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Text.Json;
using CemSys3.Helpers.Mensajes;


namespace CemSys3.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuario _usuarioService;
        public UsuarioController(IUsuario usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> AdminUsers()
        {
            UsuarioAdministradorVM viewModel = new UsuarioAdministradorVM();

            viewModel.SweetAlert = TempData.GetSweetAlert();

            try
            {
                viewModel.Roles = await _usuarioService.ObtenerRoles();
                viewModel.Usuarios = await _usuarioService.ListadoUsuarios();
            }
            catch (Exception ex) {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Ocurrió un error al cargar los roles/usuarios: " + ex.Message,
                    Tipo = "error"
                };
            }

            return View(viewModel);
        }

        [HttpPost]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> Guardar(UsuarioAdministradorVM viewModel) //sirve para modificar y registrar
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            UsuarioRequestDTO usuario = new UsuarioRequestDTO
            {
                Id = viewModel.Id,
                NombreEmpleado = viewModel.NombreEmpleado,
                ApellidoEmpleado = viewModel.ApellidoEmpleado,
                Correo = viewModel.Correo,
                NombreUsuario = viewModel.NombreUsuario,
                IdRol = viewModel.IdRol.Value
            };

            try
            {
                if (viewModel.Id.HasValue && viewModel.Id.Value > 0) //modifica
                {
                    await _usuarioService.Modificar(usuario);
                    TempData.SetSweetAlert(
                        new SweetAlertDTO
                        {
                            Titulo = "Éxito",
                            Mensaje = "Usuario actualizado correctamente.",
                            Tipo = "success"
                        }
                    );
                }
                else
                {
                    await _usuarioService.Registrar(usuario); //registra
                    TempData.SetSweetAlert(
                        new SweetAlertDTO
                        {
                            Titulo = "Éxito",
                            Mensaje = "Usuario registrado correctamente.",
                            Tipo = "success"
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                TempData.SetSweetAlert(
                     new SweetAlertDTO 
                     {
                            Titulo = "Error",
                            Mensaje = "Ocurrió un error al guardar el usuario: " + ex.Message,
                            Tipo = "error"
                     }
                );
            }

            return RedirectToAction("AdminUsers");
        }
    }
}
