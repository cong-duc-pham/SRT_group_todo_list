# Hướng Dẫn Triển Khai Lên Fly.io

Tài liệu này hướng dẫn chi tiết cách deploy ứng dụng Todo List lên **Fly.io** với database SQLite được lưu trữ trên Volume vĩnh viễn (Persistent Volume).

---

## Bước 1: Cài đặt Fly CLI (Giao diện dòng lệnh)

Mở **PowerShell** (nếu dùng Windows) hoặc **Terminal** và chạy lệnh sau để cài đặt:

### Trên Windows (PowerShell):
```powershell
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12; iwr https://fly.io/install.ps1 -useb | iex
```
*Lưu ý: Sau khi cài đặt xong, bạn có thể cần khởi động lại Terminal/PowerShell để hệ thống nhận diện lệnh `fly`.*

### Trên macOS / Linux:
```bash
curl -L https://fly.io/install.sh | sh
```

---

## Bước 2: Đăng ký / Đăng nhập tài khoản Fly.io

1. Nếu chưa có tài khoản, chạy lệnh sau để đăng ký (sẽ mở trình duyệt):
   ```bash
   fly auth signup
   ```
2. Nếu đã có tài khoản, thực hiện đăng nhập:
   ```bash
   fly auth login
   ```

---

## Bước 3: Khởi tạo ứng dụng Fly.io (fly launch)

Tại thư mục gốc của dự án (nơi có file `Dockerfile`), chạy lệnh:
```bash
fly launch
```

Fly CLI sẽ tự động quét thư mục, nhận diện `Dockerfile` và hỏi bạn một số thông tin:
1. **Choose an app name**: Nhập tên ứng dụng của bạn (ví dụ: `srt-todo-app-yourname`). Tên này phải là duy nhất.
2. **Choose a platform**: Chọn mặc định.
3. **Select Organization**: Chọn Personal.
4. **Choose a region**: Chọn khu vực gần Việt Nam nhất để ứng dụng tải nhanh, ví dụ: **Hong Kong (hkg)** hoặc **Singapore (sin)**.
5. **Would you like to set up a PostgreSQL database?**: Chọn **No** (vì chúng ta dùng SQLite).
6. **Would you like to set up an Upstash Redis database?**: Chọn **No**.
7. **Would you like to deploy now?**: Chọn **No** (vì chúng ta cần cấu hình Volume lưu trữ SQLite trước khi deploy).

*Sau lệnh này, Fly.io sẽ tự động sinh ra một file cấu hình tên là `fly.toml` trong thư mục dự án.*

---

## Bước 4: Cấu hình Volume vĩnh viễn cho SQLite

Để dữ liệu không bị mất khi ứng dụng khởi động lại, chúng ta cần tạo một ổ đĩa ảo (Volume) trên Fly.io.

1. **Tạo Volume trên Fly.io**:
   Chạy lệnh sau để tạo một volume có dung lượng 1GB (miễn phí) tại cùng khu vực bạn đã chọn ở Bước 3 (ví dụ `hkg` hoặc `sin`):
   ```bash
   fly volume create todo_data --region hkg --size 1
   ```
   *(Thay `hkg` bằng region bạn đã chọn).*

2. **Cấu hình gắn Volume vào ứng dụng**:
   Mở file `fly.toml` vừa được tạo ra ở Bước 3 bằng trình soạn thảo (VS Code / Notepad) và thêm đoạn cấu hình sau vào **cuối file**:
   ```toml
   [[mounts]]
     source = "todo_data"
     destination = "/app/data"
   ```
   *Cấu hình này yêu cầu Fly.io gắn volume `todo_data` vào thư mục `/app/data` bên trong Docker container (đúng thư mục chứa SQLite database).*

---

## Bước 5: Deploy ứng dụng lên Online!

Bây giờ bạn chỉ cần chạy lệnh cuối cùng để đẩy code lên và khởi chạy:
```bash
fly deploy
```

Quá trình này sẽ:
- Đóng gói code và gửi lên máy chủ Fly.io.
- Thực hiện build Docker image online.
- Khởi chạy Container và gắn Volume chứa database SQLite.

---

## Bước 6: Truy cập ứng dụng

Sau khi quá trình deploy hoàn tất, Fly CLI sẽ in ra địa chỉ truy cập ứng dụng của bạn dưới dạng:
```text
https://<tên-app-của-bạn>.fly.dev
```

Chúc mừng! Ứng dụng Todo List của bạn đã online vĩnh viễn và lưu trữ dữ liệu an toàn.
