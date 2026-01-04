using CemSys3.Interfaces.Usuario;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuario _usuarioService;
        public UsuarioController(IUsuario usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
