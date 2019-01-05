namespace Blueve.ObjectRedisMapping
{
    using System;

    /// <summary>
    /// The entity key attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    internal class EntityKeyAttribute : Attribute
    {
        /// <summary>
        /// Indicate whether the entity key want to use interface to extract it key.
        /// </summary>
        public bool UseInterface { get; set; } = false;
    }
}
