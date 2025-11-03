using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Thucdon
{
    public int Mathucdon { get; set; }

    public DateTime Ngayapdung { get; set; }

    public DateTime Ngaytao { get; set; }

    public virtual ICollection<Mon> Mamons { get; set; } = new List<Mon>();
}
