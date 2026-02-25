using Infomatrix.Data;
using Infomatrix.Domain;
using Infomatrix.Dtos;
using Infomatrix.Mapping;
using Microsoft.AspNetCore.Mvc;

namespace Infomatrix.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IRepository<UserEntity> _userRepository;

    public UserController(
        IRepository<UserEntity> userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    [ProducesResponseType<IEnumerable<UserResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAllAsync(
        CancellationToken ct)
    {
        var users = await _userRepository
            .GetAllAsync(ct);

        return Ok(users.ToResponse());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetAsync(
        Guid id,
        CancellationToken ct)
    {
        var user = await _userRepository
            .GetByIdAsync(id, ct);

        if (user is null)
        {
            return NotFound();
        }

        return user.ToResponse();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<UserResponse>> CreateAsync(
        [FromBody] CreateUserRequest request,
        CancellationToken ct)
    {
        var user = request.ToEntity();

        await _userRepository
            .AddAsync(user, ct);
        await _userRepository
            .SaveChangesAsync(ct);

        var response = user
            .ToResponse();

        return CreatedAtAction(
            "Get",
            new { id = user.Id },
            response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(
        Guid id,
        [FromBody] UpdateUserRequest request,
        CancellationToken ct)
    {
        var user = await _userRepository
            .GetByIdAsync(id, ct);

        if (user is null)
        {
            return NotFound();
        }

        user.Update(request.FullName);

        _userRepository
            .Update(user);
        await _userRepository
            .SaveChangesAsync(ct);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(
        Guid id,
        CancellationToken ct)
    {
        var user = await _userRepository
            .GetByIdAsync(id, ct);

        if (user is null)
        {
            return NotFound();
        }

        _userRepository
            .Delete(user);
        await _userRepository
            .SaveChangesAsync(ct);

        return NoContent();
    }
}