using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Bophan
{
    public string Mabophan { get; set; } = null!;

    public string Tenbophan { get; set; } = null!;

    public DateTime? Ngaythanhlap { get; set; }

    public virtual ICollection<Nhanvien> Nhanviens { get; set; } = new List<Nhanvien>();
}
