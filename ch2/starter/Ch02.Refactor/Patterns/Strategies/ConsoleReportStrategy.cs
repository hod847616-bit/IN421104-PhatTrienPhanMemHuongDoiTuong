using System.Text;

namespace EduHub.Ch02.Patterns.Strategies;

public class ConsoleReportStrategy : IReportStrategy
{
    public string Format(IEnumerable<ClubReportRow> rows)
    {
        var sb = new StringBuilder();
        foreach (var row in rows)
        {
            sb.AppendLine($"Club: {row.ClubName} | Members: {row.MemberCount}");
        }
        return sb.ToString().TrimEnd();
    }
}
