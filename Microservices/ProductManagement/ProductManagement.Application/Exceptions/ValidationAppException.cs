namespace ProductManagement.Application.Exceptions;

public class ValidationAppException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationAppException(IReadOnlyDictionary<string, string[]> errors)
    : base("Произошла одна или больше ошибок валидации.") 
        => Errors = errors;
}