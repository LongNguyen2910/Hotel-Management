using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Thietbi
{
    public int Mathietbi { get; set; }

    public string? Tenthietbi { get; set; }

    public bool? Tinhtrang { get; set; }

    public string Maloaithietbi { get; set; } = null!;

    [ValidateNever]
    public virtual Loaithietbi MaloaithietbiNavigation { get; set; } = null!;

    public virtual ICollection<Ncccungcapthietbi> Ncccungcapthietbis { get; set; } = new List<Ncccungcapthietbi>();

    public virtual ICollection<Phong> Maphongs { get; set; } = new List<Phong>();
}
