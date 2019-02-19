namespace Blueve.RedisEmulator
{
    using System.Threading.Tasks;
    using StackExchange.Redis;

    internal class Transaction : Batch, ITransaction
    {
        public Transaction(RedisDatabase db)
            : base(db)
        {
        }

        public ConditionResult AddCondition(Condition condition)
        {
            return null;
        }

        public bool Execute(CommandFlags flags = CommandFlags.None)
        {
            return true;
        }

        public Task<bool> ExecuteAsync(CommandFlags flags = CommandFlags.None)
        {
            return Task.FromResult(true);
        }
    }
}
