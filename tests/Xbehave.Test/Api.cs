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
#if NETCOREAPP2_2
                "../../../api-netcoreapp2_2.txt",
#endif
#if NET472
                "../../../api-net472.txt",
#endif
                ApiGenerator.GeneratePublicApi(typeof(ScenarioAttribute).Assembly));
    }
}
