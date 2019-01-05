namespace Blueve.ObjectRedisMapping
{
    /// <summary>
    /// The formatter for the value of entity key.
    /// </summary>
    internal class EntityKeyValueFormatter
    {
        /// <summary>
        /// Serialize the string value. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Format(string value)
        {
            return $"{this.GenerateLengthBlock(value.Length)}{value}";
        }

        /// <summary>
        /// Generate the length block.
        /// The length block has a fixed length and record the length of the entity's value.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <returns>The length block.</returns>
        private string GenerateLengthBlock(int length)
        {
            return $"{length.ToString("X8")}";

            // TODO: Possiable a better way is save the length as binary format.
            //       For a Int32, it will take 4 byte rather than 8 byte
            //       but it will hard to read from Redis Explore directly.
            ////var bytes = BitConverter.GetBytes(length);
            ////var length = Encoding.UTF8.GetString(bytes);
        }
    }
}
