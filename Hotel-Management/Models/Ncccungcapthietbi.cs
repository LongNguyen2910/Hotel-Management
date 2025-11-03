using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Ncccungcapthietbi
{
    public string Mancc { get; set; } = null!;

    public int Mathietbi { get; set; }

    public int? Soluong { get; set; }

    public decimal? Tienthietbi { get; set; }

    public virtual Nhacungcap ManccNavigation { get; set; } = null!;

    public virtual Thietbi MathietbiNavigation { get; set; } = null!;
}
