namespace UserManagement.Application.Services.Email;

public class EmailSettings
{
    public string BaseUrl { get; set; }
    public string FromEmail { get; set; }
    public string FromName { get; set; }
    public int TokenExpirationHours { get; set; } = 24;
}