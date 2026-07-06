# SRT Group - Todo List Application

Ứng dụng Todo List quản lý công việc hàng ngày được xây dựng trên nền tảng **ASP.NET Core 8.0 MVC**, sử dụng **Entity Framework Core với SQLite**, kiểm thử bằng **xUnit** và hỗ trợ container hóa bằng **Docker**.

---

## Công Nghệ Sử Dụng

- **Backend**: .NET 8.0 (ASP.NET Core MVC)
- **Database**: SQLite (sử dụng EF Core Code-First Migrations)
- **Frontend**: HTML5, CSS3 (Custom Flat Design), Bootstrap 5, Vanilla JavaScript (AJAX / Fetch API)
- **Unit Testing**: xUnit, EF Core InMemory Database
- **Containerization**: Docker & Docker Compose

---

## Các Tính Năng Chính

1. **Quản lý Công việc (CRUD)**:
   - Thêm mới công việc với Tiêu đề, Mô tả và Hạn hoàn thành (Due Date).
   - Chỉnh sửa thông tin công việc, thay đổi trạng thái hoàn thành thông qua Modal.
   - Xóa công việc với xác nhận bảo mật.
2. **Tương Tác Real-time (AJAX/Fetch)**:
   - Cập nhật trạng thái hoàn thành (Checkbox Toggle) không tải lại trang.
   - Thêm, sửa, xóa bất đồng bộ kèm hiệu ứng fade-out mượt mà khi xóa.
3. **Bộ Lọc & Sắp Xếp Trực Quan**:
   - Tìm kiếm công việc theo từ khóa tiêu đề hoặc mô tả.
   - Lọc công việc theo trạng thái: Tất cả, Chưa hoàn thành, Đã hoàn thành.
   - Sắp xếp linh hoạt: Mới nhất, Cũ nhất, Theo hạn hoàn thành (tăng/giảm), Theo bảng chữ cái (A-Z/Z-A).
4. **Bảng Thống Kê (Statistics)**:
   - Hiển thị trực quan số lượng công việc: Tổng số, Đang hoạt động, Đã hoàn thành.
   - Tự động cập nhật số liệu ngay khi hoàn thành hoặc xóa công việc thông qua AJAX.
5. **Giao Diện Đơn Giản, Hiện Đại (Minimalist UI)**:
   - Thiết kế phẳng gọn gàng, sử dụng bảng màu phối hợp hài hòa (3-4 màu chủ đạo).
   - Tương thích tốt với các thiết bị di động (Responsive Layout).

---

## Hướng Dẫn Cài Đặt & Chạy Ứng Dụng

### Cách 1: Chạy trực tiếp trên máy local (Dotnet SDK)

#### Yêu cầu hệ thống:
- Đã cài đặt **.NET 8.0 SDK**
- IDE/Editor: Visual Studio 2022, JetBrains Rider hoặc VS Code

#### Các bước thực hiện:
1. Mở terminal tại thư mục chứa source code.
2. Khôi phục các thư viện dependencies:
   ```bash
   dotnet restore
   ```
3. Khởi chạy dự án Web:
   ```bash
   dotnet run --project SRT_group_todo_list
   ```
   *Ứng dụng sẽ chạy tại địa chỉ mặc định: `http://localhost:5299` hoặc cổng HTTPS ngẫu nhiên.*

---

### Cách 2: Chạy bằng Docker (Khuyên dùng)

Ứng dụng hỗ trợ Docker Compose giúp tự động khởi tạo cơ sở dữ liệu SQLite trong một volume lưu trữ bền vững (persistent storage).

#### Yêu cầu hệ thống:
- Đã cài đặt **Docker** và **Docker Compose**

#### Các bước thực hiện:
1. Mở terminal tại thư mục gốc của dự án.
2. Build và khởi động các container ở chế độ chạy ngầm (detached):
   ```bash
   docker-compose up --build -d
   ```
3. Truy cập ứng dụng qua trình duyệt tại địa chỉ:
   ```http
   http://localhost:8080
   ```
4. Để dừng và xóa các container:
   ```bash
   docker-compose down
   ```

---

## Chạy Kiểm Thử (Unit Tests)

Dự án đã tích hợp sẵn 8 kịch bản kiểm thử bao phủ toàn bộ logic xử lý nghiệp vụ của `TodoService` (bao gồm CRUD, bộ lọc, sắp xếp và kiểm tra dữ liệu đầu vào).

Để chạy toàn bộ các bài test, thực hiện lệnh sau:
```bash
dotnet test
```

---

## Cấu Trúc Thư Mục Dự Án

```text
SRT_group_todo_list/
│
├── SRT_group_todo_list/            # Dự án Web chính (ASP.NET Core MVC)
│   ├── Controllers/                # Controllers xử lý HTTP Requests & AJAX
│   ├── Models/                     # Lớp dữ liệu (TodoItem) & DbContext
│   ├── Services/                   # Lớp nghiệp vụ (Business Logic Service)
│   ├── Migrations/                 # Quản lý lịch sử thay đổi cấu trúc database
│   ├── Views/                      # Giao diện hiển thị (Razor Views)
│   └── wwwroot/                    # Các file tĩnh (CSS, JS, Libs)
│
├── SRT_group_todo_list.Tests/      # Dự án Unit Tests (xUnit)
│
├── Dockerfile                      # Cấu hình build Docker image
├── docker-compose.yml              # Cấu hình container orchestrator
└── SRT_group_todo_list.sln         # File Solution của dự án
```
