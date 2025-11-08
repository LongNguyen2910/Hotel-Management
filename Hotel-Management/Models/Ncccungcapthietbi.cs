using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel_Management.Models;

public partial class Ncccungcapthietbi
{
    [Key, Column(Order = 0)]
    [Required(ErrorMessage = "Vui lòng chọn nhà cung cấp")]
    public string Mancc { get; set; } = null!;
    [Key, Column(Order = 1)]
    [Required(ErrorMessage = "Vui lòng chọn thiết bị")]
    public int Mathietbi { get; set; }

    public int? Soluong { get; set; }

    public decimal? Tienthietbi { get; set; }
    [ValidateNever]
    public virtual Nhacungcap ManccNavigation { get; set; } = null!;
    [ValidateNever]

    public virtual Thietbi MathietbiNavigation { get; set; } = null!;
}
