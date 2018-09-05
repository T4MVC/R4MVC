using System;
using System.Collections.Generic;
using System.Linq;

namespace R4Mvc.Tools.Commands.Core
{
    public static class CommandResolver
    {
        private static Type[] _allTypes = null;

        private static IEnumerable<Type> GetTypes<TType>()
        {
            if (_allTypes == null)
                _allTypes = typeof(CommandResolver).Assembly.GetTypes()
                    .Where(t => !t.IsAbstract)
                    .ToArray();

            return _allTypes.Where(t => typeof(TType).IsAssignableFrom(t));
        }

        public static ICommand[] GetCommands()
            => GetTypes<ICommand>()
                .Select(t => t.GetConstructor(Type.EmptyTypes).Invoke(null) as ICommand)
                .OrderBy(c => c.Order)
                .ThenBy(c => c.Key)
                .ToArray();

        public static IEnumerable<Type> GetCommandRunnerTypes() => GetTypes<ICommandRunner>();
    }
}
