using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Phong
{
    public string Maphong { get; set; } = null!;

    public string? Tenphong { get; set; }

    public bool Tinhtrang { get; set; }

    public string? Mota { get; set; }

    public int? Maloaiphong { get; set; }

    public string? Anhphong { get; set; }

    public virtual ICollection<Khachhangdatphong> Khachhangdatphongs { get; set; } = new List<Khachhangdatphong>();

    public virtual Loaiphong? MaloaiphongNavigation { get; set; }

    public virtual ICollection<Thietbi> Mathietbis { get; set; } = new List<Thietbi>();
}
