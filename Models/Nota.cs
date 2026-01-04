using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class Nota
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public int TipoNotaId { get; set; }

    public string? Descripcion { get; set; }

    public string? Color { get; set; }

    public virtual ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();

    public virtual TipoNotum TipoNota { get; set; } = null!;
}
