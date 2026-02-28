using Infomatrix.Api.Domain;
using Infomatrix.Api.Dtos;

namespace Infomatrix.Api.Mapping;

public static class UserExtensions
{
    public static UserResponse ToResponse(
        this UserEntity entity)
    {
        return new UserResponse(
            entity.Id,
            entity.FullName);
    }

    public static UserEntity ToEntity(
        this CreateUserRequest dto)
    {
        return UserEntity.Create(
            dto.FullName);
    }

    public static IEnumerable<UserResponse> ToResponse(
        this IEnumerable<UserEntity> entities)
    {
        return entities
            .Select(e => e.ToResponse());
    }
}
