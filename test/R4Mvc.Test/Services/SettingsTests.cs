using System;
using R4Mvc.Services;
using Xunit;

namespace R4Mvc.Test.Services
{
    public class SettingsTests
    {
        [Fact]
        public void HelpersPrefix_returns_expected_value_from_json_file()
        {
            // Arrange
            var settings = GetSettings();
            var expected = "MVCfoo";

            // Act
            var actual = settings.HelpersPrefix;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void HelpersPrefix_returns_default_value_from_json_file_when_key_not_found()
        {
            // Arrange
            var settings = GetEmptySettings();
            var expected = "MVC";

            // Act
            var actual = settings.HelpersPrefix;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void R4MvcNamespace_returns_expected_value_from_json_file()
        {
            // Arrange
            var settings = GetSettings();
            var expected = "R4Mvcfoo";

            // Act
            var actual = settings.R4MvcNamespace;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void R4MvcNamespace_returns_default_value_from_json_file_when_key_not_found()
        {
            // Arrange
            var settings = GetEmptySettings();
            var expected = "R4Mvc";

            // Act
            var actual = settings.R4MvcNamespace;

            // Assert
            Assert.Equal(expected, actual);
        }

        private ISettings GetSettings()
        {
            var projectDirectory = GetProjectDirectory();
            return new Settings(projectDirectory);
        }

        private ISettings GetEmptySettings()
        {
            var projectDirectory = GetProjectDirectory();
            return new Settings(projectDirectory, "r4mvc.empty.json");
        }

        private string GetProjectDirectory()
        { 
#if !ASPNETCORE50
        return AppDomain.CurrentDomain.BaseDirectory;
#else
       return AppContext.BaseDirectory;
#endif
        }

    }
}