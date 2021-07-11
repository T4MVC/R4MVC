using System;

namespace R4Mvc.ModelUnbinders
{
    public interface IModelUnbinderProvider
    {
        IModelUnbinder GetUnbinder(Type routeValueType);
    }
}
