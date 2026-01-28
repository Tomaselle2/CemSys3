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

    public enum TipoNotaEnum
    {
        [Display(Name = "Todos")]
        Todos = 0,

        [Display(Name = "Ingreso")]
        Ingreso = 1,

        [Display(Name = "Recordatorio")]
        Recordatorio = 2,

        
    }
    //para filtro de parcelas ocupadas o desocupadas
    public enum OcupacionParcelaEnum
    {
        [Display(Name = "Todos")]
        Todas = 0,

        [Display(Name = "Ocupados")]
        Ocupado = 1,

        [Display(Name = "Desocupdos")]
        Desocupado = 2,
    }

    public enum TemaTarifariaEnum
    {
        [Display(Name = "General")]
        General = 1,

        [Display(Name = "Inhumación")]
        Inhumacion = 2,

        [Display(Name = "Concesión nicho")]
        ConcesionNicho = 3,

        [Display(Name = "Concesión fosa")]
        ConcesionFosa = 4,

        [Display(Name = "Registro civil")]
        RegistroCivil = 5,

        [Display(Name = "Derecho de oficina")]
        DerechoDeOficina = 6,

        [Display(Name = "Fondo de ayuda")]
        Fondo = 7,
    }

    public enum AniosConcesionEnum
    {
        [Display(Name = "1 año")]
        anio1 = 1,

        [Display(Name = "5 años")]
        anio5 = 2,

        [Display(Name = "10 años")]
        anio10 = 3,

        [Display(Name = "15 años")]
        anio15 = 4,

        [Display(Name = "25 años")]
        anio25 = 5
    }

    //Estados de los tramites y notas
    public enum EstadosEnum
    {
        [Display(Name = "Registrado")]
        IngresoRegistrado = 1,

        [Display(Name = "Finalizado")]
        IngresoFinalizado = 2,

        [Display(Name = "Pendiente")]
        NotaPendiente = 3,

        [Display(Name = "Finalizado")]
        NotaFinalizado = 4,
    }

    public class Enumerables
    {
    }
}
