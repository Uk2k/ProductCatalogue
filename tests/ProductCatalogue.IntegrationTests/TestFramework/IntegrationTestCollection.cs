namespace ProductCatalogue.IntegrationTests.TestFramework;

[CollectionDefinition(Name)]
public sealed class IntegrationTestCollection : ICollectionFixture<SqlServerContainerFixture>
{
    public const string Name = "SQL Server integration tests";
}
