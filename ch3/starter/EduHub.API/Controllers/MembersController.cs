using EduHub.Domain.Entities;
using EduHub.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduHub.API.Controllers;

/// <summary>Quản lý thành viên câu lạc bộ.</summary>
[ApiController]
[Route("api/members")]
public class MembersController : ControllerBase
{
    private readonly IMemberRepository _repository;

    public MembersController(IMemberRepository repository)
    {
        _repository = repository;
    }

    /// <summary>Lấy danh sách thành viên.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Member>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var members = await _repository.GetAllAsync(ct);
        return Ok(members);
    }

    /// <summary>Lấy một thành viên theo Id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Member), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var member = await _repository.GetByIdAsync(id, ct);
        if (member is null)
            return NotFound();

        return Ok(member);
    }

    /// <summary>Thêm thành viên mới.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Member), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMemberRequest request, CancellationToken ct)
    {
        try
        {
            var member = Member.Create(request.StudentId, request.Name, request.Email);
            await _repository.AddAsync(member, ct);
            return CreatedAtAction(nameof(GetById), new { id = member.Id }, member);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Xoá một thành viên.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            await _repository.DeleteAsync(id, ct);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}

public record CreateMemberRequest(string StudentId, string Name, string Email);
