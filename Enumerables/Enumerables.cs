using System.ComponentModel.DataAnnotations;

namespace CemSys3.Enumerables
{
    public enum RolUsuario
    {
        Empleado = 1,
        Administrador = 2,
    }

    public enum TipoNumeracionParcelaEnum
    {
        [Display(Name = "Nueva (nichos repetidos)")]
        Nueva = 1,

        [Display(Name = "Antigua (sin repetir)")]
        Antigua = 2,
    }

    public enum  TipoNichoEnum
    {
        [Display(Name = "Féretro")]
        Feretro = 1,

        [Display(Name = "Urnario")]
        Urnario = 2,

        [Display(Name = "Especial")]
        Especial = 3,
    }

    public enum TipoPanteonEnum
    {
        [Display(Name = "Con nichos")]
        ConNichos = 1,

        [Display(Name = "Sin nichos")]
        SinNichos = 2,
    }

    public enum TipoParcelaEnum
    {
        [Display(Name = "Nicho")]
        Nicho = 1,

        [Display(Name = "Fosa")]
        Fosa = 2,

        [Display(Name = "Panteón")]
        Panteon = 3,
    }

    public class Enumerables
    {
    }
}
