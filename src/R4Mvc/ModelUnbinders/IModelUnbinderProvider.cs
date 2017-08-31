using System;
using System.Collections.Generic;
using System.Text;

namespace R4Mvc.ModelUnbinders
{
    public interface IModelUnbinderProvider
    {
        IModelUnbinder GetUnbinder(Type routeValueType);
    }
}
