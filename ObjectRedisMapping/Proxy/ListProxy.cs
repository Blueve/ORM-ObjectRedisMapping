namespace Blueve.ObjectRedisMapping.Proxy
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// The abstract list proxy.
    /// </summary>
    public abstract class ListProxy<T> : IList<T>, IProxy
    {
        /// <summary>
        /// Current prefix.
        /// </summary>
        protected readonly string prefix;

        /// <summary>
        /// The dynamic proxy stub.
        /// </summary>
        protected readonly DynamicProxyStub stub;

        /// <summary>
        /// Initialize an instance of list proxy.
        /// </summary>
        /// <param name="prefix">The base prefix.</param>
        /// <param name="stub">The stub.</param>
        public ListProxy(
            string prefix,
            DynamicProxyStub stub)
        {
            this.prefix = prefix;
            this.stub = stub;
        }

        /// <inheritdoc/>
        public T this[int index]
        {
            get
            {
                // TODO: User lock to handle the race condition.
                if (index >= this.Count)
                {
                    throw new IndexOutOfRangeException();
                }

                // Override following logic by using emit.
                return this.GetElemAt(index);
            }

            set
            {
                // TODO: User lock to handle the race condition.
                if (index >= this.Count)
                {
                    throw new IndexOutOfRangeException();
                }

                // Override following logic by using emit.
                this.SetElemAt(index, value);
            }
        }

        /// <inheritdoc/>
        public int Count => this.stub.Int32Getter(this.prefix);

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public void Add(T item)
        {
            // TODO: User lock to handle the race condition.
            this.SetElemAt(this.Count, item);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Get the element database key.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected string GetElemDbKey(int index)
        {
            return string.Concat(this.prefix, index.ToString());
        }

        /// <summary>
        /// Get the element which at the specificed index.
        /// </summary>
        /// <param name="index">The index number.</param>
        protected abstract T GetElemAt(int index);

        /// <summary>
        /// Set the value to specifieced index.
        /// </summary>
        /// <param name="index">The index number.</param>
        /// <param name="value">The value.</param>
        protected virtual void SetElemAt(int index, T value) { }
    }
}
