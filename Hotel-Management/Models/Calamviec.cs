using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Calamviec
{
    public string Macalamviec { get; set; } = null!;

    public DateTime Thoigianbatdau { get; set; }

    public DateTime Thoigianketthuc { get; set; }

    public virtual ICollection<Nhanvienlamca> Nhanvienlamcas { get; set; } = new List<Nhanvienlamca>();
}
