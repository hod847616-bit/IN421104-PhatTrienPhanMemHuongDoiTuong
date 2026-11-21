using EduHub.Ch02.Domain;

namespace EduHub.Ch02.Patterns.Observers;

public interface IEventObserver
{
    void OnEventCreated(Club club, Event ev);
}
