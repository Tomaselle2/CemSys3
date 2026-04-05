using CemSys3.DTOs.TramiteConcesion;

namespace CemSys3.Interfaces.TramiteConcesion
{
    public interface ICambioTitular
    {
        Task<CambioTitularDTO> AddCambioTitular(int tramiteConcesionId, int usuarioId); //get

        Task<int> CambioTitular(CambioTitularDTO dto); //post
    }
}
