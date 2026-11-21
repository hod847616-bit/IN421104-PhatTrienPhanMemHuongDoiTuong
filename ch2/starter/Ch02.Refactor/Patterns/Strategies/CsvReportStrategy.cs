using System.Text;

namespace EduHub.Ch02.Patterns.Strategies;

public class CsvReportStrategy : IReportStrategy
{
    public string Format(IEnumerable<ClubReportRow> rows)
    {
        var sb = new StringBuilder();
        sb.AppendLine("ClubId,ClubName,MemberCount");
        foreach (var row in rows)
        {
            var escapedName = row.ClubName.Contains(',') ? $"\"{row.ClubName.Replace("\"", "\"\"")}\"" : row.ClubName;
            sb.AppendLine($"{row.ClubId},{escapedName},{row.MemberCount}");
        }
        return sb.ToString().TrimEnd();
    }
}
