using EduHub.Ch02.Domain;
using EduHub.Ch02.Logging;

namespace EduHub.Ch02.Repositories;

/// <summary>
/// Decorator bọc IClubRepository để ghi log mỗi lần truy cập dữ liệu CLB.
/// </summary>
public class LoggingClubRepository : IClubRepository
{
    private readonly IClubRepository _inner;
    private readonly IAppLogger _logger;

    public LoggingClubRepository(IClubRepository inner, IAppLogger logger)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void Add(Club club)
    {
        _inner.Add(club);
        _logger.Log($"[repo] Added club '{club.Name}' ({club.Id})");
    }

    public Club? GetById(Guid id)
    {
        _logger.Log($"[repo] Querying club {id}");
        return _inner.GetById(id);
    }

    public IReadOnlyList<Club> GetAll()
    {
        var clubs = _inner.GetAll();
        _logger.Log($"[repo] Retrieved {clubs.Count} club(s)");
        return clubs;
    }
}
