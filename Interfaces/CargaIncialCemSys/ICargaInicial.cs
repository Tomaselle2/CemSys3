using CemSys3.DTOs.CargaInicial;

namespace CemSys3.Interfaces.CargaIncialCemSys
{
    public interface ICargaInicial
    {
        /// <summary>
        /// Procesa el csv de carga inicial fila por fila, agrupando por concesión.
        /// Si el servicio se instancia en modo prueba, NO persiste nada en la base
        /// (cada grupo se procesa dentro de una transacción que siempre se revierte),
        /// pero sí devuelve los archivos de éxito/error para poder iterar.
        /// </summary>
        Task<CargaInicialResumenDTO> CargaInicial(IFormFile excel);
    }
}
