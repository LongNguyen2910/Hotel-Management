using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Dichvu
{
    public string Madichvu { get; set; } = null!;

    public string? Tendichvu { get; set; }

    public decimal? Gia { get; set; }

    public bool? Trangthai { get; set; }

    public virtual ICollection<Hoadondichvu> Hoadondichvus { get; set; } = new List<Hoadondichvu>();
}
