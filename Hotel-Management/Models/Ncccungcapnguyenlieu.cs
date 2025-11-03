using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Ncccungcapnguyenlieu
{
    public string Mancc { get; set; } = null!;

    public string Manguyenlieu { get; set; } = null!;

    public int? Luong { get; set; }

    public decimal? Tiennguyenlieu { get; set; }

    public virtual Nhacungcap ManccNavigation { get; set; } = null!;

    public virtual Nguyenlieu ManguyenlieuNavigation { get; set; } = null!;
}
