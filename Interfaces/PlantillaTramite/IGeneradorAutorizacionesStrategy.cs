using CemSys3.DTOs.Persona;

namespace CemSys3.Interfaces.PlantillaTramite
{
    public interface IGeneradorAutorizacionesStrategy
    {
        Task GenerarAsync(int tramiteId, List<TitularesContratoDTO> titularesActuales, List<TitularesContratoDTO> nuevosTitulares, int usuarioId, string parentesco);
    }
}
