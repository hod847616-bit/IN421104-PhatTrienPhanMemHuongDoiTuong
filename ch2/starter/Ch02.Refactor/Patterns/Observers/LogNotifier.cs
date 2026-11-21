using EduHub.Ch02.Domain;
using EduHub.Ch02.Logging;

namespace EduHub.Ch02.Patterns.Observers;

public class LogNotifier : IEventObserver
{
    private readonly IAppLogger _logger;

    public LogNotifier(IAppLogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnEventCreated(Club club, Event ev)
    {
        _logger.Log($"[LOG] Event created: '{ev.Title}' for club '{club.Name}' on {ev.Date:yyyy-MM-dd}");
    }
}
