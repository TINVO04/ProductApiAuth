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

## Công nghệ

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
- PostgreSQL
- Swagger / OpenAPI

## Cấu hình database

Connection string không để trong source. Project đọc cấu hình từ .NET User Secrets với key:

```text
ConnectionStrings:DefaultConnection
```

Thiết lập trên máy local:

```cmd
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=ProductApiDb;Username=postgres;Password=YOUR_PASSWORD" --project src/ProductApi/ProductApi.csproj
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

## Kết quả kiểm tra

- Đăng ký hợp lệ trả `201`.
- Email trùng, kể cả khác chữ hoa/thường, trả `409`.
- Password 5 ký tự trả `400`.
- Trong database chỉ lưu `PasswordHash`, không lưu mật khẩu gốc.
- Build Release thành công, không có warning và error.

## Lưu ý

- Không commit connection string, password database, token hoặc secret.
- API response không trả `PasswordHash`.
- Email được chuyển về chữ thường trước khi lưu.
- Email trùng được kiểm tra ở service và unique constraint của database.
