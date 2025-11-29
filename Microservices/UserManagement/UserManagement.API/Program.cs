using UserManagement.API.Configurations;
using UserManagement.API.Middlewares;
using UserManagement.Application.DI;

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

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

var app = builder.Build();
    

app.UseCors(myAllowSpecificOrigins);
app.UseMiddleware<ExceptionHandlingMiddleware>();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();