namespace LuxMedTest.FunctionalTests.Fixtures
{
    [CollectionDefinition("DbTests", DisableParallelization = true)]

    public class InMemoryDbCollection : ICollectionFixture<TestFixture>
    {
    }
}
