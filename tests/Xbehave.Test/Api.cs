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
            AssertFile.Contains(
#if NETCOREAPP3_0
                "../../../api-netcoreapp3_0.txt",
#endif
#if NET472
                "../../../api-net472.txt",
#endif
                ApiGenerator.GeneratePublicApi(typeof(ScenarioAttribute).Assembly));
    }
}
