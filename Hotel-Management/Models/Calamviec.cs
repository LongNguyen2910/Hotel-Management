using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models;

public partial class Calamviec
{
    [Required(ErrorMessage = "Tên ca làm việc không được để trống")]
    public string? Macalamviec { get; set; }
    [Required(ErrorMessage = "Thời gian bắt đầu không được để trống")]
    public DateTime? Thoigianbatdau { get; set; }
    [Required(ErrorMessage = "Thời gian kết thúc không được để trống")]
    public DateTime? Thoigianketthuc { get; set; }

    public virtual ICollection<Nhanvienlamca> Nhanvienlamcas { get; set; } = new List<Nhanvienlamca>();
}
