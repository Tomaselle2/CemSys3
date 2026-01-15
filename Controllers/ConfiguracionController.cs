using CemSys3.Enumerables;
using CemSys3.Helpers.Roles_Autenticacion;
using Microsoft.AspNetCore.Mvc;

namespace CemSys3.Controllers
{
    public class ConfiguracionController : Controller
    {
        [AuthorizeRole(RolUsuario.Administrador)]

        public IActionResult Index()
        {
            return View();
        }
    }
}
