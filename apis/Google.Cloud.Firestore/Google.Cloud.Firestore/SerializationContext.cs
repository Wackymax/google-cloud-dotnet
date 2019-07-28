namespace Google.Cloud.Firestore
{
    /// <summary>
    /// Serialization context to use when serializing values
    /// </summary>
    public sealed class SerializationContext
    {
        internal IValueSerializer Serializer { get; }

        /// <summary>
        /// Constructs a new context.
        /// </summary>
        internal SerializationContext(IValueSerializer valueSerializer)
        {
            Serializer = valueSerializer;
        }
    }
}
