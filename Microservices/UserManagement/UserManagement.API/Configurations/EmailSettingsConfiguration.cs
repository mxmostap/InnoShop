using UserManagement.Application.Common.Interfaces;
using UserManagement.Application.Services.Email;

namespace UserManagement.API.Configurations;

public static class EmailSettingsConfiguration
{
    public static IServiceCollection ConfigureEmail(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));

        services.AddScoped<ISmtpClient, SmtpClient>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}