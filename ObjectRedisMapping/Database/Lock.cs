namespace Blueve.ObjectRedisMapping
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using StackExchange.Redis;

    /// <summary>
    /// The lock.
    /// </summary>
    internal class Lock : IDisposable
    {
        /// <summary>
        /// The database client.
        /// </summary>
        private readonly IDatabase db;

        /// <summary>
        /// The lock key.
        /// </summary>
        private readonly string key;

        /// <summary>
        /// The lock token.
        /// </summary>
        private readonly string token;

        /// <summary>
        /// Initialize an instance of <see cref="Lock"/>.
        /// </summary>
        /// <param name="db">The database client.</param>
        /// <param name="key">The lock key.</param>
        /// <param name="token">The lock token.</param>
        private Lock(IDatabase db, string key, string token)
        {
            this.db = db;
            this.key = key;
            this.token = token;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.db.LockRelease(this.key, this.token);
        }

        /// <summary>
        /// Take a lock.
        /// This method accept a cancellation token and 
        /// will try take the lock until get the lock successfully or 
        /// the cancellation requested from cancellation token.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <param name="key">The lock key.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns></returns>
        public static Lock Take(IDatabase db, string key, CancellationToken ct = default)
        {
            var lockKey = $"lock:{key}";
            var token = Guid.NewGuid().ToString();
            while (!db.LockTake(lockKey, token, TimeSpan.FromMinutes(1)))
            {
                ct.ThrowIfCancellationRequested();
                Task.Delay(TimeSpan.FromMilliseconds(500)).GetAwaiter().GetResult();
            }

            return new Lock(db, lockKey, token);
        }
    }
}
