using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models;

public partial class Nhanvien
{
    [Display(Name = "Mã nhân viên")]
    [Required(ErrorMessage = "Mã nhân viên không được để trống")]
    public string? Manv { get; set; }
    [Display(Name = "Họ và tên")]
    [Required(ErrorMessage = "Tên nhân viên không được để trống")]
    public string? Hoten { get; set; }
    [Display(Name = "Giới tính")]
    
    public string? Gioitinh { get; set; }
    [Display(Name = "Ngày sinh")]
    [Required(ErrorMessage = "Ngày sinh không được để trống")]
    public DateTime? Ngaysinh { get; set; }
    [Display(Name = "Số điện thoại")]
    [Required(ErrorMessage = "Số điện thoại không được để trống")]
    [StringLength(10, ErrorMessage = "Số điện thoại phải đúng 10 chữ số",MinimumLength = 10)]
    public string? Sodienthoai { get; set; }
    [Required(ErrorMessage = "CCCD không được để trống")]
    [StringLength(12, ErrorMessage = "Số CCCD phải đúng 12 chữ số", MinimumLength = 12)]
    public string? Cccd { get; set; }
    [Display(Name = "Ngày vào làm")]
    [Required(ErrorMessage = "Ngày vào làm không được để trống")]
    public DateTime? Ngayvaolam { get; set; }
    [Display(Name = "Trạng thái")]
    public byte? Trangthai { get; set; }
    public string? Anhnv { get; set; }
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
