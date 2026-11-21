# Ch03 — Clean Architecture + REST API: EduHub Skeleton

Hoàn thành Bài tập Chương 3 (EduHub Skeleton — HĐ1.3) theo mô hình Clean Architecture 4 lớp và REST API với ASP.NET Core + Swagger + xUnit/Moq.

## Cấu trúc 4 lớp & chiều tham chiếu

```
EduHub.Domain/          Entity (Club, Member) + interface repository (không phụ thuộc lớp nào)
EduHub.Application/     Handler của từng use case (phụ thuộc Domain)
EduHub.Infrastructure/  InMemoryClubRepository, InMemoryMemberRepository (phụ thuộc Application)
EduHub.API/             Controllers (ClubsController, MembersController), Swagger, DI (phụ thuộc Application + Infrastructure)
EduHub.Tests/           Unit test lớp Application, mock repository bằng Moq
```

## Nội dung đã hoàn thành

| TODO | Tệp | Nội dung |
|---|---|---|
| 1a–1e | `EduHub.Infrastructure/Repositories/InMemoryClubRepository.cs` | Cài đặt 5 phương thức `GetAllAsync`, `GetByIdAsync`, `AddAsync`, `UpdateAsync`, `DeleteAsync` |
| 2 | `EduHub.Application/Clubs/GetClubsQuery.cs` | `GetClubsQueryHandler.HandleAsync` gọi `_repository.GetAllAsync(ct)` |
| 3 | `EduHub.Application/Clubs/GetClubByIdQuery.cs` | `GetClubByIdQueryHandler.HandleAsync` gọi `_repository.GetByIdAsync(query.Id, ct)` |
| 4a–4d | `EduHub.Application/Clubs/CreateClubCommand.cs` | Kiểm tra `Name` không rỗng, `<= 50` ký tự, tạo `Club.Create(...)` và lưu vào repository |
| 5 | `EduHub.Application/Clubs/DeleteClubCommand.cs` | `DeleteClubCommandHandler.HandleAsync` gọi `_repository.DeleteAsync(command.Id, ct)` |
| 6a–6c | `EduHub.API/Program.cs` | Đăng ký DI (`AddScoped`) cho `IClubRepository`, `IMemberRepository` và 4 handler |
| 7 | `EduHub.API/Program.cs` | Cấu hình SwaggerDoc `"v1"` với `Title = "EduHub API"` |
| 8a–8i | `EduHub.API/Controllers/ClubsController.cs` | Cài đặt 4 endpoint `GET`, `GET {id}`, `POST`, `DELETE` với đúng HTTP status code (`200`, `201`, `204`, `400`, `404`) |
| Test 5 | `EduHub.Tests/Clubs/CreateClubCommandTests.cs` | Giữ nguyên 4 test có sẵn và bổ sung `GetClubByIdQueryTests.GetClubById_WhenExists_ReturnsClub` dùng Moq |
| Bonus | `EduHub.API/Controllers/MembersController.cs` | Cài đặt thêm `MembersController` (`/api/members`) với `GET`, `GET {id}`, `POST`, `DELETE` |

## API Endpoints

### Clubs (`/api/clubs`)
- `GET /api/clubs` — Lấy danh sách câu lạc bộ (`200 OK`)
- `GET /api/clubs/{id}` — Lấy câu lạc bộ theo Id (`200 OK` / `404 Not Found`)
- `POST /api/clubs` — Tạo câu lạc bộ mới (`201 Created` / `400 Bad Request`)
- `DELETE /api/clubs/{id}` — Xoá câu lạc bộ (`204 No Content` / `404 Not Found`)

### Members (`/api/members`, bonus)
- `GET /api/members` — Lấy danh sách thành viên (`200 OK`)
- `GET /api/members/{id}` — Lấy thành viên theo Id (`200 OK` / `404 Not Found`)
- `POST /api/members` — Thêm thành viên mới (`201 Created` / `400 Bad Request`)
- `DELETE /api/members/{id}` — Xoá thành viên (`204 No Content` / `404 Not Found`)

## Cách build, test và chạy

```bash
dotnet build EduHub.sln
dotnet test EduHub.sln
dotnet run --project EduHub.API   # http://localhost:5000/swagger
```

## Quy ước nộp bài (Git / Pull Request)

- Tạo nhánh riêng từ `main` (ví dụ: `feature/ch03-skeleton`).
- Commit theo chuẩn Conventional Commits:
  - `feat(club): implement InMemoryClubRepository, handlers, DI, Swagger and ClubsController`
  - `test(club): add GetClubByIdQuery unit test with Moq`
  - `feat(member): add MembersController endpoints`
  - `docs: update README.md`
- Tạo Pull Request về `main`, mô tả rõ **What**, **Why**, và **How to test**.
