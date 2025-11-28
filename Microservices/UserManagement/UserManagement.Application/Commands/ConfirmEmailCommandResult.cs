namespace UserManagement.Application.Commands;

public class ConfirmEmailCommandResult
{
    public bool Success { get; set; }
    public string Message { get; set; }

    public static ConfirmEmailCommandResult Successful()
    {
        return new ConfirmEmailCommandResult 
        { 
            Success = true, 
            Message = "Email успешно подтвержден" 
        };
    }

    public static ConfirmEmailCommandResult Failed(string message)
    {
        return new ConfirmEmailCommandResult 
        { 
            Success = false, 
            Message = message 
        };
    }
}