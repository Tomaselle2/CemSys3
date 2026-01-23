using CemSys3.DTOs.Seccion;

namespace CemSys3.Interfaces.Seccion
{
    public interface ISeccionNichoTarifaria
    {
        Task<IEnumerable<SeccionNichoTarifariaDTO>> GetAllSeccionesNichosParaTarifaria();
    }
}
