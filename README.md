# ProductApiAuth

Product API xây dựng bằng ASP.NET Core 8, Entity Framework Core và PostgreSQL. Repo này dùng để làm phần Authentication và phân quyền của tuần 7.

## Day 01 - User và đăng ký tài khoản

- User entity gồm `Id`, `FullName`, `Email`, `PasswordHash`, `Role`, `CreatedAt`.
- Role nền tảng: `Admin`, `Staff`, `User`.
- Email không phân biệt hoa thường và có unique index trong PostgreSQL.
- `RegisterDto`, `LoginDto`, `AuthResponseDto`.
- Hash và verify password bằng `PasswordHasher<User>`.
- API `POST /api/auth/register`.
- Chặn email trùng với status `409 Conflict`.
- Validate password từ 6 đến 100 ký tự với status `400 Bad Request`.
- Response đăng ký không trả password hoặc password hash.

## Day 02 - Đăng nhập và JWT

- API `POST /api/auth/login` kiểm tra email và password đã hash.
- Login thành công trả access token và thời điểm token hết hạn.
- JWT có các claims `userId`, `email`, `role`.
- Token được kiểm tra issuer, audience, chữ ký và thời hạn.
- Swagger có nút Authorize để nhập Bearer token.
- API `GET /api/auth/me` đọc thông tin user từ token và yêu cầu đăng nhập.
- Email không tồn tại hoặc password sai đều trả `401 Unauthorized` với cùng một thông báo.

## Công nghệ

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
- PostgreSQL
- Swagger / OpenAPI
- JWT Bearer Authentication

## Cấu hình database

Connection string không để trong source. Project đọc cấu hình từ .NET User Secrets với key:

```text
ConnectionStrings:DefaultConnection
```

Thiết lập trên máy local:

```cmd
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=ProductApiDb;Username=postgres;Password=YOUR_PASSWORD" --project src/ProductApi/ProductApi.csproj
```

## Cấu hình JWT

JWT cũng được đọc từ .NET User Secrets. Có thể dùng các giá trị local sau và tự thay secret bằng chuỗi ngẫu nhiên dài ít nhất 32 ký tự:

```cmd
dotnet user-secrets set "Jwt:Issuer" "ProductApi" --project src/ProductApi/ProductApi.csproj
dotnet user-secrets set "Jwt:Audience" "ProductApiClient" --project src/ProductApi/ProductApi.csproj
dotnet user-secrets set "Jwt:AccessTokenMinutes" "15" --project src/ProductApi/ProductApi.csproj
dotnet user-secrets set "Jwt:Secret" "YOUR_RANDOM_SECRET_AT_LEAST_32_CHARACTERS" --project src/ProductApi/ProductApi.csproj
```

## Khởi tạo database

```cmd
dotnet ef database update --project src/ProductApi/ProductApi.csproj --startup-project src/ProductApi/ProductApi.csproj
```

Migration Day 01 tạo bảng `Users` với unique index trên email.

## Chạy API

```cmd
dotnet run --project src/ProductApi/ProductApi.csproj
```

Sau khi chạy, mở Swagger theo địa chỉ hiện trong terminal. Ví dụ:

```text
http://localhost:5291/swagger
```

## Đăng ký tài khoản

Endpoint:

```http
POST /api/auth/register
Content-Type: application/json
```

Request hợp lệ:

```json
{
  "fullName": "Nguyen Van A",
  "email": "nguyenvana@example.com",
  "password": "Secret123!"
}
```

Nếu thành công API trả `201 Created`, role mặc định là `User`.

Một số trường hợp lỗi:

- Email đã tồn tại: `409 Conflict`.
- Password ngắn hơn 6 ký tự: `400 Bad Request`.
- Email sai định dạng hoặc thiếu trường bắt buộc: `400 Bad Request`.

## Đăng nhập

Endpoint:

```http
POST /api/auth/login
Content-Type: application/json
```

Request:

```json
{
  "email": "nguyenvana@example.com",
  "password": "Secret123!"
}
```

Login thành công trả `200 OK`. Lấy giá trị `data.accessToken`, bấm **Authorize** trong Swagger rồi dán token vào ô xác thực. Swagger sẽ tự gửi header Bearer cho các request tiếp theo.

Sau khi authorize, gọi:

```http
GET /api/auth/me
```

Endpoint này trả `userId`, `email`, `role` từ token. Nếu không có token, token không hợp lệ hoặc đã hết hạn thì API trả `401 Unauthorized`.

## Test bằng Postman

Import file `postman/ProductApiAuth.postman_collection.json` vào Postman. Collection có sẵn các biến `baseUrl`, `email`, `password`, `accessToken`.

Chạy lần lượt:

1. `Register` để tạo tài khoản test.
2. `Login` để nhận JWT. Script của request sẽ tự lưu `data.accessToken` vào biến `accessToken`.
3. `Get Current User` để gọi `/api/auth/me` bằng token vừa lưu.

Login trả JWT:

![Login trả JWT](docs/images/day02-login.png)

Token được dùng lại để đọc thông tin user:

![Đọc thông tin user từ token](docs/images/day02-current-user.png)

## Kết quả kiểm tra

- Đăng ký hợp lệ trả `201`.
- Email trùng, kể cả khác chữ hoa/thường, trả `409`.
- Password 5 ký tự trả `400`.
- Trong database chỉ lưu `PasswordHash`, không lưu mật khẩu gốc.
- Login đúng trả `200`, access token và expiration.
- Login sai password hoặc email không tồn tại trả `401`.
- `/api/auth/me` trả đúng `userId`, `email`, `role` khi token hợp lệ.
- `/api/auth/me` không có token hoặc dùng token sai trả `401`.
- JWT có đúng issuer, audience, expiration và các claims đã cấu hình.
- Swagger hiển thị Bearer scheme và hai endpoint login/me.
- Postman tự lưu access token sau khi login và dùng lại cho `/api/auth/me`.

## Lưu ý

- Không commit connection string, password database, token hoặc JWT secret.
- API response không trả `PasswordHash`.
- Email được chuyển về chữ thường trước khi lưu.
- Email trùng được kiểm tra ở service và unique constraint của database.
- Access token hiện có thời hạn mặc định 15 phút và chưa có refresh token.
