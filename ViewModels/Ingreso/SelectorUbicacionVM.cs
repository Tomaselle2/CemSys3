namespace CemSys3.ViewModels.Ingreso
{
    public class SelectorUbicacionVM
    {
        public int? TipoParcelaID { get; set; }
        public int? SeccionID { get; set; }
        public int? ParcelaID { get; set; }

        // nombres reales del VM padre
        public string TipoParcelaName { get; set; }
        public string SeccionName { get; set; }
        public string ParcelaName { get; set; }

        public string UrlSecciones { get; set; }
        public string UrlParcelas { get; set; }
    }
}
