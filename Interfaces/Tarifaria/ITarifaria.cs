using CemSys3.DTOs.Tarifaria;
using static CemSys3.Business.Tarifaria.TarifariaService;

namespace CemSys3.Interfaces.Tarifaria
{
    public interface ITarifaria
    {
        Task<IEnumerable<TarifariaRequestDTO>> GetPrecios(); //trae los precios sin paginar de todo
        Task<List<NuevoIdDTO>> ActualizarPreciosTarifaria(List<PrecioActualizarDTO> preciosActualizar); //para actualizar todos los precios
        Task AplicarAumentoPorcentual(decimal porcentaje, int decimales);
        Task<PdfPreciosNichosDTO> ObtenerDatosParaPdfNichosFosasAsync();
    }
}
