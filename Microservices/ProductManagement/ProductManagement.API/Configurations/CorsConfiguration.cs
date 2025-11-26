namespace ProductManagement.API.Configurations;

public static class CorsConfiguration
{
    public static IServiceCollection ConfigureCors(
        this IServiceCollection services,
        string specificOrigins)
    {
        return services.AddCors(options =>
        {
            options.AddPolicy(name: specificOrigins,
                policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });
    }
}