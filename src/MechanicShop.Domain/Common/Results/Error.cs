namespace MechanicShop.Domain.Common.Results;

public readonly record struct Error
{
    private Error(string code,
                  string description,
                  ErrorKind type)
    {
        Code = code;
        Description = description;
        Type = type;
    }
    public string Code { get; }
    public string Description { get; }
    public ErrorKind Type { get; }



    public static Error Failure(string code = nameof(Failure), string description = "General Failure.", ErrorKind type = ErrorKind.Failure)
                  => new(code, description, type);

    public static Error Unexpected(string code = nameof(Unexpected), string description = "Unexpected Error.", ErrorKind type = ErrorKind.Unexpected)
                  => new(code, description, type);
    public static Error Validation(string code = nameof(Validation), string description = "Validation Error.", ErrorKind type = ErrorKind.Validation)
                  => new(code, description, type);
    public static Error NotFound(string code = nameof(NotFound), string description = "NotFound.", ErrorKind type = ErrorKind.NotFound)
                  => new(code, description, type);
    public static Error UnAuthorized(string code = nameof(UnAuthorized), string description = "UnAuthorized.", ErrorKind type = ErrorKind.UnAuthorized)
                  => new(code, description, type);
    public static Error Forbidden(string code = nameof(Forbidden), string description = "Forbidden.", ErrorKind type = ErrorKind.Forbidden)
                  => new(code, description, type);
    public static Error Conflict(string code = nameof(Conflict), string description = "Conflict.", ErrorKind type = ErrorKind.Conflict)
    => new(code, description, type);
}