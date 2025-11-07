using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models;

public partial class Nhacungcap
{
    [Display(Name = "Mã nhà cung cấp")]
    [Required(ErrorMessage = "Mã nhà cung cấp không được để trống")]
    public string? Manhacungcap { get; set; }

    [Display(Name = "Tên nhà cung cấp")]
    [Required(ErrorMessage = "Tên nhà cung cấp không được để trống")]
    public string? Tennhacungcap { get; set; }

    [Display(Name = "Địa chỉ")]
    public string? Diachi { get; set; }

    [Display(Name = "Số điện thoại")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string? Sodienthoai { get; set; }

    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string? Email { get; set; }

    public virtual ICollection<Ncccungcapnguyenlieu> Ncccungcapnguyenlieus { get; set; } = new List<Ncccungcapnguyenlieu>();

    public virtual ICollection<Ncccungcapthietbi> Ncccungcapthietbis { get; set; } = new List<Ncccungcapthietbi>();
}
