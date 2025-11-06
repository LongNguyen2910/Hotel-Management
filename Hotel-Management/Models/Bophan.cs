using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models;

public partial class Bophan
{
    [Required(ErrorMessage = "Mã bộ phận không được để trống")]
    public string? Mabophan { get; set; }
    [Required(ErrorMessage = "Tên bộ phận không được để trống")]
    public string? Tenbophan { get; set; }
    [Required(ErrorMessage = "Ngày thành lập không được để trống")]
    public DateTime? Ngaythanhlap { get; set; }

    public virtual ICollection<Nhanvien> Nhanviens { get; set; } = new List<Nhanvien>();
}
