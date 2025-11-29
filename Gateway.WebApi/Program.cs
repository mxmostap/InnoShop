using Gateway.WebApi.Confidurations;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

var routes = "Routes";
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"{routes}/ocelot.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"{routes}/ocelot.SwaggerEndPoints.{builder.Environment.EnvironmentName}.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddSwaggerForOcelot(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddHttpsRedirection(options => options.HttpsPort = 443);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var app = builder.Build();
app.UseCors(MyAllowSpecificOrigins);

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs";
    opt.ReConfigureUpstreamSwaggerJson = AlterUpstream.AlterUpstreamSwaggerJson;
}).UseOcelot().Wait();
//app.UseHttpsRedirection();
//}

app.Run();