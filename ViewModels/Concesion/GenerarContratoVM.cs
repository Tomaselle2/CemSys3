using CemSys3.DTOs.Concesion;
using CemSys3.DTOs.SweetAlert;
using System.ComponentModel.DataAnnotations;

namespace CemSys3.ViewModels.Concesion
{
    public class GenerarContratoVM
    {
        [Required(ErrorMessage = "El vencimiento es obligatorio")]
        public DateOnly? Vencimiento { get; set; }

        [Required(ErrorMessage = "La forma de pago es obligatoria")]
        public string? FormaDePago { get; set; }

        public int? CantidadCuotaSeleccionada { get; set; }
        public int? CantidadAniosId { get; set; }  // Nueva propiedad para el ID de años

        public string ParcelaString { get; set; } = string.Empty;

        public decimal PrecioFinal { get; set; }

        public string? otraFormaPago { get; set; }

        [Required(ErrorMessage = "La cantidad de años es obligatoria")]
        public int? PrecioSeleccionado { get; set; }
        public GenerarContratoDTO contrato { get; set; } = new GenerarContratoDTO();

        //manero de errores
        public SweetAlertDTO? SweetAlert { get; set; }


        //Método para calcular el precio de 1 año
        public void CalcularPrecioNichoUnAnio()
        {
            if (contrato.TipoParcela != "Nicho")
                return;

            var precio1Anio = contrato.PreciosNichos
                .FirstOrDefault(p => p.AniosConcesionId == 1);

            var precio5Anios = contrato.PreciosNichos
                .FirstOrDefault(p => p.AniosConcesionId == 2);

            if (precio1Anio != null && precio5Anios != null)
            {
                var baseAnual = precio5Anios.Precio / 5m;
                precio1Anio.Precio = baseAnual * 1.5m;
            }
        }

    }
}
