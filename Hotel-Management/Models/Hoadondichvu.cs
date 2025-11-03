using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Hoadondichvu
{
    public string Mahoadon { get; set; } = null!;

    public string Madichvu { get; set; } = null!;

    public DateTime Ngaydat { get; set; }

    public byte? Soluong { get; set; }

    public virtual Dichvu MadichvuNavigation { get; set; } = null!;

    public virtual Hoadon MahoadonNavigation { get; set; } = null!;
}
