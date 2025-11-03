using System;
using System.Collections.Generic;

namespace Hotel_Management.Models;

public partial class Nhacungcap
{
    public string Mancc { get; set; } = null!;

    public string? Tenncc { get; set; }

    public string? Diachi { get; set; }

    public string? Sdt { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Ncccungcapnguyenlieu> Ncccungcapnguyenlieus { get; set; } = new List<Ncccungcapnguyenlieu>();

    public virtual ICollection<Ncccungcapthietbi> Ncccungcapthietbis { get; set; } = new List<Ncccungcapthietbi>();
}
