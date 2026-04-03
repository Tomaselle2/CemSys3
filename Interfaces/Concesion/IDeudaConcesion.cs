namespace CemSys3.Interfaces.Concesion
{
    public interface IDeudaConcesion
    {
        Task<string> CalculoDeudaConcesion(int tramiteId);
    }
}
