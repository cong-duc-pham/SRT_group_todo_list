# Hướng Dẫn Triển Khai Lên Render.com (Miễn phí & Không cần Thẻ tín dụng)

Do **Fly.io** hiện tại yêu cầu liên kết thẻ tín dụng (Credit Card) để xác minh danh tính ngay cả với tài khoản miễn phí, **Render.com** là lựa chọn thay thế tốt nhất:
- **Hoàn toàn miễn phí** (Free Tier).
- **Không yêu cầu thẻ tín dụng**.
- Tự động đồng bộ và build lại mỗi khi bạn push code mới lên GitHub.

---

## ⚠️ Lưu Ý Về Dữ Liệu Trên Render (Free Tier)
Ở gói Free của Render, hệ thống **không hỗ trợ** lưu trữ ổ đĩa vĩnh viễn (Persistent Disk). Do đó, file database SQLite (`todo.db`) sẽ ở trạng thái tạm thời (ephemeral):
- Khi bạn thao tác Thêm/Sửa/Xóa, ứng dụng vẫn hoạt động bình thường.
- Tuy nhiên, nếu ứng dụng không có lượt truy cập trong vòng 15 phút, Render sẽ tạm thời cho container "ngủ" để tiết kiệm tài nguyên. Khi có người truy cập lại, container sẽ khởi động lại và cơ sở dữ liệu sẽ quay về trạng thái ban đầu (trống).
- **Đối với bài test tuyển dụng/đánh giá kỹ thuật, điều này hoàn toàn được chấp nhận** vì người chấm thi chỉ cần truy cập vào link để click dùng thử tính năng trực tiếp.

---

## Các Bước Triển Khai Chi Tiết

### Bước 1: Push toàn bộ mã nguồn lên GitHub
Đảm bảo tất cả các file cấu hình Docker (`Dockerfile`, `docker-compose.yml`) đã được push lên repo của bạn trên GitHub (nhánh `develop` hoặc `main`).

---

### Bước 2: Tạo tài khoản và liên kết với GitHub
1. Truy cập trang web: [https://render.com/](https://render.com/)
2. Chọn **Sign Up** và đăng ký bằng tài khoản **GitHub** của bạn (để Render tự động lấy danh sách repo).

---

### Bước 3: Tạo Web Service mới trên Render
1. Tại trang quản trị (Dashboard) của Render, nhấn nút **New +** ở góc trên bên phải và chọn **Web Service**.
2. Tại phần **Connect a repository**, bạn sẽ thấy danh sách các kho lưu trữ GitHub của mình. 
3. Tìm repo `SRT_group_todo_list` và nhấn nút **Connect** bên cạnh.

---

### Bước 4: Cấu hình thông số Web Service
Thiết lập các thông số cấu hình như sau:

- **Name**: Nhập tên ứng dụng của bạn (ví dụ: `srt-todo-app` hoặc bất kỳ tên nào chưa bị trùng).
- **Region**: Chọn **Singapore (Southeast Asia)** để có tốc độ truy cập từ Việt Nam nhanh nhất.
- **Branch**: Chọn nhánh chứa code hoàn thiện nhất của bạn (ví dụ: `develop` hoặc `main`).
- **Runtime**: Chọn **Docker** (Render sẽ tự động đọc cấu hình từ file `Dockerfile` của dự án để build).
- **Instance Type**: Chọn gói **Free** ($0/month).

---

### Bước 5: Khởi tạo và theo dõi quá trình deploy
1. Nhấn nút **Create Web Service** ở cuối trang.
2. Render sẽ bắt đầu kéo code từ GitHub về, build Docker image và khởi chạy container. Bạn có thể theo dõi trực tiếp các dòng log hiển thị trên màn hình.
3. Quá trình build và start app thường mất khoảng **2 - 4 phút**.

---

### Bước 6: Lấy link truy cập
Khi thấy dòng log hiển thị chữ `Live` màu xanh lá cây hoặc thông báo deploy thành công, hãy nhìn lên góc trên bên trái màn hình giao diện Render. Bạn sẽ thấy đường dẫn ứng dụng của mình dạng:
```text
https://ten-app-cua-ban.onrender.com
```

Bạn có thể copy đường dẫn này gửi cho người đánh giá hoặc chèn vào file `README.md`.
