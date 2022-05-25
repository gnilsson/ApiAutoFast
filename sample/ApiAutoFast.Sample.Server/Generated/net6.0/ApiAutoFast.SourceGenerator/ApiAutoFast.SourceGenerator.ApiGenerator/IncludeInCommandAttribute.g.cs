
    using System;

    namespace ApiAutoFast;

    /// <summary>
    /// Attribute to include property in another entity command.
    /// <param name="otherEntityType">The other entity</param>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class IncludeInCommandAttribute : Attribute
    {
        public IncludeInCommandAttribute(params Type[] otherEntityTypes)
        {
            OtherEntityTypes = otherEntityTypes;
        }

        public Type[] OtherEntityTypes { get; }
    }
    