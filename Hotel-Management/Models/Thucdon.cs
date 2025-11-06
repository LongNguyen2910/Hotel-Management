using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models;

public partial class Thucdon
{
    [Required(ErrorMessage = "Mã thực đơn không được đê trống")]
    public int? Mathucdon { get; set; }
    [Required(ErrorMessage = "Ngày áp dụng không được đê trống")]
    public DateTime? Ngayapdung { get; set; }
    [Required(ErrorMessage = "Ngày tạo không được đê trống")]
    public DateTime? Ngaytao { get; set; }
    public virtual ICollection<Mon> Mamons { get; set; } = new List<Mon>();
}
