using System;

namespace Testing.Common.Builders.Domain
{
    public static class MockBuildExtensions
    {
        /// <summary>
        /// Helper to set a protected member for testing purposes, for example navigation properties
        /// </summary>
        public static void SetProtected(this object instance, string propertyName, object value)
        {
            var instanceType = instance.GetType();
            var property = instanceType.GetProperty(propertyName);
            if (property == null) throw new InvalidOperationException($"No property '{propertyName}' found on object of type '{instanceType.Name}'");
            property.SetValue(instance, value);
        }
    }
}