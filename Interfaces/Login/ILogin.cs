using CemSys3.DTOs.Login;

namespace CemSys3.Interfaces.Login
{
    public interface ILogin
    {
        Task<LoginResultDTO> Loguearse (LoginDTO datosLogin);
    }
}
