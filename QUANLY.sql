-- Bảng Loại Phòng
CREATE TABLE LoaiPhong (
    MaLoaiPhong NUMBER(6) not null PRIMARY KEY,
    TenLoaiPhong VARCHAR2(100 CHAR) NOT NULL,
    SucChua NUMBER(3) CHECK (SucChua >= 1),
    Gia NUMBER(12,3) CHECK (Gia >= 0)
);
/
--Bảng Phòng
CREATE TABLE Phong (
    MaPhong VARCHAR2(10) not null PRIMARY KEY,
    TenPhong VARCHAR2(100 CHAR),
    TinhTrang NUMBER(1) NOT NULL CHECK (TinhTrang IN (0,1,3,4)),
    MoTa VARCHAR2(500),
    AnhPhong VARCHAR2(100),
    MaLoaiPhong NUMBER(6),
    CONSTRAINT fk_phong_loaiphong FOREIGN KEY (MaLoaiPhong) 
        REFERENCES LoaiPhong(MaLoaiPhong)
);
/
--Bảng Khách Hàng
CREATE TABLE KhachHang (
    MaKhachHang NUMBER(10) not null PRIMARY KEY,
    HoTen VARCHAR2(100 CHAR),
    QuocTich VARCHAR2(60),
    CCCD VARCHAR2(12),
    SDT VARCHAR2(10),
    HoChieu VARCHAR2(8)
);
/
--Bảng Khách hàng đặt phòng
CREATE TABLE KhachHangDatPhong (
    MaKhachHang NUMBER(10),
    MaPhong VARCHAR2(10),
    NgayDat DATE,
    NgayCheckIn DATE,
    NgayCheckOut DATE,
    PRIMARY KEY (MaKhachHang, MaPhong),
    CONSTRAINT fk_khdp_khachhang FOREIGN KEY (MaKhachHang) 
        REFERENCES KhachHang(MaKhachHang),
    CONSTRAINT fk_khdp_phong FOREIGN KEY (MaPhong) 
        REFERENCES Phong(MaPhong),
    CONSTRAINT chk_ngay_dat CHECK (NgayDat <= NgayCheckIn AND NgayCheckIn <= NgayCheckOut)
);
/
--Bảng Thực đơn
CREATE TABLE ThucDon (
    MaThucDon NUMBER(10) not null,
    NgayApDung DATE,
    NgayTao DATE NOT NULL,
    primary key (MaThucDon,NgayApDung)
);
/
-- Bảng Món
CREATE TABLE Mon (
    MaMon VARCHAR2(10) not null PRIMARY KEY,
    TenMon VARCHAR2(50 CHAR),
    AnhMon VARCHAR2(100),
    Gia NUMBER(12,3) CHECK (Gia >= 0)
);
/
-- Bảng Món Thuộc Thực Đơn
CREATE TABLE MonThuocThucDon (
    MaMon VARCHAR2(10),
    MaThucDon NUMBER(10),
    NgayApDung DATE,
    PRIMARY KEY (MaMon, MaThucDon, NgayApDung),
    CONSTRAINT fk_mttd_mon FOREIGN KEY (MaMon) 
        REFERENCES Mon(MaMon),
    CONSTRAINT fk_mttd_thucdon FOREIGN KEY (MaThucDon,NgayApDung) 
        REFERENCES ThucDon(MaThucDon,NgayApDung)
    
);
/
-- Bảng Khách Hàng Đặt Món
CREATE TABLE KhachHangDatMon (
    MaKhachHang NUMBER(10),
    MaMon VARCHAR2(10),
    NgayDat DATE NOT NULL,
    SoLuong NUMBER(3) CHECK (SoLuong > 0),
    PRIMARY KEY (MaKhachHang, MaMon, NgayDat),
    CONSTRAINT fk_khdm_khachhang FOREIGN KEY (MaKhachHang) 
        REFERENCES KhachHang(MaKhachHang),
    CONSTRAINT fk_khdm_mon FOREIGN KEY (MaMon) 
        REFERENCES Mon(MaMon)
);
/
--Bang LoaiTHETBI
CREATE TABLE LoaiThietBi (
    MaLoaiThietBi VARCHAR(10) not null primary key,
    TenLoaiThietBi VARCHAR2(50 CHAR)
);
/
--Bảng Thiết bị
CREATE TABLE ThietBi (
    MaThietBi NUMBER(10) not null primary key,
    TenThietBi VARCHAR2(50 CHAR),
    TinhTrang NUMBER(1) CHECK (TinhTrang IN (0,1,2)),
    MaLoaiThietBi VARCHAR(10) not null,
    constraint fk_thietbi_loaithiebi FOREIGN KEY (MaLoaiThietBi)
        references LoaiThietBi(MaLoaiThietBi)
);
/
-- Bảng Phòng Chứa Thiết Bị
CREATE TABLE PhongChuaThietBi (
    MaThietBi NUMBER(10),
    MaPhong VARCHAR2(10),
    PRIMARY KEY (MaThietBi, MaPhong),
    CONSTRAINT fk_pctb_phong FOREIGN KEY (MaPhong) 
        REFERENCES Phong(MaPhong),
    constraint fk_pctb_thietbi foreign key(MaThietBi)
        references ThietBi(MaThietBi)
);
/
-- Bảng Bảo Trì
CREATE TABLE BaoTri (
    NgayBatDau DATE not null,
    NgayKetThuc DATE not null,
    TrangThai NUMBER(1) CHECK (TrangThai IN (0,1,2)),
    TienBaoTri NUMBER(12,3) CHECK (TienBaoTri >= 0),
    PRIMARY KEY (NgayBatDau, NgayKetThuc)
);
/
-- Bảng Thiết Bị Được Bảo Trì
CREATE TABLE ThietBiDuocBaoTri (
    NgayBatDau DATE,
    NgayKetThuc DATE,
    MaThietBi NUMBER(10),
    PRIMARY KEY (NgayBatDau, NgayKetThuc, MaThietBi),
    CONSTRAINT fk_tbdbt_baotri FOREIGN KEY (NgayBatDau, NgayKetThuc) 
        REFERENCES BaoTri(NgayBatDau, NgayKetThuc)
);
/
-- Bảng Nhà Cung Cấp
CREATE TABLE NhaCungCap (
    MaNCC VARCHAR2(10) NOT NULL PRIMARY KEY,
    TenNCC VARCHAR2(50 CHAR),
    DiaChi VARCHAR2(100 CHAR),
    SDT VARCHAR2(10),
    Email VARCHAR2(500 CHAR)
);
/
-- Bảng NCC Cung Cấp Thiết Bị
CREATE TABLE NCCCungCapThietBi (
    MaNCC VARCHAR2(10),
    MaThietBi NUMBER(10),
    SoLuong NUMBER(10) CHECK (SoLuong >= 0),
    TienThietBi NUMBER(12,3) CHECK (TienThietBi >= 0),
    PRIMARY KEY (MaNCC, MaThietBi),
    CONSTRAINT fk_ncctb_thietbi FOREIGN KEY (MaThietBi)
        REFERENCES ThietBi(MaThietBi),
    CONSTRAINT fk_nccctb_ncc FOREIGN KEY (MaNCC) 
        REFERENCES NhaCungCap(MaNCC)
);
/
-- Bảng NCC Cung Cấp Nguyên Liệu
CREATE TABLE NCCCungCapNguyenLieu (
    MaNCC VARCHAR2(10),
    MaNguyenLieu VARCHAR2(10),
    Luong NUMBER(10) CHECK (Luong >= 0),
    TienNguyenLieu NUMBER(12,3) CHECK (TienNguyenLieu >= 0),
    TENNGUYENLIEU VARCHAR2(100),
    NGAYSANXUAT DATE,
    NGAYNHAP DATE,
     HANSUDUNG DATE,
    PRIMARY KEY (MaNCC, MaNguyenLieu),
    CONSTRAINT fk_ncccnl_ncc FOREIGN KEY (MaNCC) 
        REFERENCES NhaCungCap(MaNCC),
);
/
-- Bảng Bộ Phận
CREATE TABLE BoPhan (
    MaBoPhan VARCHAR2(10) not null PRIMARY KEY,
    TenBoPhan VARCHAR2(100 CHAR) NOT NULL,
    NgayThanhLap DATE
);
/
-- Bảng Chức Vụ
CREATE TABLE ChucVu (
    TenChucVu VARCHAR2(100 CHAR) not null PRIMARY KEY,
    LuongCoBan NUMBER(12,3) CHECK (LuongCoBan >= 0)
);
/
-- Bảng Ca Làm Việc
CREATE TABLE CaLamViec (
    MaCaLamViec VARCHAR2(10) PRIMARY KEY,
    ThoiGianBatDau DATE NOT NULL,
    ThoiGianKetThuc DATE NOT NULL,
    CONSTRAINT chk_gio_ca CHECK (ThoiGianBatDau < ThoiGianKetThuc)
);
/
-- Bảng Nhân Viên
CREATE TABLE NhanVien (
    MaNV VARCHAR2(10) not null PRIMARY KEY,
    HoTen VARCHAR2(100 CHAR),
    GioiTinh VARCHAR2(5) CHECK (GioiTinh IN ('Nam', N'Nữ')),
    NgaySinh DATE,
    SoDienThoai VARCHAR2(10),
    CCCD VARCHAR2(12),
    NgayVaoLam DATE,
    TrangThai NUMBER(1) CHECK (TrangThai IN (0,1)),
    MaBoPhan VARCHAR2(10),
    TenChucVu VARCHAR2(100 CHAR),
    AnhNhanVien VARCHAR2(100),
    CONSTRAINT fk_nv_bophan FOREIGN KEY (MaBoPhan) 
        REFERENCES BoPhan(MaBoPhan),
    CONSTRAINT fk_nv_chucvu FOREIGN KEY (TenChucVu) 
        REFERENCES ChucVu(TenChucVu)
);
/
-- Bảng Nhân Viên Làm Ca
CREATE TABLE NhanVienLamCa (
    MaNV VARCHAR2(10),
    MaCaLamViec VARCHAR2(10),
    NgayLam DATE NOT NULL,
    PRIMARY KEY (MaNV, MaCaLamViec),
    CONSTRAINT fk_nvlc_nhanvien FOREIGN KEY (MaNV) 
        REFERENCES NhanVien(MaNV),
    CONSTRAINT fk_nvlc_calamviec FOREIGN KEY (MaCaLamViec) 
        REFERENCES CaLamViec(MaCaLamViec)
);
/
-- Bảng Dịch Vụ
CREATE TABLE DichVu (
    MaDichVu VARCHAR2(10) not null PRIMARY KEY,
    TenDichVu VARCHAR2(100 CHAR),
    Gia NUMBER(12,3) CHECK (Gia >= 0),
    TrangThai NUMBER(1) CHECK (TrangThai IN (0,1))
);
/
-- Bảng Hóa Đơn
CREATE TABLE HoaDon (
    MaHoaDon VARCHAR2(10) not null PRIMARY KEY,
    NgayLap DATE,
    NgayThanhToan DATE,
    GiaPhong NUMBER(12,3) CHECK (GiaPhong >= 0),
    GiaDichVu NUMBER(12,3) CHECK (GiaDichVu >= 0),
    GiaMon NUMBER(12,3) CHECK (GiaMon >= 0),
    MaKhachHang NUMBER(10),
    MaNV VARCHAR2(10) not null,
    CONSTRAINT fk_hoadon_khachhang FOREIGN KEY (MaKhachHang) 
        REFERENCES KhachHang(MaKhachHang)
);
/
-- Bảng Hóa Đơn Dịch Vụ
CREATE TABLE HoaDonDichVu (
    MaHoaDon VARCHAR2(10),
    MaDichVu VARCHAR2(10),
    NgayDat DATE NOT NULL,
    SoLuong NUMBER(3) DEFAULT 1,
    PRIMARY KEY (MaHoaDon, MaDichVu),
    CONSTRAINT fk_hddv_hoadon FOREIGN KEY (MaHoaDon) 
        REFERENCES HoaDon(MaHoaDon),
    CONSTRAINT fk_hddv_dichvu FOREIGN KEY (MaDichVu) 
        REFERENCES DichVu(MaDichVu)
);
