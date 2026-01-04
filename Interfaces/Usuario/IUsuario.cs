namespace CemSys3.Interfaces.Usuario
{
    public interface IUsuario
    {
        string HashPassword(string password); //encripta la contraseña para usuarios nuevos
        bool VerificarPassword(string plainPassword, string storedHash); //verifica la contraseña ingresada con la almacenada

    }
}
