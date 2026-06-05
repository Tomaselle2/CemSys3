using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.Generics;
using CemSys3.DTOs.Paginacion;

namespace CemSys3.Interfaces.Concesion
{
    public interface IConcesion
    {
        Task<GenericResultDTO> Add(ConcesionDTO dto);
        Task<GenericResultDTO> Update(ConcesionDTO dto);

        Task<PaginadoResponse<TablaConcesionDTO>> GellAllPaginado(
            int filtroEstado = 0,
            int pagina = 1,
            int porPagina = 10,
            string nombre = "",
            string apellido = "",
            int concesion = 0,
            int? tipoParcelaID = null,
            int? seccionID = null, 
            int? parcelaID = null);

        public Task<GenerarContratoDTO> SolicitarDatosParaGenerarContrato(int idTramite);

        public Task<InfoGeneralDTO> InfoGeneral(int idTramite);
        public Task<InfoGeneralDTO> InfoGeneralMinima(int idTramite);


        public Task<bool> ExisteNroConcesion(int nroConcesion);

        public Task<ModificarDatosConcesionDTO> ModificarDatosConecesion(int tramiteId);
        public Task ModificarDatosConecesion(ModificarDatosConcesionDTO dto);

    }
}
