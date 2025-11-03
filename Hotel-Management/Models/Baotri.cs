using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Baotri
{
    public DateTime Ngaybatdau { get; set; }

    public DateTime Ngayketthuc { get; set; }

    public bool? Trangthai { get; set; }

    public decimal? Tienbaotri { get; set; }

    public virtual ICollection<Thietbiduocbaotri> Thietbiduocbaotris { get; set; } = new List<Thietbiduocbaotri>();
}
