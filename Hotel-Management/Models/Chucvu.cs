using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models;

public partial class Chucvu
{
    [Required(ErrorMessage = "Tên chức vụ không được để trống")]
    public string? Tenchucvu { get; set; }
    [Required(ErrorMessage = "Lương cơ bản không được để trống")]
    public decimal? Luongcoban { get; set; }

    public virtual ICollection<Nhanvien> Nhanviens { get; set; } = new List<Nhanvien>();
}
