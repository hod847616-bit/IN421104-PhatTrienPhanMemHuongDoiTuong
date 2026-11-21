using EduHub.Ch02.Domain;
using EduHub.Ch02.Notifications;
using EduHub.Ch02.Repositories;

namespace EduHub.Ch02.Patterns.Observers;

public class EmailNotifier : IEventObserver
{
    private readonly IMemberRepository _memberRepository;
    private readonly INotificationService _notificationService;

    public EmailNotifier(IMemberRepository memberRepository, INotificationService notificationService)
    {
        _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
    }

    public void OnEventCreated(Club club, Event ev)
    {
        foreach (var member in _memberRepository.GetByClubId(club.Id))
        {
            _notificationService.Send(
                member.Email,
                $"New event '{ev.Title}' at {club.Name} on {ev.Date:yyyy-MM-dd}");
        }
    }
}
