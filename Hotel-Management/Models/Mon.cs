using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Mon
{
    public string Mamon { get; set; } = null!;

    public string? Tenmon { get; set; }

    public decimal? Gia { get; set; }
    public string? Anhmon { get; set; }

    public virtual ICollection<Khachhangdatmon> Khachhangdatmons { get; set; } = new List<Khachhangdatmon>();

    public virtual ICollection<Thucdon> Thucdons { get; set; } = new List<Thucdon>();
}
