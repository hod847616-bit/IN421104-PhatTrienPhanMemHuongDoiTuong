using EduHub.Ch02.Logging;
using EduHub.Ch02.Patterns.Strategies;
using EduHub.Ch02.Repositories;

namespace EduHub.Ch02.Services;

public class ReportService : IReportService
{
    private readonly IClubRepository _clubRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IAppLogger _logger;

    public ReportService(
        IClubRepository clubRepository,
        IMemberRepository memberRepository,
        IAppLogger logger)
    {
        _clubRepository = clubRepository ?? throw new ArgumentNullException(nameof(clubRepository));
        _memberRepository = memberRepository ?? throw new ArgumentNullException(nameof(memberRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string Generate(IReportStrategy strategy)
    {
        ArgumentNullException.ThrowIfNull(strategy);

        var rows = _clubRepository.GetAll()
            .Select(c => new ClubReportRow(c.Id, c.Name, _memberRepository.CountByClubId(c.Id)))
            .ToList();

        _logger.Log($"Report generated for {rows.Count} club(s) using {strategy.GetType().Name}");
        return strategy.Format(rows);
    }
}
