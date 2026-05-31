using CemSys3.DTOs.SweetAlert;
using CemSys3.DTOs.Usuario;
using CemSys3.Enumerables;
using CemSys3.Helpers.Enumerable;
using CemSys3.Helpers.Mensajes;
using CemSys3.Helpers.Roles_Autenticacion;
using CemSys3.Interfaces.Usuario;
using CemSys3.ViewModels.Usuario;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;


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

        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> Editar(int Id) 
        {
            try
            {
                UsuarioRequestDTO usuario = await _usuarioService.GetUserById(Id); //busca el usuario por id

                if (usuario == null)
                {
                    TempData.SetSweetAlert(
                         new SweetAlertDTO
                         {
                             Titulo = "Error",
                             Mensaje = "Usuario no encontrado.",
                             Tipo = "error"
                         }
                    );
                }

                UsuarioAdministradorVM viewModel = new UsuarioAdministradorVM //armo el VM
                {
                    Id = usuario.Id,
                    NombreEmpleado = usuario.NombreEmpleado,
                    ApellidoEmpleado = usuario.ApellidoEmpleado,
                    Correo = usuario.Correo,
                    NombreUsuario = usuario.NombreUsuario,
                    IdRol = usuario.IdRol,
                    Roles = await _usuarioService.ObtenerRoles(),
                    Usuarios = await _usuarioService.ListadoUsuarios()
                };

                return View("AdminUsers", viewModel);
            }
            catch (Exception ex)
            {
                TempData.SetSweetAlert(
                     new SweetAlertDTO
                     {
                         Titulo = "Error",
                         Mensaje = "Ocurrió un error al buscar el usuario: " + ex.Message,
                         Tipo = "error"
                     }
                );
                return RedirectToAction("AdminUsers");
            }
        }

        [HttpGet]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> Eliminar(int Id)
        {
            try
            {
                await _usuarioService.Delete(Id);
                TempData.SetSweetAlert(
                     new SweetAlertDTO
                     {
                         Titulo = "Éxito",
                         Mensaje = "Usuario eliminado correctamente.",
                         Tipo = "success"
                     }
                );
            }
            catch (Exception ex)
            {
                TempData.SetSweetAlert(
                     new SweetAlertDTO
                     {
                         Titulo = "Error",
                         Mensaje = "Ocurrió un error al eliminar el usuario: " + ex.Message,
                         Tipo = "error"
                     }
                );
            }
            return RedirectToAction("AdminUsers");
        }

        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            PerfilUsuarioVM viewModel = new PerfilUsuarioVM();
            viewModel.SweetAlert = TempData.GetSweetAlert();

            try
            {
                UsuarioRequestDTO usuario = await _usuarioService.GetUserById(HttpContext.Session.GetInt32("IdUsuario") ?? 0);

                viewModel.Id = usuario.Id;
                viewModel.Nombre = usuario.NombreEmpleado;
                viewModel.Apellido = usuario.ApellidoEmpleado;
                viewModel.Correo = usuario.Correo;
                viewModel.NombreUsuario = usuario.NombreUsuario;
                viewModel.Rol = EnumHelper.GetDisplayNameByValue<RolUsuario>(usuario.IdRol);
                

                return View("Perfil", viewModel);
            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Ocurrió un error al eliminar el usuario: " + ex.Message,
                    Tipo = "error"
                };
                return View("Perfil", viewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ModificarPerfil(PerfilUsuarioVM viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Complete los campos obligatorios",
                    Tipo = "error"
                };
                return View("Perfil", viewModel);
            }


            try
            {
                UsuarioRequestDTO usuario = await _usuarioService.GetUserById(viewModel.Id ?? 0); //busca el usuario por id

                usuario.NombreEmpleado = viewModel.Nombre ?? "";
                usuario.ApellidoEmpleado = viewModel.Apellido ?? "";
                usuario.Correo = viewModel.Correo ?? "";
                usuario.NombreUsuario = viewModel.NombreUsuario ?? "";

                await _usuarioService.Modificar(usuario);

                TempData.SetSweetAlert(
                    new SweetAlertDTO
                    {
                        Titulo = "Éxito",
                        Mensaje = "Perfil actualizado correctamente.",
                        Tipo = "success"
                    }
                );

                return RedirectToAction("Perfil");

            }
            catch (Exception ex)
            {
                viewModel.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Ocurrió un error al modificar el perfil: " + ex.Message,
                    Tipo = "error"
                };
                return View("Perfil", viewModel);
            }

        }

        [HttpGet]
        public IActionResult CambiarContraseniaUsuario(int usuarioId)
        {

            CambiarContraseniaUsuarioVM viewModel = new CambiarContraseniaUsuarioVM
            {
                Id = usuarioId
            };

            viewModel.SweetAlert = TempData.GetSweetAlert();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CambiarContraseniaUsuario(CambiarContraseniaUsuarioVM model)
        {
            if (!ModelState.IsValid)
            {
                model.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "Complete los campos obligatorios",
                    Tipo = "error"
                };

                return View("CambiarContraseniaUsuario", model);
            }

            try
            {
                await _usuarioService.ModificarContrasenia(model.Id ?? 0, model.ClaveNueva ?? "", model.ClaveAnterior ?? "");

                TempData.SetSweetAlert(
                    new SweetAlertDTO
                    {
                        Titulo = "Éxito",
                        Mensaje = "Contraseña modificada correctamente",
                        Tipo = "success"
                    }
                );
            }
            catch (ValidationException ex)
            {
                model.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Validación",
                    Mensaje = ex.Message,
                    Tipo = "warning"
                };
                return View("CambiarContraseniaUsuario", model);
            }
            catch (Exception ex)
            {
                model.SweetAlert = new SweetAlertDTO
                {
                    Titulo = "Error",
                    Mensaje = "No se pudo cambiar la contraseña: " + ex.Message,
                    Tipo = "warning"
                };
                return View("CambiarContraseniaUsuario", model);
            }

            return RedirectToAction("CambiarContraseniaUsuario", new { usuarioId = model.Id });

        }

        [HttpPost]
        [AuthorizeRole(RolUsuario.Administrador)]
        public async Task<IActionResult> Resetear(int Id)
        {
            try
            {
                await _usuarioService.ResetearContrasenia(Id);

                TempData.SetSweetAlert(
                    new SweetAlertDTO
                    {
                        Titulo = "Éxito",
                        Mensaje = "Contraseña reseteada correctamente",
                        Tipo = "success"
                    }
                );
            }
            catch (Exception ex)
            {
                TempData.SetSweetAlert(
                    new SweetAlertDTO
                    {
                        Titulo = "Error",
                        Mensaje = "No se pudo resetear la contraseña: " + ex.Message,
                        Tipo = "error"
                    }
                );
            }

            return RedirectToAction("AdminUsers");
        }
    }
}
