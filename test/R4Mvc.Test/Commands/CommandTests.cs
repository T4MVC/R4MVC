using System;
using System.Collections.Generic;
using System.Linq;
using R4Mvc.Tools.Commands.Core;
using Xunit;

namespace R4Mvc.Test.Commands
{
    public class CommandTests
    {
        [Fact]
        public void Commands_Any()
        {
            var commands = CommandResolver.GetCommands();

            Assert.NotEmpty(commands);
        }

        [Fact]
        public void Commands_UniqueKeys()
        {
            var commands = CommandResolver.GetCommands();
            var duplicates = commands
                .GroupBy(c => c.Key, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1);

            Assert.Empty(duplicates);
        }

        public static IEnumerable<object[]> GetCommands()
            => CommandResolver.GetCommands()
                .Select(c => new object[] { c });

        [Theory]
        [MemberData(nameof(GetCommands))]
        public void Commands_HaveKey(ICommand command)
        {
            Assert.NotNull(command.Key);
            Assert.NotEmpty(command.Key);
        }

        [Theory]
        [MemberData(nameof(GetCommands))]
        public void Commands_HaveImplementation(ICommand command)
        {
            var runnerType = command.GetCommandType();

            Assert.NotNull(runnerType);
            Assert.True(typeof(ICommandRunner).IsAssignableFrom(runnerType));
        }

        [Fact]
        public void Commands_Runners_Any()
        {
            var runners = CommandResolver.GetCommandRunnerTypes();

            Assert.NotEmpty(runners);
        }
    }
}
