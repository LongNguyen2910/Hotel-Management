using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Hotel_Management.Models;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Baotri> Baotris { get; set; }

    public virtual DbSet<Bophan> Bophans { get; set; }

    public virtual DbSet<Calamviec> Calamviecs { get; set; }

    public virtual DbSet<Chucvu> Chucvus { get; set; }

    public virtual DbSet<Dichvu> Dichvus { get; set; }

    public virtual DbSet<Hoadon> Hoadons { get; set; }

    public virtual DbSet<Hoadondichvu> Hoadondichvus { get; set; }

    public virtual DbSet<Khachhang> Khachhangs { get; set; }

    public virtual DbSet<Khachhangdatmon> Khachhangdatmons { get; set; }

    public virtual DbSet<Khachhangdatphong> Khachhangdatphongs { get; set; }

    public virtual DbSet<Loaiphong> Loaiphongs { get; set; }

    public virtual DbSet<Loaithietbi> Loaithietbis { get; set; }

    public virtual DbSet<Mon> Mons { get; set; }

    public virtual DbSet<Ncccungcapnguyenlieu> Ncccungcapnguyenlieus { get; set; }

    public virtual DbSet<Ncccungcapthietbi> Ncccungcapthietbis { get; set; }

    public virtual DbSet<Nguyenlieu> Nguyenlieus { get; set; }

    public virtual DbSet<Nhacungcap> Nhacungcaps { get; set; }

    public virtual DbSet<Nhanvien> Nhanviens { get; set; }

    public virtual DbSet<Nhanvienlamca> Nhanvienlamcas { get; set; }

    public virtual DbSet<Phong> Phongs { get; set; }

    public virtual DbSet<Thietbi> Thietbis { get; set; }

    public virtual DbSet<Thietbiduocbaotri> Thietbiduocbaotris { get; set; }

    public virtual DbSet<Thucdon> Thucdons { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseOracle("Name=Oracle");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("QUANLY")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<Baotri>(entity =>
        {
            entity.HasKey(e => new { e.Ngaybatdau, e.Ngayketthuc }).HasName("SYS_C007668");

            entity.ToTable("BAOTRI");

            entity.Property(e => e.Ngaybatdau)
                .HasColumnType("DATE")
                .HasColumnName("NGAYBATDAU");
            entity.Property(e => e.Ngayketthuc)
                .HasColumnType("DATE")
                .HasColumnName("NGAYKETTHUC");
            entity.Property(e => e.Tienbaotri)
                .HasColumnType("NUMBER(12,3)")
                .HasColumnName("TIENBAOTRI");
            entity.Property(e => e.Trangthai)
                .HasColumnType("NUMBER(1)")
                .HasColumnName("TRANGTHAI");
        });

        modelBuilder.Entity<Bophan>(entity =>
        {
            entity.HasKey(e => e.Mabophan).HasName("SYS_C007688");

            entity.ToTable("BOPHAN");

            entity.Property(e => e.Mabophan)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MABOPHAN");
            entity.Property(e => e.Ngaythanhlap)
                .HasColumnType("DATE")
                .HasColumnName("NGAYTHANHLAP");
            entity.Property(e => e.Tenbophan)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("TENBOPHAN");
        });

        modelBuilder.Entity<Calamviec>(entity =>
        {
            entity.HasKey(e => e.Macalamviec).HasName("SYS_C007695");

            entity.ToTable("CALAMVIEC");

            entity.Property(e => e.Macalamviec)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MACALAMVIEC");
            entity.Property(e => e.Thoigianbatdau)
                .HasColumnType("DATE")
                .HasColumnName("THOIGIANBATDAU");
            entity.Property(e => e.Thoigianketthuc)
                .HasColumnType("DATE")
                .HasColumnName("THOIGIANKETTHUC");
        });

        modelBuilder.Entity<Chucvu>(entity =>
        {
            entity.HasKey(e => e.Tenchucvu).HasName("SYS_C007691");

            entity.ToTable("CHUCVU");

            entity.Property(e => e.Tenchucvu)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("TENCHUCVU");
            entity.Property(e => e.Luongcoban)
                .HasColumnType("NUMBER(12,3)")
                .HasColumnName("LUONGCOBAN");
        });

        modelBuilder.Entity<Dichvu>(entity =>
        {
            entity.HasKey(e => e.Madichvu).HasName("SYS_C007709");

            entity.ToTable("DICHVU");

            entity.Property(e => e.Madichvu)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MADICHVU");
            entity.Property(e => e.Gia)
                .HasColumnType("NUMBER(12,3)")
                .HasColumnName("GIA");
            entity.Property(e => e.Tendichvu)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("TENDICHVU");
            entity.Property(e => e.Trangthai)
                .HasColumnType("NUMBER(1)")
                .HasColumnName("TRANGTHAI");
        });

        modelBuilder.Entity<Hoadon>(entity =>
        {
            entity.HasKey(e => e.Mahoadon).HasName("SYS_C007715");

            entity.ToTable("HOADON");

            entity.Property(e => e.Mahoadon)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MAHOADON");
            entity.Property(e => e.Giadichvu)
                .HasColumnType("NUMBER(12,3)")
                .HasColumnName("GIADICHVU");
            entity.Property(e => e.Giamon)
                .HasColumnType("NUMBER(12,3)")
                .HasColumnName("GIAMON");
            entity.Property(e => e.Giaphong)
                .HasColumnType("NUMBER(12,3)")
                .HasColumnName("GIAPHONG");
            entity.Property(e => e.Makhachhang)
                .HasPrecision(10)
                .HasColumnName("MAKHACHHANG");
            entity.Property(e => e.Manv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MANV");
            entity.Property(e => e.Ngaylap)
                .HasColumnType("DATE")
                .HasColumnName("NGAYLAP");
            entity.Property(e => e.Ngaythanhtoan)
                .HasColumnType("DATE")
                .HasColumnName("NGAYTHANHTOAN");

            entity.HasOne(d => d.MakhachhangNavigation).WithMany(p => p.Hoadons)
                .HasForeignKey(d => d.Makhachhang)
                .HasConstraintName("FK_HOADON_KHACHHANG");

            entity.HasOne(d => d.ManvNavigation).WithMany(p => p.Hoadons)
                .HasForeignKey(d => d.Manv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HOADON_NHANVIEN");
        });

        modelBuilder.Entity<Hoadondichvu>(entity =>
        {
            entity.HasKey(e => new { e.Mahoadon, e.Madichvu }).HasName("SYS_C007719");

            entity.ToTable("HOADONDICHVU");

            entity.Property(e => e.Mahoadon)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MAHOADON");
            entity.Property(e => e.Madichvu)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MADICHVU");
            entity.Property(e => e.Ngaydat)
                .HasColumnType("DATE")
                .HasColumnName("NGAYDAT");
            entity.Property(e => e.Soluong)
                .HasPrecision(3)
                .HasDefaultValueSql("1")
                .HasColumnName("SOLUONG");

            entity.HasOne(d => d.MadichvuNavigation).WithMany(p => p.Hoadondichvus)
                .HasForeignKey(d => d.Madichvu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HDDV_DICHVU");

            entity.HasOne(d => d.MahoadonNavigation).WithMany(p => p.Hoadondichvus)
                .HasForeignKey(d => d.Mahoadon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HDDV_HOADON");
        });

        modelBuilder.Entity<Khachhang>(entity =>
        {
            entity.HasKey(e => e.Makhachhang).HasName("SYS_C007635");

            entity.ToTable("KHACHHANG");

            entity.Property(e => e.Makhachhang)
                .HasPrecision(10)
                .ValueGeneratedNever()
                .HasColumnName("MAKHACHHANG");
            entity.Property(e => e.Cccd)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("CCCD");
            entity.Property(e => e.Hochieu)
                .HasMaxLength(8)
                .IsUnicode(false)
                .HasColumnName("HOCHIEU");
            entity.Property(e => e.Hoten)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("HOTEN");
            entity.Property(e => e.Quoctich)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("QUOCTICH");
            entity.Property(e => e.Sdt)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SDT");
        });

        modelBuilder.Entity<Khachhangdatmon>(entity =>
        {
            entity.HasKey(e => new { e.Makhachhang, e.Mamon, e.Ngaydat }).HasName("SYS_C007651");

            entity.ToTable("KHACHHANGDATMON");

            entity.Property(e => e.Makhachhang)
                .HasPrecision(10)
                .HasColumnName("MAKHACHHANG");
            entity.Property(e => e.Mamon)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MAMON");
            entity.Property(e => e.Ngaydat)
                .HasColumnType("DATE")
                .HasColumnName("NGAYDAT");
            entity.Property(e => e.Soluong)
                .HasPrecision(3)
                .HasColumnName("SOLUONG");

            entity.HasOne(d => d.MakhachhangNavigation).WithMany(p => p.Khachhangdatmons)
                .HasForeignKey(d => d.Makhachhang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KHDM_KHACHHANG");

            entity.HasOne(d => d.MamonNavigation).WithMany(p => p.Khachhangdatmons)
                .HasForeignKey(d => d.Mamon)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KHDM_MON");
        });

        modelBuilder.Entity<Khachhangdatphong>(entity =>
        {
            entity.HasKey(e => new { e.Makhachhang, e.Maphong }).HasName("SYS_C007637");

            entity.ToTable("KHACHHANGDATPHONG");

            entity.Property(e => e.Makhachhang)
                .HasPrecision(10)
                .HasColumnName("MAKHACHHANG");
            entity.Property(e => e.Maphong)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MAPHONG");
            entity.Property(e => e.Ngaycheckin)
                .HasColumnType("DATE")
                .HasColumnName("NGAYCHECKIN");
            entity.Property(e => e.Ngaycheckout)
                .HasColumnType("DATE")
                .HasColumnName("NGAYCHECKOUT");
            entity.Property(e => e.Ngaydat)
                .HasColumnType("DATE")
                .HasColumnName("NGAYDAT");

            entity.HasOne(d => d.MakhachhangNavigation).WithMany(p => p.Khachhangdatphongs)
                .HasForeignKey(d => d.Makhachhang)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KHDP_KHACHHANG");

            entity.HasOne(d => d.MaphongNavigation).WithMany(p => p.Khachhangdatphongs)
                .HasForeignKey(d => d.Maphong)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KHDP_PHONG");
        });

        modelBuilder.Entity<Loaiphong>(entity =>
        {
            entity.HasKey(e => e.Maloaiphong).HasName("SYS_C007628");

            entity.ToTable("LOAIPHONG");

            entity.Property(e => e.Maloaiphong)
                .HasPrecision(6)
                .ValueGeneratedNever()
                .HasColumnName("MALOAIPHONG");
            entity.Property(e => e.Gia)
                .HasColumnType("NUMBER(12,3)")
                .HasColumnName("GIA");
            entity.Property(e => e.Succhua)
                .HasPrecision(3)
                .HasColumnName("SUCCHUA");
            entity.Property(e => e.Tenloaiphong)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("TENLOAIPHONG");
        });

        modelBuilder.Entity<Loaithietbi>(entity =>
        {
            entity.HasKey(e => e.Maloaithietbi).HasName("SYS_C007655");

            entity.ToTable("LOAITHIETBI");

            entity.Property(e => e.Maloaithietbi)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MALOAITHIETBI");
            entity.Property(e => e.Tenloaithietbi)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TENLOAITHIETBI");
        });

        modelBuilder.Entity<Mon>(entity =>
        {
            entity.HasKey(e => e.Mamon).HasName("SYS_C007645");

            entity.ToTable("MON");

            entity.Property(e => e.Mamon)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MAMON");
            entity.Property(e => e.Gia)
                .HasColumnType("NUMBER(12,3)")
                .HasColumnName("GIA");
            entity.Property(e => e.Tenmon)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TENMON");

            entity.HasMany(d => d.Thucdons).WithMany(p => p.Mamons)
                .UsingEntity<Dictionary<string, object>>(
                    "Monthuocthucdon",
                    r => r.HasOne<Thucdon>().WithMany()
                        .HasForeignKey("Mathucdon", "Ngayapdung")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_MTTD_THUCDON"),
                    l => l.HasOne<Mon>().WithMany()
                        .HasForeignKey("Mamon")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_MTTD_MON"),
                    j =>
                    {
                        j.HasKey("Mamon", "Mathucdon", "Ngayapdung").HasName("SYS_C007646");
                        j.ToTable("MONTHUOCTHUCDON");
                        j.IndexerProperty<string>("Mamon")
                            .HasMaxLength(10)
                            .IsUnicode(false)
                            .HasColumnName("MAMON");
                        j.IndexerProperty<int>("Mathucdon")
                            .HasPrecision(10)
                            .HasColumnName("MATHUCDON");
                        j.IndexerProperty<DateTime>("Ngayapdung")
                            .HasColumnType("DATE")
                            .HasColumnName("NGAYAPDUNG");
                    });
        });

        modelBuilder.Entity<Ncccungcapnguyenlieu>(entity =>
        {
            entity.HasKey(e => new { e.Mancc, e.Manguyenlieu }).HasName("SYS_C007683");

            entity.ToTable("NCCCUNGCAPNGUYENLIEU");

            entity.Property(e => e.Mancc)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MANCC");
            entity.Property(e => e.Manguyenlieu)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MANGUYENLIEU");
            entity.Property(e => e.Luong)
                .HasPrecision(10)
                .HasColumnName("LUONG");
            entity.Property(e => e.Tiennguyenlieu)
                .HasColumnType("NUMBER(12,3)")
                .HasColumnName("TIENNGUYENLIEU");

            entity.HasOne(d => d.ManccNavigation).WithMany(p => p.Ncccungcapnguyenlieus)
                .HasForeignKey(d => d.Mancc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NCCCNL_NCC");

            entity.HasOne(d => d.ManguyenlieuNavigation).WithMany(p => p.Ncccungcapnguyenlieus)
                .HasForeignKey(d => d.Manguyenlieu)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NCCCNL_NL");
        });

        modelBuilder.Entity<Ncccungcapthietbi>(entity =>
        {
            entity.HasKey(e => new { e.Mancc, e.Mathietbi }).HasName("SYS_C007678");

            entity.ToTable("NCCCUNGCAPTHIETBI");

            entity.Property(e => e.Mancc)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MANCC");
            entity.Property(e => e.Mathietbi)
                .HasPrecision(10)
                .HasColumnName("MATHIETBI");
            entity.Property(e => e.Soluong)
                .HasPrecision(10)
                .HasColumnName("SOLUONG");
            entity.Property(e => e.Tienthietbi)
                .HasColumnType("NUMBER(12,3)")
                .HasColumnName("TIENTHIETBI");

            entity.HasOne(d => d.ManccNavigation).WithMany(p => p.Ncccungcapthietbis)
                .HasForeignKey(d => d.Mancc)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NCCCTB_NCC");

            entity.HasOne(d => d.MathietbiNavigation).WithMany(p => p.Ncccungcapthietbis)
                .HasForeignKey(d => d.Mathietbi)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NCCTB_THIETBI");
        });

        modelBuilder.Entity<Nguyenlieu>(entity =>
        {
            entity.HasKey(e => e.Manguyenlieu).HasName("SYS_C007675");

            entity.ToTable("NGUYENLIEU");

            entity.Property(e => e.Manguyenlieu)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MANGUYENLIEU");
            entity.Property(e => e.Hansudung)
                .HasColumnType("DATE")
                .HasColumnName("HANSUDUNG");
            entity.Property(e => e.Ngaynhap)
                .HasColumnType("DATE")
                .HasColumnName("NGAYNHAP");
            entity.Property(e => e.Ngaysanxuat)
                .HasColumnType("DATE")
                .HasColumnName("NGAYSANXUAT");
            entity.Property(e => e.Tennguyenlieu)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("TENNGUYENLIEU");
        });

        modelBuilder.Entity<Nhacungcap>(entity =>
        {
            entity.HasKey(e => e.Mancc).HasName("SYS_C007672");

            entity.ToTable("NHACUNGCAP");

            entity.Property(e => e.Mancc)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MANCC");
            entity.Property(e => e.Diachi)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("DIACHI");
            entity.Property(e => e.Email)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.Sdt)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SDT");
            entity.Property(e => e.Tenncc)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TENNCC");
        });

        modelBuilder.Entity<Nhanvien>(entity =>
        {
            entity.HasKey(e => e.Manv).HasName("SYS_C007699");

            entity.ToTable("NHANVIEN");

            entity.Property(e => e.Manv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MANV");
            entity.Property(e => e.Cccd)
                .HasMaxLength(12)
                .IsUnicode(false)
                .HasColumnName("CCCD");
            entity.Property(e => e.Gioitinh)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("GIOITINH");
            entity.Property(e => e.Hoten)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("HOTEN");
            entity.Property(e => e.Mabophan)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MABOPHAN");
            entity.Property(e => e.Ngaysinh)
                .HasColumnType("DATE")
                .HasColumnName("NGAYSINH");
            entity.Property(e => e.Ngayvaolam)
                .HasColumnType("DATE")
                .HasColumnName("NGAYVAOLAM");
            entity.Property(e => e.Sodienthoai)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("SODIENTHOAI");
            entity.Property(e => e.Tenchucvu)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("TENCHUCVU");
            entity.Property(e => e.Trangthai)
                .HasColumnType("NUMBER(1)")
                .HasColumnName("TRANGTHAI");

            entity.HasOne(d => d.MabophanNavigation).WithMany(p => p.Nhanviens)
                .HasForeignKey(d => d.Mabophan)
                .HasConstraintName("FK_NV_BOPHAN");

            entity.HasOne(d => d.TenchucvuNavigation).WithMany(p => p.Nhanviens)
                .HasForeignKey(d => d.Tenchucvu)
                .HasConstraintName("FK_NV_CHUCVU");
        });

        modelBuilder.Entity<Nhanvienlamca>(entity =>
        {
            entity.HasKey(e => new { e.Manv, e.Macalamviec }).HasName("SYS_C007703");

            entity.ToTable("NHANVIENLAMCA");

            entity.Property(e => e.Manv)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MANV");
            entity.Property(e => e.Macalamviec)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MACALAMVIEC");
            entity.Property(e => e.Ngaylam)
                .HasColumnType("DATE")
                .HasColumnName("NGAYLAM");

            entity.HasOne(d => d.MacalamviecNavigation).WithMany(p => p.Nhanvienlamcas)
                .HasForeignKey(d => d.Macalamviec)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NVLC_CALAMVIEC");

            entity.HasOne(d => d.ManvNavigation).WithMany(p => p.Nhanvienlamcas)
                .HasForeignKey(d => d.Manv)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NVLC_NHANVIEN");
        });

        modelBuilder.Entity<Phong>(entity =>
        {
            entity.HasKey(e => e.Maphong).HasName("SYS_C007632");

            entity.ToTable("PHONG");

            entity.Property(e => e.Maphong)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MAPHONG");
            entity.Property(e => e.Anhphong)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("ANHPHONG");
            entity.Property(e => e.Maloaiphong)
                .HasPrecision(6)
                .HasColumnName("MALOAIPHONG");
            entity.Property(e => e.Mota)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("MOTA");
            entity.Property(e => e.Tenphong)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("TENPHONG");
            entity.Property(e => e.Tinhtrang)
                .HasColumnType("NUMBER(1)")
                .HasColumnName("TINHTRANG");

            entity.HasOne(d => d.MaloaiphongNavigation).WithMany(p => p.Phongs)
                .HasForeignKey(d => d.Maloaiphong)
                .HasConstraintName("FK_PHONG_LOAIPHONG");
        });

        modelBuilder.Entity<Thietbi>(entity =>
        {
            entity.HasKey(e => e.Mathietbi).HasName("SYS_C007659");

            entity.ToTable("THIETBI");

            entity.Property(e => e.Mathietbi)
                .HasPrecision(10)
                .ValueGeneratedNever()
                .HasColumnName("MATHIETBI");
            entity.Property(e => e.Maloaithietbi)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("MALOAITHIETBI");
            entity.Property(e => e.Tenthietbi)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("TENTHIETBI");
            entity.Property(e => e.Tinhtrang)
                .HasColumnType("NUMBER(1)")
                .HasColumnName("TINHTRANG");

            entity.HasOne(d => d.MaloaithietbiNavigation).WithMany(p => p.Thietbis)
                .HasForeignKey(d => d.Maloaithietbi)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_THIETBI_LOAITHIEBI");

            entity.HasMany(d => d.Maphongs).WithMany(p => p.Mathietbis)
                .UsingEntity<Dictionary<string, object>>(
                    "Phongchuathietbi",
                    r => r.HasOne<Phong>().WithMany()
                        .HasForeignKey("Maphong")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_PCTB_PHONG"),
                    l => l.HasOne<Thietbi>().WithMany()
                        .HasForeignKey("Mathietbi")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_PCTB_THIETBI"),
                    j =>
                    {
                        j.HasKey("Mathietbi", "Maphong").HasName("SYS_C007661");
                        j.ToTable("PHONGCHUATHIETBI");
                        j.IndexerProperty<int>("Mathietbi")
                            .HasPrecision(10)
                            .HasColumnName("MATHIETBI");
                        j.IndexerProperty<string>("Maphong")
                            .HasMaxLength(10)
                            .IsUnicode(false)
                            .HasColumnName("MAPHONG");
                    });
        });

        modelBuilder.Entity<Thietbiduocbaotri>(entity =>
        {
            entity.HasKey(e => new { e.Ngaybatdau, e.Ngayketthuc, e.Mathietbi }).HasName("SYS_C007669");

            entity.ToTable("THIETBIDUOCBAOTRI");

            entity.Property(e => e.Ngaybatdau)
                .HasColumnType("DATE")
                .HasColumnName("NGAYBATDAU");
            entity.Property(e => e.Ngayketthuc)
                .HasColumnType("DATE")
                .HasColumnName("NGAYKETTHUC");
            entity.Property(e => e.Mathietbi)
                .HasPrecision(10)
                .HasColumnName("MATHIETBI");

            entity.HasOne(d => d.Baotri).WithMany(p => p.Thietbiduocbaotris)
                .HasForeignKey(d => new { d.Ngaybatdau, d.Ngayketthuc })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TBDBT_BAOTRI");
        });

        modelBuilder.Entity<Thucdon>(entity =>
        {
            entity.HasKey(e => new { e.Mathucdon, e.Ngayapdung }).HasName("SYS_C007642");

            entity.ToTable("THUCDON");

            entity.Property(e => e.Mathucdon)
                .HasPrecision(10)
                .HasColumnName("MATHUCDON");
            entity.Property(e => e.Ngayapdung)
                .HasColumnType("DATE")
                .HasColumnName("NGAYAPDUNG");
            entity.Property(e => e.Ngaytao)
                .HasColumnType("DATE")
                .HasColumnName("NGAYTAO");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
