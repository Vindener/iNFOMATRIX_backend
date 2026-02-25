namespace Infomatrix.Domain;

public class DomainException : Exception
{
    public DomainException()
    {
    }

    public DomainException(string message)
        : base(message)
    {
    }

    public static void ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new DomainException(message);
    }
}

public class UserException : DomainException
{
    public UserException(string message)
        : base(message)
    {
    }

    public static void ThrowIfFullNameInvalid(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new UserException("Full name cannot be null or empty.");

        if (fullName.Length > 100)
            throw new UserException("Full name cannot exceed 100 characters.");
    }
}
