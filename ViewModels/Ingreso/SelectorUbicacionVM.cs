using System.ComponentModel.DataAnnotations;

namespace CemSys3.ViewModels.Ingreso
{
    public class SelectorUbicacionVM
    {
        [Required(ErrorMessage = "El tipo de parcela es obligatorio")]
        public int? TipoParcelaID { get; set; }

        [Required(ErrorMessage = "La sección es obligatoria")]
        public int? SeccionID { get; set; }

        [Required(ErrorMessage = "La parcela es obligatoria")]
        public int? ParcelaID { get; set; }

        // nombres reales del VM padre
        public string TipoParcelaName { get; set; }
        public string SeccionName { get; set; }
        public string ParcelaName { get; set; }

        public string UrlSecciones { get; set; }
        public string UrlParcelas { get; set; }
    }
}
