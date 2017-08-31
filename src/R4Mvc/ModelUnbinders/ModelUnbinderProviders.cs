using System;
using System.Collections.Generic;
using System.Text;

namespace R4Mvc.ModelUnbinders
{
    public class ModelUnbinderProviders
    {
        private readonly List<IModelUnbinderProvider> _unbinderProviders = new List<IModelUnbinderProvider>();
        public virtual void Add(IModelUnbinderProvider unbinderProvider)
        {
            _unbinderProviders.Add(unbinderProvider);
        }
        public virtual IModelUnbinder FindUnbinderFor(Type type)
        {
            foreach (var unbinderProvider in _unbinderProviders)
            {
                var result = unbinderProvider.GetUnbinder(type);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public virtual void Clear()
        {
            _unbinderProviders.Clear();
        }
    }
}
