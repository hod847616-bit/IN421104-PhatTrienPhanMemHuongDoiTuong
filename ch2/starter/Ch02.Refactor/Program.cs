using EduHub.Ch02.Logging;
using EduHub.Ch02.Notifications;
using EduHub.Ch02.Patterns.Observers;
using EduHub.Ch02.Patterns.Strategies;
using EduHub.Ch02.Repositories;
using EduHub.Ch02.Services;

// Composition Root: nơi duy nhất khởi tạo các implementation cụ thể
IAppLogger logger = new ConsoleLogger();
IClubRepository clubRepository = new LoggingClubRepository(new InMemoryClubRepository(), logger);
IMemberRepository memberRepository = new InMemoryMemberRepository();
IEventRepository eventRepository = new InMemoryEventRepository();
INotificationServiceFactory notificationFactory = new NotificationServiceFactory();

IClubService clubService = new ClubService(clubRepository, memberRepository, logger, notificationFactory);
IEventService eventService = new EventService(clubRepository, eventRepository, logger);
IReportService reportService = new ReportService(clubRepository, memberRepository, logger);

// Observer registration
eventService.Subscribe(new LogNotifier(logger));
eventService.Subscribe(new EmailNotifier(memberRepository, notificationFactory.Create(NotificationChannel.Email)));

Console.WriteLine("=== 1. Create Club & Add Members ===");
var techClub = clubService.CreateClub("Tech Club", "Câu lạc bộ công nghệ", "president@gmail.com");
clubService.AddMember(techClub.Id, "Nguyễn Văn A", "a@student.dthu.edu.vn");
clubService.AddMember(techClub.Id, "Trần Thị B", "b@student.dthu.edu.vn");

Console.WriteLine("\n=== 2. Create Event (Observer Pattern) ===");
eventService.CreateEvent(techClub.Id, "Workshop SOLID", new DateTime(2026, 11, 20));

Console.WriteLine("\n=== 3. Reports (Strategy Pattern) ===");
Console.WriteLine("--- Console Report ---");
Console.WriteLine(reportService.Generate(new ConsoleReportStrategy()));
Console.WriteLine("--- CSV Report ---");
Console.WriteLine(reportService.Generate(new CsvReportStrategy()));

Console.WriteLine("\n=== 4. Factory Pattern ===");
notificationFactory.Create(NotificationChannel.Sms).Send("0901234567", "Nhắc lịch họp CLB lúc 18:00");
