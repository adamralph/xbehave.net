namespace Xbehave.Test
{
    using PublicApiGenerator;
    using Xbehave;
    using Xbehave.Test.Infrastructure;
    using Xunit;

    public class Api
    {
        [Fact]
        public void IsUnchanged() =>
#if NETCOREAPP2_1
            AssertFile.Contains(ApiGenerator.GeneratePublicApi(typeof(ScenarioAttribute).Assembly), "../../../api-netcoreapp2_1.txt");
#endif
#if NET472
            AssertFile.Contains(ApiGenerator.GeneratePublicApi(typeof(ScenarioAttribute).Assembly), "../../../api-net472.txt");
#endif
    }
}
