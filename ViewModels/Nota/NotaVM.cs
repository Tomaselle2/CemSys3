using CemSys3.DTOs.Nota;
using CemSys3.DTOs.Paginacion;
using CemSys3.DTOs.SweetAlert;

namespace CemSys3.ViewModels.Nota
{
    public class NotaVM
    {
        public IEnumerable<NotaDTO> ListaNotas { get; set; } = new List<NotaDTO>();

        public int tipoNotaId { get; set; }

        public int filtroEstadoId { get; set; }

        //alertas
        public SweetAlertDTO? SweetAlert { get; set; }

        public PaginacionDTO Paginacion { get; set; } = new PaginacionDTO();
    }
}
