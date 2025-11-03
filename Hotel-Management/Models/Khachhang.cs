using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Khachhang
{
    public int Makhachhang { get; set; }

    public string? Hoten { get; set; }

    public string? Quoctich { get; set; }

    public string? Cccd { get; set; }

    public string? Sdt { get; set; }

    public string? Hochieu { get; set; }

    public virtual ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();

    public virtual ICollection<Khachhangdatmon> Khachhangdatmons { get; set; } = new List<Khachhangdatmon>();

    public virtual ICollection<Khachhangdatphong> Khachhangdatphongs { get; set; } = new List<Khachhangdatphong>();
}
