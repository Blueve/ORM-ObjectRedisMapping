namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// The interface for extracting the key.
    /// </summary>
    public interface IEntityKey
    {
        /// <summary>
        /// Gets the key of current instance.
        /// </summary>
        /// <returns>The key.</returns>
        string GetKey();
    }
}
