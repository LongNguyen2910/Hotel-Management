using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models;

public partial class Nhanvien
{
    [Display(Name = "Mã nhân viên")]
    public string Manv { get; set; } = null!;
    [Display(Name = "Họ và tên")]
    public string? Hoten { get; set; }
    [Display(Name = "Giới tính")]
    public string? Gioitinh { get; set; }
    [Display(Name = "Ngày sinh")]
    public DateTime? Ngaysinh { get; set; }
    [Display(Name = "Số điện thoại")]
    public string? Sodienthoai { get; set; }
    
    public string? Cccd { get; set; }
    [Display(Name = "Ngày vào làm")]
    public DateTime? Ngayvaolam { get; set; }
    [Display(Name = "Trạng thái")]
    public byte? Trangthai { get; set; }
    
    public string? Mabophan { get; set; }
    
    public string? Tenchucvu { get; set; }

    public virtual ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();
    [Display(Name = "Bộ phận")]
    public virtual Bophan? MabophanNavigation { get; set; }
    [Display(Name = "Ca làm việc")]
    public virtual ICollection<Nhanvienlamca> Nhanvienlamcas { get; set; } = new List<Nhanvienlamca>();
    [Display(Name = "Chức vụ")]
    public virtual Chucvu? TenchucvuNavigation { get; set; }
}
