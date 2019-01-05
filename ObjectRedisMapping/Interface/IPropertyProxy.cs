namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// The interface of property proxy.
    /// </summary>
    internal interface IPropertyProxy
    {
        IPropertyProxy Get();

        void Set(IPropertyProxy proxy);
    }
}
