namespace Blueve.ObjectRedisMapping
{
    using System.Collections.Generic;

    /// <summary>
    /// The interface of database accessor.
    /// </summary>
    internal interface IDbAccessor
    {
        #region DB String
        /// <summary>
        /// Sets a string to database by a key.
        /// </summary>
        /// <param name="key">The database key.</param>
        /// <param name="value">The value.</param>
        void Set(string key, string value);

        /// <summary>
        /// Gets a string from database by a key.
        /// </summary>
        /// <param name="key">The database key.</param>
        /// <returns>The value.</returns>
        string Get(string key);
        #endregion

        #region DB Linked List
        /// <summary>
        /// Sets a list to database by a key.
        /// </summary>
        /// <param name="key">The database key.</param>
        /// <param name="value">The list.</param>
        void Set(string key, IList<string> value);

        /// <summary>
        /// Push a member to the end of list by a key.
        /// </summary>
        /// <param name="key">The database key.</param>
        /// <param name="member">The member.</param>
        void PushBack(string key, string member);

        /// <summary>
        /// Push a member to the front of list by a key.
        /// </summary>
        /// <param name="key">The database key.</param>
        /// <param name="member">The member.</param>
        void PushFront(string key, string member);

        /// <summary>
        /// Pop a member from the end of list by a key.
        /// </summary>
        /// <param name="key">The database key.</param>
        /// <returns>The member.</returns>
        string PopBack(string key);

        /// <summary>
        /// Pop a member from the front of list by a key.
        /// </summary>
        /// <param name="key">The databse key.</param>
        /// <returns>The member.</returns>
        string PopFront(string key);
        #endregion

        #region DB Hash Set
        /// <summary>
        /// Sets a set to database by a key.
        /// </summary>
        /// <param name="key">The database key.</param>
        /// <param name="value">The set.</param>
        void Set(string key, ISet<string> value);

        /// <summary>
        /// Add a member to a set.
        /// </summary>
        /// <param name="key">The databse key.</param>
        /// <param name="member">The member.</param>
        void Add(string key, string member);
        #endregion

        #region DB Ordered Set
        /// <summary>
        /// Sets an ordered set to database by a key.
        /// </summary>
        /// <param name="key">The databse key.</param>
        /// <param name="value">The ordered set.</param>
        void Set(string key, IDictionary<int, string> value);
        #endregion
    }
}
