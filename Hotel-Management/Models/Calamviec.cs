using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models;

public partial class Calamviec
{
    [Display(Name = "Tên ca làm việc")]
    public string Macalamviec { get; set; } = null!;
    [Display(Name = "Thời gian bắt đầu")]
    public DateTime Thoigianbatdau { get; set; }
    [Display(Name = "Thời gian kết thúc")]
    public DateTime Thoigianketthuc { get; set; }

    public virtual ICollection<Nhanvienlamca> Nhanvienlamcas { get; set; } = new List<Nhanvienlamca>();
}
