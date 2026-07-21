# Checklist bảo mật tuần 7

Mình dùng checklist này để rà lại phần Auth trước khi kết thúc tuần 7. Những mục đã đánh dấu là các phần đã có trong project và đã test ở môi trường local.

## Tài khoản và mật khẩu

- [x] Mật khẩu được hash bằng `PasswordHasher<User>`, database không lưu mật khẩu gốc.
- [x] API không trả `PasswordHash` trong response đăng ký, đăng nhập hoặc danh sách user.
- [x] Khi đăng nhập sai, email không tồn tại và mật khẩu không đúng đều nhận cùng một thông báo.
- [x] Email được chuẩn hóa trước khi đăng ký và đăng nhập.
- [x] Database có unique index trên email để tránh tạo hai tài khoản trùng nhau.

## Access token

- [x] Access token có thời hạn ngắn, thời gian sống lấy từ cấu hình.
- [x] Khi nhận token, server kiểm tra issuer, audience, chữ ký và thời gian hết hạn.
- [x] `ClockSkew` đang để bằng 0 nên token hết hạn đúng theo thời gian đã cấu hình.
- [x] Token chỉ chứa các thông tin đang cần dùng: user id, email và role.
- [x] JWT secret không ghi trong source code, môi trường local đọc từ User Secrets.
- [x] API cần đăng nhập trả `401` nếu thiếu token hoặc token không hợp lệ.
- [x] User đã đăng nhập nhưng không đúng role sẽ nhận `403`.

## Phân quyền API

- [x] Xem Product và Category vẫn là public.
- [x] Admin và Staff được tạo, cập nhật Product và Category.
- [x] Chỉ Admin được xóa hoặc khôi phục Product.
- [x] Chỉ Admin được xóa Category.
- [x] Chỉ Admin được xem danh sách user.
- [x] Đã test lại bằng ba tài khoản Admin, Staff và User.

## Refresh token và logout

- [x] Refresh token được tạo bằng `RandomNumberGenerator`.
- [x] Database chỉ lưu SHA-256 hash của refresh token, không lưu token gốc.
- [x] Cột token hash có unique index.
- [x] Refresh token được kiểm tra tồn tại, thời hạn và trạng thái thu hồi trước khi cấp token mới.
- [x] Mỗi lần refresh thành công, token cũ bị thu hồi và được thay bằng token mới.
- [x] Token cũ sau rotation không dùng lại được.
- [x] Logout thu hồi refresh token của phiên hiện tại.
- [x] Token không tồn tại, hết hạn hoặc đã bị thu hồi đều bị từ chối với `401`.

## Secret và log

- [x] Connection string và JWT secret không nằm trong `appsettings.json` đã commit.
- [x] `.env`, `appsettings.Local.json` và các file local của khóa học đã được thêm vào `.gitignore`.
- [x] README chỉ dùng giá trị ví dụ, không có mật khẩu hoặc secret thật.
- [x] Swagger chỉ bật khi chạy môi trường Development.
- [x] Request log hiện chỉ ghi method, path, status code và thời gian xử lý; không ghi request body hoặc Authorization header.
- [x] Khi có lỗi, client không nhận stack trace.

## Trước khi đưa lên production

Các mục dưới đây chưa cần cho bài local, nhưng phải làm nếu API được triển khai thật:

- [ ] Bắt buộc dùng HTTPS và cấu hình đúng reverse proxy.
- [ ] Chọn nơi lưu refresh token an toàn ở phía client. Với web app nên ưu tiên HttpOnly, Secure, SameSite cookie.
- [ ] Thêm rate limiting cho register, login, refresh và logout.
- [ ] Theo dõi số lần đăng nhập sai và cân nhắc khóa tạm thời tài khoản.
- [ ] Có job dọn refresh token đã hết hạn hoặc bị thu hồi lâu ngày.
- [ ] Thu hồi các phiên đang hoạt động khi đổi mật khẩu, khóa tài khoản hoặc phát hiện token bị lộ.
- [ ] Không ghi access token, refresh token, mật khẩu hoặc connection string vào hệ thống log.
- [ ] Quản lý secret bằng công cụ của môi trường triển khai và có kế hoạch thay secret định kỳ.
- [ ] Cấu hình CORS theo đúng danh sách frontend được phép truy cập.
- [ ] Bổ sung các security header phù hợp ở ứng dụng hoặc reverse proxy.

## Ghi chú về logout

Logout hiện tại chỉ thu hồi refresh token. Access token đã cấp vẫn dùng được cho đến khi hết hạn, vì vậy thời gian sống của access token nên để ngắn. Nếu sau này cần khóa access token ngay lập tức thì phải bổ sung thêm cơ chế như token version, deny list hoặc kiểm tra phiên ở server.
