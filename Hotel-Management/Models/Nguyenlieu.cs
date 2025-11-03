using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Nguyenlieu
{
    public string Manguyenlieu { get; set; } = null!;

    public string Tennguyenlieu { get; set; } = null!;

    public DateTime? Ngaynhap { get; set; }

    public DateTime? Hansudung { get; set; }

    public DateTime? Ngaysanxuat { get; set; }

    public virtual ICollection<Ncccungcapnguyenlieu> Ncccungcapnguyenlieus { get; set; } = new List<Ncccungcapnguyenlieu>();
}
