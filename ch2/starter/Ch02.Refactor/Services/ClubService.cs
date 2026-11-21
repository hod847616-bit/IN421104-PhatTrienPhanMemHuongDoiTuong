using EduHub.Ch02.Domain;
using EduHub.Ch02.Logging;
using EduHub.Ch02.Notifications;
using EduHub.Ch02.Repositories;

namespace EduHub.Ch02.Services;

public class ClubService : IClubService
{
    private readonly IClubRepository _clubRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IAppLogger _logger;
    private readonly INotificationServiceFactory _notificationFactory;

    public ClubService(
        IClubRepository clubRepository,
        IMemberRepository memberRepository,
        IAppLogger logger,
        INotificationServiceFactory notificationFactory)
    {
        _clubRepository = clubRepository ?? throw new ArgumentNullException(nameof(clubRepository));
        _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _notificationFactory = notificationFactory ?? throw new ArgumentNullException(nameof(notificationFactory));
    }

    public Club CreateClub(string name, string description, string presidentEmail)
    {
        var club = Club.Create(name, description);
        _clubRepository.Add(club);

        _logger.Log($"Club created: {club.Name}");
        var emailService = _notificationFactory.Create(NotificationChannel.Email);
        emailService.Send(presidentEmail, $"Club '{club.Name}' has been created.");

        return club;
    }

    public Member AddMember(Guid clubId, string memberName, string email)
    {
        var club = _clubRepository.GetById(clubId)
            ?? throw new InvalidOperationException("Club not found.");

        var currentCount = _memberRepository.CountByClubId(clubId);
        if (!club.HasRoom(currentCount))
            throw new InvalidOperationException($"Club is full (max {Club.MaxMembers} members).");

        var member = new Member
        {
            Id = Guid.NewGuid(),
            Name = memberName,
            Email = email,
            ClubId = clubId
        };
        _memberRepository.Add(member);

        _logger.Log($"Member '{memberName}' joined '{club.Name}'");
        _notificationFactory.Create(NotificationChannel.Email).Send(email, $"Welcome to {club.Name}!");
        _notificationFactory.Create(NotificationChannel.Slack).Send("#clubs", $"{memberName} joined {club.Name}");

        return member;
    }

    public IReadOnlyList<Club> GetAllClubs() => _clubRepository.GetAll();
}
