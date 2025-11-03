using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Nhanvienlamca
{
    public string Manv { get; set; } = null!;

    public string Macalamviec { get; set; } = null!;

    public DateTime Ngaylam { get; set; }

    public virtual Calamviec MacalamviecNavigation { get; set; } = null!;

    public virtual Nhanvien ManvNavigation { get; set; } = null!;
}
