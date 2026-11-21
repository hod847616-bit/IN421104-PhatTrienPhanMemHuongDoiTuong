using EduHub.Ch02.Domain;

namespace EduHub.Ch02.Repositories;

public interface IMemberRepository
{
    void Add(Member member);
    IReadOnlyList<Member> GetByClubId(Guid clubId);
    int CountByClubId(Guid clubId);
}
