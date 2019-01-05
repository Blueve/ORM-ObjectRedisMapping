namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// The interface of primitive type proxy.
    /// </summary>
    internal interface IPrimitiveProxy
    {
        DbValue Get(DbValueType type);

        void Set(DbValue value);
    }
}
