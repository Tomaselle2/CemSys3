using CemSys3.DTOs.Login;
using CemSys3.DTOs.SweetAlert;
using CemSys3.Interfaces.Login;
using CemSys3.ViewModels.Login;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CemSys3.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILogin _loginService;

        public LoginController(ILogin login)
        {
            _loginService = login;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginVM viewModel = new LoginVM();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            //mapea el VM al DTO
            LoginDTO loginDTO = new LoginDTO
            {
                Usuario = viewModel.NombreUsuario,
                Clave = viewModel.Clave
            };

            LoginResultDTO result = await _loginService.Loguearse(loginDTO);

            if (result.Success) //inicio de sesion exitoso
            {
                HttpContext.Session.SetString("NombreUsuario", $"{result.NombreEmpleado} {result.ApellidoEmpleado}");
                HttpContext.Session.SetInt32("IdUsuario", result.UsuarioId.Value);
                HttpContext.Session.SetInt32("IdRol", result.RolId.Value);

                return RedirectToAction("Index", "Home"); //ridirige al home
            }

            viewModel.sweetAlertDTO = new SweetAlertDTO //en caso de error
            {
                Titulo = "Error de acceso",
                Mensaje = result.ErrorMessage,
                Tipo = "error" //tipo de alerta
            };

            return View(viewModel);
        }


    }
}
