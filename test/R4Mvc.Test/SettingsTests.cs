using R4Mvc.Tools;
using Xunit;

namespace R4Mvc.Test
{
    public class SettingsTests
    {
        [Fact]
        public void HelpersPrefix_Default() => Assert.NotNull(new Settings().HelpersPrefix);

        [Fact]
        public void R4MvcNamespace_Default() => Assert.NotNull(new Settings().R4MvcNamespace);

        [Fact]
        public void Linksnamespace_Default() => Assert.NotNull(new Settings().LinksNamespace);
    }
}
