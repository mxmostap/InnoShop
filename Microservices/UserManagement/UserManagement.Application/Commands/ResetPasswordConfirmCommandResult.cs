namespace UserManagement.Application.Commands;

public class ResetPasswordConfirmCommandResult
{
    public bool Success { get; set; }
    public string Message { get; set; }

    public static ResetPasswordConfirmCommandResult Successful()
    {
        return new ResetPasswordConfirmCommandResult 
        { 
            Success = true, 
            Message = "Пароль успешно изменен" 
        };
    }

    public static ResetPasswordConfirmCommandResult Failed(string message)
    {
        return new ResetPasswordConfirmCommandResult 
        { 
            Success = false, 
            Message = message 
        };
    }
}