using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Usuario1 { get; set; } = null!;

    public string Clave { get; set; } = null!;

    public bool Visibilidad { get; set; }

    public int RolId { get; set; }

    public virtual ICollection<Concesione> Concesiones { get; set; } = new List<Concesione>();

    public virtual ICollection<Introduccione> Introducciones { get; set; } = new List<Introduccione>();

    public virtual RolesUsuario Rol { get; set; } = null!;

    public virtual ICollection<Tramite> Tramites { get; set; } = new List<Tramite>();
}
