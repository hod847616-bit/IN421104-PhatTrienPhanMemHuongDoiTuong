using EduHub.Ch02;
using EduHub.Ch02.Domain;

namespace Ch02.Refactor.Tests;

// Mẫu AAA: Arrange (chuẩn bị) - Act (thực hiện) - Assert (kiểm tra).
// Mỗi test chỉ kiểm tra MỘT hành vi. Đặt tên: TênMethod_TìnhHuống_KếtQuảMongĐợi.
public class DomainTests
{
    [Fact]
    public void CreateClub_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        var manager = new ClubManager();

        // Act
        var act = () => manager.CreateClub("", "mô tả", "p@gmail.com");

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Club_Create_WithValidName_ReturnsClub()
    {
        // Arrange
        var name = "Tech Club";
        var description = "CLB Công nghệ";

        // Act
        var club = Club.Create(name, description);

        // Assert
        Assert.NotEqual(Guid.Empty, club.Id);
        Assert.Equal(name, club.Name);
        Assert.Equal(description, club.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Club_Create_WithInvalidName_ThrowsArgumentException(string invalidName)
    {
        // Arrange
        var description = "Mô tả";

        // Act
        var act = () => Club.Create(invalidName, description);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Club_Create_WithNameExceedingMaxLength_ThrowsArgumentException()
    {
        // Arrange
        var tooLongName = new string('A', Club.MaxNameLength + 1);

        // Act
        var act = () => Club.Create(tooLongName, "Mô tả");

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Club_HasRoom_BelowMaxMembers_ReturnsTrue()
    {
        // Arrange
        var club = Club.Create("Tech Club", "Mô tả");

        // Act
        var hasRoom = club.HasRoom(Club.MaxMembers - 1);

        // Assert
        Assert.True(hasRoom);
    }

    [Fact]
    public void Club_HasRoom_AtMaxMembers_ReturnsFalse()
    {
        // Arrange
        var club = Club.Create("Tech Club", "Mô tả");

        // Act
        var hasRoom = club.HasRoom(Club.MaxMembers);

        // Assert
        Assert.False(hasRoom);
    }

    [Fact]
    public void Event_Register_NewMember_ReturnsTrue()
    {
        // Arrange
        var ev = new Event { Id = Guid.NewGuid(), ClubId = Guid.NewGuid(), Title = "Workshop", Date = DateTime.Today };
        var memberId = Guid.NewGuid();

        // Act
        var registered = ev.Register(memberId);

        // Assert
        Assert.True(registered);
        Assert.Contains(memberId, ev.RegisteredMemberIds);
    }

    [Fact]
    public void Event_Register_SameMemberTwice_ReturnsFalse()
    {
        // Arrange
        var ev = new Event { Id = Guid.NewGuid(), ClubId = Guid.NewGuid(), Title = "Workshop", Date = DateTime.Today };
        var memberId = Guid.NewGuid();
        ev.Register(memberId);

        // Act
        var secondRegister = ev.Register(memberId);

        // Assert
        Assert.False(secondRegister);
    }

    [Fact]
    public void Event_CheckIn_RegisteredMember_AddsToCheckedIn()
    {
        // Arrange
        var ev = new Event { Id = Guid.NewGuid(), ClubId = Guid.NewGuid(), Title = "Workshop", Date = DateTime.Today };
        var memberId = Guid.NewGuid();
        ev.Register(memberId);

        // Act
        ev.CheckIn(memberId);

        // Assert
        Assert.Contains(memberId, ev.CheckedInMemberIds);
    }

    [Fact]
    public void Event_CheckIn_UnregisteredMember_ThrowsInvalidOperationException()
    {
        // Arrange
        var ev = new Event { Id = Guid.NewGuid(), ClubId = Guid.NewGuid(), Title = "Workshop", Date = DateTime.Today };
        var unregisteredId = Guid.NewGuid();

        // Act
        var act = () => ev.CheckIn(unregisteredId);

        // Assert
        Assert.Throws<InvalidOperationException>(act);
    }
}
