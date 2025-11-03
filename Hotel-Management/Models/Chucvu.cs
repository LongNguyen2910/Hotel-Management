using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models;

public partial class Chucvu
{
    [Display(Name = "Tên chức vụ")]
    public string Tenchucvu { get; set; } = null!;
    [Display(Name = "Lương cơ bản")]
    public decimal? Luongcoban { get; set; }

    public virtual ICollection<Nhanvien> Nhanviens { get; set; } = new List<Nhanvien>();
}
