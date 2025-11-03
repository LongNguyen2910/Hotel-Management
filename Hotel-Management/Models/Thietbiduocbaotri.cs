using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Thietbiduocbaotri
{
    public DateTime Ngaybatdau { get; set; }

    public DateTime Ngayketthuc { get; set; }

    public int Mathietbi { get; set; }

    public virtual Baotri Baotri { get; set; } = null!;
}
