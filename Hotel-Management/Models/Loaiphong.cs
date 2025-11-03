using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Loaiphong
{
    public int Maloaiphong { get; set; }

    public string Tenloaiphong { get; set; } = null!;

    public byte? Succhua { get; set; }

    public decimal? Gia { get; set; }

    public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();
}
