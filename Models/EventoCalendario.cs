using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class EventoCalendario
{
    public int Id { get; set; }

    public string Titulo { get; set; } = null!;

    public DateTime Fecha { get; set; }
}
