using Microsoft.Extensions.DependencyInjection;
using UserManagement.Infrastructure.Persistance;

namespace UserManagement.Tests.Integration.Common;

public abstract class BaseIntegrationTest
    : IClassFixture<CustomWebApplicationFactory>,
        IAsyncLifetime
{
    private readonly IServiceScope _scope;
    private readonly CustomWebApplicationFactory _factory;
    protected readonly HttpClient Client;
    protected readonly EFDBContext DbContext;

    protected BaseIntegrationTest(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _scope = factory.Services.CreateScope();

        Client = factory.HttpClient;

        DbContext = _scope.ServiceProvider
            .GetRequiredService<EFDBContext>();
    }

    public Task InitializeAsync() => Task.CompletedTask;    

    public async Task DisposeAsync()
    {
        await _factory.ResetDatabaseAsync();
        await DbContext.SaveChangesAsync();
    } 
    
    public void Dispose()
    {
        _scope?.Dispose();
        DbContext?.Dispose();
    }
}