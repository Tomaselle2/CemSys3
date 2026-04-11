namespace CemSys3.Interfaces.PlantillaTramite
{
    public interface IEstrategiaTramite
    {
        // Retorna qué tipos de autorización son requeridos para este trámite
        //IEnumerable<TipoAutorizacion> ObtenerTiposRequeridos(int tipoTramiteId);
        string TipoTramiteNombre { get; }
    }
}
