using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Nhanvien
{
    public string Manv { get; set; } = null!;

    public string? Hoten { get; set; }

    public string? Gioitinh { get; set; }

    public DateTime? Ngaysinh { get; set; }

    public string? Sodienthoai { get; set; }

    public string? Cccd { get; set; }

    public DateTime? Ngayvaolam { get; set; }

    public bool? Trangthai { get; set; }

    public string? Mabophan { get; set; }

    public string? Tenchucvu { get; set; }

    public virtual ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();

    public virtual Bophan? MabophanNavigation { get; set; }

    public virtual ICollection<Nhanvienlamca> Nhanvienlamcas { get; set; } = new List<Nhanvienlamca>();

    public virtual Chucvu? TenchucvuNavigation { get; set; }
}
