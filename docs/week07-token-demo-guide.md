# Kịch bản demo Auth và token

Kịch bản này dùng để quay video khoảng 3-5 phút bằng Postman. Mục tiêu là cho thấy luồng đăng nhập, phân quyền và refresh token chạy đúng; không cần giải thích toàn bộ source code trong video.

## Chuẩn bị trước khi quay

- Chạy API ở môi trường Development.
- Import collection `postman/ProductApiAuth.postman_collection.json`.
- Kiểm tra biến `baseUrl` trỏ đúng địa chỉ API đang chạy.
- Chuẩn bị sẵn tài khoản Admin và Staff trong collection.
- Xóa kết quả cũ trong Postman Console nếu muốn màn hình dễ nhìn.
- Không mở hoặc đọc JWT secret, password hash hay refresh token đầy đủ trong video.

Nếu quay bằng API local hiện tại, có thể đặt:

```text
baseUrl = http://127.0.0.1:5293
```

## Thứ tự demo đề xuất

### 1. Giới thiệu ngắn

Nói trong khoảng 15-20 giây:

> Đây là Product API sau tuần 7. API có đăng ký, đăng nhập bằng JWT, phân quyền Admin/Staff/User, refresh-token rotation và logout. Em sẽ chạy các request chính bằng Postman.

### 2. Register tài khoản mới

Mở folder `Day 05 - Auth Regression`, chạy request `01 - Register New User`.

Điểm cần chỉ trên màn hình:

- Status là `201 Created`.
- Email được tạo riêng cho lần chạy.
- Role mặc định là `User`.
- Response không có password hoặc password hash.

Có thể giải thích:

> Khi đăng ký, password được hash trước khi lưu. Tài khoản mới luôn nhận role User, client không được tự chọn Admin hay Staff.

### 3. Login và lấy token

Chạy `02 - Login New User`.

Điểm cần chỉ:

- Status là `200 OK`.
- Response có access token và refresh token.
- Script đã lưu access token vào collection variable để request sau dùng lại.

Không cần phóng to hoặc đọc toàn bộ giá trị token. Chỉ cần cho thấy hai trường token tồn tại.

Có thể giải thích:

> Access token dùng để gọi API cần đăng nhập và có thời gian sống ngắn. Refresh token dùng để xin cặp token mới khi access token hết hạn.

### 4. Đọc thông tin từ access token

Chạy `03 - Get Current User`.

Điểm cần chỉ:

- Request gửi Bearer token từ biến `day05UserToken`.
- Status là `200 OK`.
- Response trả đúng `userId`, `email` và role `User`.

Có thể giải thích:

> Endpoint `/api/auth/me` đọc các claim trong JWT. Server vẫn kiểm tra chữ ký, issuer, audience và thời hạn trước khi cho request đi tiếp.

### 5. Demo `401` và `403`

Chạy lần lượt:

1. `08 - Create Category Without Token - 401`.
2. `09 - User Create Category - 403`.
3. `10 - Staff Create Category - 201`.

Điểm cần nói rõ:

- `401` nghĩa là request chưa có danh tính hợp lệ.
- `403` nghĩa là đã đăng nhập nhưng role không đủ quyền.
- Staff được tạo Category nên request thứ ba trả `201`.

Có thể giải thích:

> Cùng là request tạo Category nhưng kết quả khác nhau theo trạng thái đăng nhập và role. Không có token nhận 401, User có token nhưng thiếu quyền nhận 403, còn Staff được phép nên tạo thành công.

### 6. Demo quyền Admin với Product restore

Tiếp tục chạy các request từ `13 - Staff Create Product - 201` đến `18 - Admin Restore Product - 200`.

Tập trung vào bốn request restore:

- Không token: `401`.
- User: `403`.
- Staff: `403`.
- Admin: `200`.

Có thể giải thích:

> Khôi phục Product đã xóa mềm là thao tác nhạy cảm nên chỉ Admin được thực hiện. API kiểm tra role trước khi chạy phần nghiệp vụ restore.

Sau đó chạy hai request cleanup để dọn Product và Category thử nghiệm.

### 7. Demo refresh-token rotation

Mở folder `Day 04 - Refresh Token`, chạy từng request theo thứ tự.

#### Login

Chạy `01 - Login` và chỉ ra rằng collection đã lưu refresh token hiện tại.

#### Refresh và xoay token

Chạy `02 - Refresh And Rotate Token`.

Điểm cần chỉ:

- Status là `200 OK`.
- Access token mới được trả về.
- Refresh token mới khác refresh token cũ.

Có thể giải thích:

> Khi refresh thành công, token cũ bị revoke và được thay bằng token mới. Database chỉ lưu hash của refresh token, không lưu raw token.

#### Thử dùng lại token cũ

Chạy `03 - Reuse Rotated Token - 401`.

Điểm cần chỉ:

- Status là `401 Unauthorized`.
- Message cho biết refresh token đã bị revoke.

Có thể giải thích:

> Token cũ không thể dùng lần thứ hai. Đây là phần rotation giúp giảm rủi ro khi một refresh token cũ bị lộ.

### 8. Demo logout

Chạy:

1. `04 - Logout`.
2. `05 - Refresh After Logout - 401`.

Điểm cần chỉ:

- Logout trả `200 OK`.
- Refresh token vừa logout bị từ chối với `401`.

Có thể giải thích:

> Logout hiện tại thu hồi refresh token. Access token đã cấp không bị xóa ngay và vẫn dùng được đến khi hết hạn, vì JWT access token đang hoạt động theo kiểu stateless.

## Kết thúc video

Nói trong khoảng 15 giây:

> Qua flow này, register, login, JWT claims, phân quyền theo role, refresh-token rotation và logout đều đã được kiểm tra. Collection cũng có các assertion cho status code, role, token và cleanup dữ liệu test.

## Các điểm nên tránh khi quay

- Không quay màn hình có JWT secret hoặc connection string.
- Không đọc password thật hoặc token đầy đủ.
- Không nói logout làm access token hết hiệu lực ngay lập tức, vì project hiện tại chỉ revoke refresh token.
- Không dùng lẫn `401` và `403`.
- Không bỏ qua request cleanup nếu vừa tạo Category và Product test.
- Không chạy request ngoài thứ tự trong folder refresh token vì các request phụ thuộc token của bước trước.

## Checklist trước khi nộp video

- [ ] Video dài khoảng 3-5 phút.
- [ ] Có register, login và `/api/auth/me`.
- [ ] Có ví dụ `401` và `403`.
- [ ] Có ít nhất một thao tác thành công của Staff hoặc Admin.
- [ ] Có refresh-token rotation.
- [ ] Có thử dùng lại token cũ và nhận `401`.
- [ ] Có logout và thử refresh lại.
- [ ] Không để lộ secret hoặc thông tin nhạy cảm.
- [ ] Giải thích đúng rằng logout chỉ revoke refresh token trong phiên bản hiện tại.
