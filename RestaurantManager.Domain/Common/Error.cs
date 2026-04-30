namespace RestaurantManager.Domain.Common;

public sealed record Error
{
    public string Code { get; }
    public string Message { get; }
    public ErrorType Type { get; }
    public Dictionary<string, string[]>? ValidationErrors { get; }

    private Error(string code, string message, ErrorType type, Dictionary<string, string[]>? validationErrors = null)
    {
        Code = code;
        Message = message;
        Type = type;
        ValidationErrors = validationErrors;
    }

    public static Error None => new(string.Empty, string.Empty, ErrorType.None);

    public static Error NotFound(string code, string message)
        => new(code, message, ErrorType.NotFound);

    public static Error Validation(string code, string message, Dictionary<string, string[]>? validationErrors = null)
        => new(code, message, ErrorType.Validation, validationErrors);

    public static Error Conflict(string code, string message)
        => new(code, message, ErrorType.Conflict);

    public static Error Failure(string code, string message)
        => new(code, message, ErrorType.Failure);

    public static Error Unauthorized(string code, string message)
        => new(code, message, ErrorType.Unauthorized);

    public static Error Forbidden(string code, string message)
        => new(code, message, ErrorType.Forbidden);
}
