using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models;

public partial class Mon
{
    [Required(ErrorMessage = "Mã món không được để trống")]
    public string? Mamon { get; set; } = null!;
    [Required(ErrorMessage = "Tên không được để trống")]
    public string? Tenmon { get; set; }
    [Required(ErrorMessage = "Giá không được để trống")]
    public decimal? Gia { get; set; }

    public string? Anhmon { get; set; }

    public virtual ICollection<Khachhangdatmon> Khachhangdatmons { get; set; } = new List<Khachhangdatmon>();

    public virtual ICollection<Thucdon> Thucdons { get; set; } = new List<Thucdon>();
}
