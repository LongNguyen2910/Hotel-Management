using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models;

public partial class Bophan
{
    [Display(Name = "Mã bộ phận")]
    public string Mabophan { get; set; } = null!;
    [Display(Name = "Tên bộ phận")]
    public string Tenbophan { get; set; } = null!;
    [Display(Name = "Ngày thành lập")]
    public DateTime? Ngaythanhlap { get; set; }

    public virtual ICollection<Nhanvien> Nhanviens { get; set; } = new List<Nhanvien>();
}
