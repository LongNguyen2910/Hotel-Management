# Hotel-Management

Requirements

.NET 8.0;

Oracle Database 19c

Package:

- Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.21)
- Microsoft.AspNetCore.Identity.UI (8.0.21)
- Microsoft.EntityFrameworkCore (8.0.21)
- Microsoft.EntityFrameworkCore.Tools (8.0.21)
- Microsoft.VisualStudio.Web.CodeGeneration.Design (8.0.7)
- Oracle.Entity.FramworkCore (8.23.90)

---

Hướng dẫn cách cài đặt để chạy chương trình

Set up Database

- Đăng nhập vào user sys/system tạo user với username/password - quanly/quanly123
  "create user quanly identified by quanly123;"
- Gán quyền cho user quanly
  "grant create sesion, create table, create trigger, create sequence to quanly;"
- connect vào user quanly và sử dụng file chứa SQL tạo database (QUANLY.sql)

Set up Connection String

- Tạo file appsettings.json, thêm đoạn code (Chú ý thay đổi <your-ip/localhost> tùy theo máy)
  "ConnectionStrings": {
  "Oracle": "User Id=quanly;Password=quanly123;Data Source=//<your-ip/localhost>:1521/orcl;"
  }

Set up Roles Table

- Tại Package Manager Console gõ lệnh "Update-Database -Context UserContext"

---

Thành Viên

- Giáp Minh Hiển
- Nguyễn Đăng Khoa
- Nguyễn Đức Long
- Đỗ Thanh Sơn
