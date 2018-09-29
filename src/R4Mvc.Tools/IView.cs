using System;

namespace R4Mvc.Tools
{
    public interface IView
    {
        Uri RelativePath { get; }
        string TemplateKind { get; }
        string Name { get; }
    }
}
