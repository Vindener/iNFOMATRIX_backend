namespace Infomatrix.Api.Domain;

public class UserEntity : BaseEntity
{
    public string FullName { get; private set; } = string.Empty;

    private UserEntity()
    {
    }

    private UserEntity(string fullName)
    {
        FullName = fullName;
    }

    public static UserEntity Create(string fullName)
    {
        UserException
            .ThrowIfFullNameInvalid(fullName);

        return new UserEntity(fullName);
    }

    public void Update(string fullName)
    {
        UserException
            .ThrowIfFullNameInvalid(fullName);

        FullName = fullName;
    }
}
