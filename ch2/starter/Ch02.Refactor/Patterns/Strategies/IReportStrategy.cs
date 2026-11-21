namespace EduHub.Ch02.Patterns.Strategies;

public interface IReportStrategy
{
    string Format(IEnumerable<ClubReportRow> rows);
}
