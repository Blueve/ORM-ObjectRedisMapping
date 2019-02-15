namespace Blueve.ObjectRedisMapping.UnitTests.Model
{
    using System.Collections.Generic;

    public class ListObject : IEntityKey
    {
        public virtual IList<int> Numbers { get; set; }

        public string GetKey()
        {
            return string.Join("|", this.Numbers);
        }
    }
}
