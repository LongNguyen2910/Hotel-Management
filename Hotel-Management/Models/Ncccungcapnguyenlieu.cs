using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel_Management.Models;

public partial class Ncccungcapnguyenlieu
{
    [Key, Column(Order = 0)]

    public string Mancc { get; set; } = null!;
    [Key, Column(Order = 1)]

    public string Manguyenlieu { get; set; } = null!;

    public string Tennguyenlieu { get; set; } = null!;

    public DateTime? Ngaynhap { get; set; }

    public DateTime? Hansudung { get; set; }

    public DateTime? Ngaysanxuat { get; set; }

    public int? Luong { get; set; }

    public decimal? Tiennguyenlieu { get; set; }
    [ValidateNever]
    public virtual Nhacungcap ManccNavigation { get; set; } = null!;
}
