using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models;

public partial class Phong
{
    [Required(ErrorMessage = "Mã phòng không được để trống")]
    public string Maphong { get; set; } = null!;

    [Required(ErrorMessage = "Tên phòng không được để trống")]
    public string? Tenphong { get; set; }

    [Required(ErrorMessage = "Tình trạng phòng không được để trống")]
    public bool Tinhtrang { get; set; }

    [Required(ErrorMessage = "Mô tả phòng không được để trống")]
    public string? Mota { get; set; }

    [Required(ErrorMessage = "mã loại phòng không được để trống")]
    public int? Maloaiphong { get; set; }

    public string? Anhphong { get; set; }

    public virtual ICollection<Khachhangdatphong> Khachhangdatphongs { get; set; } = new List<Khachhangdatphong>();

    public virtual Loaiphong? MaloaiphongNavigation { get; set; }

    public virtual ICollection<Thietbi> Mathietbis { get; set; } = new List<Thietbi>();
}
