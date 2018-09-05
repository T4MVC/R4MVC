using System;

namespace R4Mvc.Tools.Commands.Core
{
    public interface ICommand
    {
        bool IsGlobal { get; }
        byte Order { get; }
        string Key { get; }
        string Summary { get; }
        string Description { get; }

        Type GetCommandType();
    }
}
