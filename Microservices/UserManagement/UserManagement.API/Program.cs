using UserManagement.API.Configurations;
using UserManagement.API.Middlewares;
using UserManagement.Application.DI;
using UserManagement.Infrastructure.Extensions;
using UserManagement.Infrastructure.Seeders;

var builder = WebApplication.CreateBuilder(args);
var myAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.ConfigureEmail(builder.Configuration);

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.RegisterRequestHandlers();
builder.Services.ConfigureSwagger();
builder.Services.ConfigureAuthorization(builder.Configuration);
builder.Services.ConfigureCors(myAllowSpecificOrigins);

var app = builder.Build();

await app.Services.ApplyMigrationsAsync();

using var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
await seeder.SeedAdminInfoAsync();
    

app.UseCors(myAllowSpecificOrigins);
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();