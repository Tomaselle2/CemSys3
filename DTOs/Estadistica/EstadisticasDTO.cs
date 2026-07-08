namespace CemSys3.DTOs.Estadistica
{
    public class EstadisticasDTO
    {
        public int DifuntosActuales { get; set; }

        public int ConcesionesVigentes { get; set; }
        public int ConcesionesVencidas { get; set; }
        public int ConcesionesCaducadas { get; set; }
        public int ConcesionesSinContrato { get; set; }

        public int NichosOcupados { get; set; }
        public int NichosDesocupados { get; set; }

        public int FosasOcupadas { get; set; }
        public int FosasDesocupadas { get; set; }
        public int PanteonesOcupados { get; set; }
        public int PanteonesDesocupados { get; set; }
    }
}
