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

    public enum TipoTramiteEnum
    {
        [Display(Name = "Ingreso")]
        Ingreso = 1,

        [Display(Name = "Autorización para cremación")]
        Cremacion = 2,

        [Display(Name = "Autorización para reducción")]
        Reduccion = 3,

        [Display(Name = "Contrato de concesión")]
        ContratoConcesion = 4,

        [Display(Name = "Autorización para traslado")]
        Traslado = 5,

        [Display(Name = "Cambio de titularidad")]
        CambioTitular = 6,

        [Display(Name = "Nota")]
        Nota = 7,
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

    //Categoria Persona
    public enum CategoriaPersonaEnum
    {
        [Display(Name = "Titular")]
        Titular = 1,

        [Display(Name = "Fallecido")]
        Fallecido = 2,
    }

    public enum EstadoDifuntoEnum
    {
        [Display(Name = "Cuerpo completo")]
        CuerpoCompleto = 1,

        [Display(Name = "Reducido")]
        Reducido = 2,

        [Display(Name = "Cremado")]
        Cremado = 3,
    }


    //para filtro de parcelas ocupadas o desocupadas
    public enum OcupacionParcelaEnum
    {
        [Display(Name = "Todos")]
        Todas = 0,

        [Display(Name = "Ocupados")]
        Ocupado = 1,

        [Display(Name = "Disponibles")]
        Desocupado = 2,
    }

    //para filtro de estados de ingresos
    public enum FiltroEstadosIngresosEnum
    {
        [Display(Name = "Todos")]
        Todas = 0,

        [Display(Name = "Registrados")]
        Registrados = 1,

        [Display(Name = "Finalizados")]
        Finalizados = 2,
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

    //Estados de los tramites de ingreso
    public enum EstadosIngresoEnum
    {
        [Display(Name = "Registrado")]
        IngresoRegistrado = 1,

        [Display(Name = "Finalizado")]
        IngresoFinalizado = 2,
    }

    //Estados de notas
    public enum EstadosNotaEnum
    {
        [Display(Name = "Pendiente")]
        NotaPendiente = 3,

        [Display(Name = "Finalizado")]
        NotaFinalizado = 4,
    }

    //estados de tramites de concesion
    public enum EstadosConcesionEnum
    {
        [Display(Name = "Sin contrato")]
        SinContrato = 5,

        [Display(Name = "Vigente")]
        Vigente = 6,

        [Display(Name = "Vencido")]
        Vencido = 7,

        [Display(Name = "Caducado")]
        Caducado = 8,
    }

    public enum EstadosTramiteEnum
    {
        [Display(Name = "Registrado")]
        IngresoRegistrado = 1,

        [Display(Name = "Finalizado")]
        IngresoFinalizado = 2,

        [Display(Name = "Sin contrato")]
        SinContrato = 5,

        [Display(Name = "Vigente")]
        Vigente = 6,

        [Display(Name = "Vencido")]
        Vencido = 7,

        [Display(Name = "Caducado")]
        Caducado = 8,
    }

    //concepto de la tarifaria 
    public enum ConceptosTarifariaEnum
    {
        AperturaNichoConPlaca = 1,
        AperturaNichoSinPlaca = 2,
        AperturaFosa = 3,
        PorcentajeFondoAyudaCentroSalud = 18,
        MontoMinimoFondo = 19,
        PorcentajeAumentoInhumacionOtrasLocalidades = 20,
        PorcentajeAumentoConcesionesOtrasLocalidades = 21,
        PorcentajeAumentoIntroduccionDerechoOficinaOtrasLocalidades = 22,
        PorcentajePreciosNichosUrnariosConcesionSecc16_18 = 23,
        PorcentajeIntroduccionUrnaDerechoOficina = 24
    }

    public enum CategoriaArchivosEnum //cada categoria que se agrega debe ser al final siempre
    {
        [Display(Name = "Contrato de Concesión")]
        Contrato_Concesion,

        [Display(Name = "Recibo")]
        Recibo,

        [Display(Name = "Documento de Identidad")]
        DNI,

        [Display(Name = "Acta")]
        Acta,

        [Display(Name = "Libreta de Familia")]
        Libreta_Familia,

        [Display(Name = "Decreto Municipal")]
        Decreto,

        [Display(Name = "Tarifaria")]
        Tarifaria,

        [Display(Name = "Documentación CemSys")]
        Documentacion,

        [Display(Name = "Otro tipo de archivo")]
        Otro
    }

    public class Enumerables
    {
    }
}
