using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Hoadon
{
    public string Mahoadon { get; set; } = null!;

    public DateTime? Ngaylap { get; set; }

    public DateTime? Ngaythanhtoan { get; set; }

    public decimal? Giaphong { get; set; }

    public decimal? Giamon { get; set; }

    public int? Makhachhang { get; set; }

    public string? Manv { get; set; } = null!;

    public virtual Khachhang? MakhachhangNavigation { get; set; }

    public virtual Nhanvien ManvNavigation { get; set; } = null!;
}
