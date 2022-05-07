using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiAutoFast;

public abstract class EntityConfig
{
    //internal abstract void DefineProperties();
    //internal abstract void DefineEndpoints();
    public List<Type> EndpointInjectionTypes { get; } = new();
    public virtual void Configure()
    {
    }
}
