using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.Parcela;
using CemSys3.DTOs.SweetAlert;

namespace CemSys3.ViewModels.Parcela
{
    public class ParcelaIndexVM
    {
        public IEnumerable<ParcelaIndexRequestDTO> ListadoParcelas { get; set; } = new List<ParcelaIndexRequestDTO>();

        public string NombreSeccion { get; set; } = string.Empty;
        public int TipoParcelaId { get; set; }

        //alertas
        public SweetAlertDTO? SweetAlert { get; set; }

        public PaginacionDTO Paginacion { get; set; } = new PaginacionDTO();
    }
}
