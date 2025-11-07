using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Khachhangdatmon
{
    public int Makhachhang { get; set; }

    public string Mamon { get; set; } = null!;

    public DateTime Ngaydat { get; set; }

    public int? Soluong { get; set; }

    public virtual Khachhang MakhachhangNavigation { get; set; } = null!;

    public virtual Mon MamonNavigation { get; set; } = null!;
}
