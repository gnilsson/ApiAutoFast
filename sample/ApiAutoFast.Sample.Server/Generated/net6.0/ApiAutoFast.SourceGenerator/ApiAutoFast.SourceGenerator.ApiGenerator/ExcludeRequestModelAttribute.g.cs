﻿
using System;

namespace ApiAutoFast;

/// <summary>
/// Attribute to exclude property from request model.
/// <param name="includeRequestModelTarget">If not applied, property is per default included in
/// RequestModelTarget.CreateCommand | RequestModelTarget.DeleteCommand | RequestModelTarget.QueryRequest</param>
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ExcludeRequestModelAttribute : Attribute
{
    public ExcludeRequestModelAttribute(RequestModelTarget includeRequestModelTarget = RequestModelTarget.None)
    {
        IncludeRequestModelTarget = includeRequestModelTarget;
    }

    public RequestModelTarget IncludeRequestModelTarget { get; }
}
