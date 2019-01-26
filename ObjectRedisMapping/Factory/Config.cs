namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// The configurations of ORM.
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Indicate whether use full type name as the prefix of entity.
        /// </summary>
        public bool UseFullTypeName { get; set; }
    }
}
