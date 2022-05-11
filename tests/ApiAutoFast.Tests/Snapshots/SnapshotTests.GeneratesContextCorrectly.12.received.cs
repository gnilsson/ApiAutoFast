﻿//HintName: ExcludeRequestModelAttribute.g.cs

using System;

namespace ApiAutoFast;

[AttributeUsage(AttributeTargets.Property)]
public class ExcludeRequestModelAttribute : Attribute
{
    public ExcludeRequestModelAttribute(RequestModelTarget includeRequestModelTarget = RequestModelTarget.None)
    {
        IncludeRequestModelTarget = includeRequestModelTarget;
    }

    public RequestModelTarget IncludeRequestModelTarget { get; }
}
