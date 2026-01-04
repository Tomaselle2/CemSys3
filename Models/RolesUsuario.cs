using System;
using System.Collections.Generic;

namespace CemSys3.Models;

public partial class RolesUsuario
{
    public int Id { get; set; }

    public string Rol { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
