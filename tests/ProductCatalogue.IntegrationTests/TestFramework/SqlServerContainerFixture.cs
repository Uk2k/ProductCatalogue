using System.Data.Common;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;

namespace ProductCatalogue.IntegrationTests.TestFramework;

public sealed class SqlServerContainerFixture : IAsyncLifetime
{
    private readonly MsSqlContainer container = new MsSqlBuilder()
        .WithPassword("TestPassword123!")
        .Build();

    public Task InitializeAsync() => container.StartAsync();

    public async Task<DatabaseScope> CreateDatabaseAsync()
    {
        var databaseName = $"ProductCatalogueTest_{Guid.NewGuid():N}";
        var connectionStringBuilder = new SqlConnectionStringBuilder(container.GetConnectionString())
        {
            InitialCatalog = databaseName
        };

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionStringBuilder.ConnectionString)
            .Options;
        var context = new AppDbContext(options);

        try
        {
            await context.Database.MigrateAsync();
            return new DatabaseScope(databaseName, connectionStringBuilder.ConnectionString, context);
        }
        catch
        {
            await context.DisposeAsync();
            throw;
        }
    }

    public Task DisposeAsync() => container.DisposeAsync().AsTask();

    public sealed class DatabaseScope(
        string databaseName,
        string connectionString,
        AppDbContext context) : IAsyncDisposable
    {
        private readonly string connectionStringValue = connectionString;

        public AppDbContext Context { get; } = context;

        public string ConnectionString { get; } = connectionString;

        public AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connectionStringValue)
                .Options;
            return new AppDbContext(options);
        }

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();

            var masterConnectionString = new SqlConnectionStringBuilder(connectionStringValue)
            {
                InitialCatalog = "master"
            };

            await using var connection = new SqlConnection(masterConnectionString.ConnectionString);
            await connection.OpenAsync();
            await using var command = connection.CreateCommand();
            command.CommandText = $"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{databaseName}];";
            await command.ExecuteNonQueryAsync();
        }
    }
}
