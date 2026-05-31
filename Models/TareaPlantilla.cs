using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class TareaPlantilla
{
    public int Id { get; set; }

    public string Descripcion { get; set; } = null!;

    public int TipoTramiteId { get; set; }

    public bool Visibilidad { get; set; }

    public bool Estado { get; set; }

    public virtual ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();

    public virtual TipoTramite TipoTramite { get; set; } = null!;
}
