using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Loaithietbi
{
    public string Maloaithietbi { get; set; } = null!;

    public string? Tenloaithietbi { get; set; }

    public virtual ICollection<Thietbi> Thietbis { get; set; } = new List<Thietbi>();
}
