using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Khachhangdatphong
{
    public int Makhachhang { get; set; }

    public string Maphong { get; set; } = null!;

    public DateTime? Ngaydat { get; set; }

    public DateTime? Ngaycheckin { get; set; }

    public DateTime? Ngaycheckout { get; set; }

    public virtual Khachhang MakhachhangNavigation { get; set; } = null!;

    public virtual Phong MaphongNavigation { get; set; } = null!;
}
