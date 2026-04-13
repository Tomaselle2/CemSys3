using CemSys3.DTOs.TramitesConcesion;

namespace CemSys3.Interfaces.TramitesConcesion
{
    public interface ICambioTitular
    {
        Task<CambioTitularDTO> AddCambioTitular(int tramiteConcesionId, int usuarioId);

        Task<int> CambioTitularPost(CambioTitularDTO dto); //post

        Task<CambioTitularDTO> Get(int cambioTitularId, int concesionId);

    }
}
