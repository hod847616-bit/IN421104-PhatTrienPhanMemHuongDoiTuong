using EduHub.Ch02.Domain;
using EduHub.Ch02.Logging;
using EduHub.Ch02.Notifications;
using EduHub.Ch02.Patterns.Observers;
using EduHub.Ch02.Patterns.Strategies;
using EduHub.Ch02.Repositories;
using EduHub.Ch02.Services;

namespace Ch02.Refactor.Tests;

public class PatternTests
{
    private sealed class FakeLogger : IAppLogger
    {
        public List<string> Messages { get; } = new();
        public void Log(string message) => Messages.Add(message);
    }

    private sealed class FakeObserver : IEventObserver
    {
        public int CallCount { get; private set; }
        public Event? LastEvent { get; private set; }

        public void OnEventCreated(Club club, Event ev)
        {
            CallCount++;
            LastEvent = ev;
        }
    }

    [Theory]
    [InlineData(NotificationChannel.Email, typeof(EmailNotificationService))]
    [InlineData(NotificationChannel.Sms, typeof(SmsNotificationService))]
    [InlineData(NotificationChannel.Slack, typeof(SlackNotificationService))]
    public void NotificationFactory_Create_ReturnsExpectedImplementation(NotificationChannel channel, Type expectedType)
    {
        // Arrange
        var factory = new NotificationServiceFactory();

        // Act
        var service = factory.Create(channel);

        // Assert
        Assert.IsType(expectedType, service);
    }

    [Fact]
    public void LoggingClubRepository_Add_DelegatesAndLogs()
    {
        // Arrange
        var inner = new InMemoryClubRepository();
        var logger = new FakeLogger();
        var repo = new LoggingClubRepository(inner, logger);
        var club = Club.Create("AI Club", "CLB AI");

        // Act
        repo.Add(club);

        // Assert
        Assert.Single(inner.GetAll());
        Assert.Single(logger.Messages);
        Assert.Contains("AI Club", logger.Messages[0]);
    }

    [Fact]
    public void CsvReportStrategy_Format_ReturnsHeaderAndRows()
    {
        // Arrange
        var strategy = new CsvReportStrategy();
        var id = Guid.NewGuid();
        var rows = new[] { new ClubReportRow(id, "Tech Club", 12) };

        // Act
        var csv = strategy.Format(rows);

        // Assert
        Assert.Contains("ClubId,ClubName,MemberCount", csv);
        Assert.Contains($"{id},Tech Club,12", csv);
    }

    [Fact]
    public void EventService_CreateEvent_NotifiesSubscribedObservers()
    {
        // Arrange
        var clubRepo = new InMemoryClubRepository();
        var club = Club.Create("Tech Club", "Mô tả");
        clubRepo.Add(club);
        var eventRepo = new InMemoryEventRepository();
        var logger = new FakeLogger();
        var service = new EventService(clubRepo, eventRepo, logger);
        var observer = new FakeObserver();
        service.Subscribe(observer);

        // Act
        var ev = service.CreateEvent(club.Id, "SOLID Talk", DateTime.Today);

        // Assert
        Assert.Equal(1, observer.CallCount);
        Assert.Equal(ev.Id, observer.LastEvent?.Id);
    }
}
