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
            AssertFile.Contains("../../../api-netcoreapp2_1.txt", ApiGenerator.GeneratePublicApi(typeof(ScenarioAttribute).Assembly));
#endif
#if NET472
            AssertFile.Contains("../../../api-net472.txt", ApiGenerator.GeneratePublicApi(typeof(ScenarioAttribute).Assembly));
#endif
    }
}
