using CemSys3.DTOs.PlantillaTramite;
using CemSys3.Interfaces.PlantillaTramite;

namespace CemSys3.Business.PlantillaTramite
{
    public class AutorizacionFactory : IAutorizacionFactory
    {
        public Task Crear(PlantillaTramiteDTO plantilla)
        {
            throw new NotImplementedException();
        }

        public Task CrearMultiples(IEnumerable<PlantillaTramiteDTO> plantilla)
        {
            throw new NotImplementedException();
        }
    }
}
