# Phân tích vi phạm SOLID trong `ClubManager`

Tài liệu này phân tích các vi phạm nguyên tắc SOLID trong lớp `ClubManager` (`Ch02.Refactor/ClubManager.cs`) và đề xuất hướng refactor.

## 1. Single Responsibility Principle (SRP)

- **Vị trí:** Toàn bộ lớp `ClubManager` (dòng 28–117), đặc biệt là `CreateClub` (dòng 35–64), `AddMemberToClub` (dòng 67–84), `CreateEvent` (dòng 88–99), `PrintClubReport` (dòng 103–113).
- **Vi phạm:** `ClubManager` là một *God class* đảm nhiệm quá nhiều trách nhiệm không liên quan trực tiếp với nhau:
  1. Kiểm tra dữ liệu đầu vào (validation tên CLB, sức chứa CLB).
  2. Lưu trữ dữ liệu (`_clubs`, `_members`).
  3. Ghi log ra file (`File.AppendAllText`).
  4. Gửi thông báo qua nhiều kênh (`Console.WriteLine`).
  5. Quản lý sự kiện và định dạng báo cáo.
- **Hậu quả:** Bất kỳ thay đổi nào (đổi cách log, đổi định dạng báo cáo, đổi rule nghiệp vụ) đều buộc phải sửa `ClubManager`, dễ gây lỗi dây chuyền và rất khó viết unit test cô lập từng phần.
- **Hướng sửa:** Tách thành `ClubService`, `EventService`, `ReportService`, các `IRepository`, `IAppLogger`, `INotificationService`, và đưa quy tắc nghiệp vụ về `Domain/Club.cs`, `Domain/Event.cs`.

## 2. Open/Closed Principle (OCP)

- **Vị trí:**
  - `CreateClub` (dòng 52–63): chuỗi `if / else if / else` kiểm tra `@gmail.com`, `@yahoo.com`.
  - `AddMemberToClub` (dòng 82–83): gọi cứng email và Slack.
  - `CreateEvent` (dòng 96–97): vòng lặp gửi email cứng trong hàm tạo sự kiện.
  - `PrintClubReport` (dòng 103–113): in báo cáo theo một định dạng cố định ra Console.
- **Vi phạm:** Khi muốn thêm kênh thông báo mới (SMS, Push) hoặc thêm định dạng báo cáo mới (CSV, JSON) hoặc thêm hành động phản ứng khi sự kiện được tạo, lập trình viên phải sửa trực tiếp mã nguồn của `ClubManager`.
- **Hậu quả:** Code dễ phát sinh bug hồi quy trên các luồng đang chạy ổn định, khó mở rộng.
- **Hướng sửa:**
  - Dùng **Factory** (`INotificationServiceFactory`) + `INotificationService` cho kênh thông báo.
  - Dùng **Strategy** (`IReportStrategy`) cho định dạng báo cáo.
  - Dùng **Observer** (`IEventObserver`) cho các hành động khi tạo sự kiện.

## 3. Dependency Inversion Principle (DIP)

- **Vị trí:**
  - Dòng 48, 79, 94, 111: gọi trực tiếp `File.AppendAllText(_logFilePath, ...)`.
  - Dòng 54, 58, 62, 82, 83, 97, 108: gọi trực tiếp `Console.WriteLine(...)`.
  - Dòng 30–31: khởi tạo trực tiếp `new List<Club>()`, `new List<Member>()` bên trong class.
- **Vi phạm:** Module nghiệp vụ cấp cao (`ClubManager`) phụ thuộc trực tiếp vào chi tiết cài đặt cấp thấp (hệ thống file `System.IO.File`, `Console`, cấu trúc lưu trữ `List<T>`).
- **Hậu quả:** Không thể thay thế cách lưu trữ (sang database), không thể đổi nơi ghi log (sang Console/Serilog), và không thể mock dependency khi viết unit test.
- **Hướng sửa:** Phụ thuộc vào abstraction (`IClubRepository`, `IMemberRepository`, `IEventRepository`, `IAppLogger`, `INotificationService`) và nhận qua constructor (Constructor Injection).

## 4. Interface Segregation Principle (ISP)

- **Vị trí:** Toàn bộ `ClubManager` (dòng 28–117) và `GetAllClubs()` (dòng 116).
- **Vi phạm:** Mọi chức năng (quản lý CLB, thành viên, sự kiện, báo cáo) bị gom chung vào một API duy nhất, đồng thời `GetAllClubs()` trả về trực tiếp `List<Club>` nội bộ. Một client chỉ cần in báo cáo vẫn phải phụ thuộc vào toàn bộ `ClubManager` chứa cả hàm tạo sự kiện và thêm thành viên.
- **Hậu quả:** Các thành phần phụ thuộc vào nhiều phương thức mà chúng không sử dụng, tăng mức độ ghép nối (coupling).
- **Hướng sửa:** Chia nhỏ thành các interface chuyên biệt theo vai trò: `IClubRepository`, `IMemberRepository`, `IEventRepository`, `IClubService`, `IEventService`, `IReportService`.
