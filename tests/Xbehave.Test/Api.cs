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
#if NETCOREAPP3_1
                "../../../api-netcoreapp3_1.txt",
#endif
#if NET48
                "../../../api-net48.txt",
#endif
                typeof(ScenarioAttribute).Assembly.GeneratePublicApi());
    }
}
