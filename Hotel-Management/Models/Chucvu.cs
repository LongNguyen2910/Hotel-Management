using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Chucvu
{
    public string Tenchucvu { get; set; } = null!;

    public decimal? Luongcoban { get; set; }

    public virtual ICollection<Nhanvien> Nhanviens { get; set; } = new List<Nhanvien>();
}
