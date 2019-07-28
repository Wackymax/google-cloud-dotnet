using System.Collections.Generic;
using Google.Cloud.Firestore.V1;

namespace Google.Cloud.Firestore
{
    /// <summary>
    /// Interface to handle value serialization
    /// </summary>
    public interface IValueSerializer
    {
        /// <summary>
        /// Serializes a single input to a Value.
        /// </summary>
        /// <remarks>
        /// It's important that this always clones any mutable values - which is really only
        /// relevant when the input is already a proto. That allows the caller to then mutate the result
        /// where appropriate.
        /// </remarks>
        /// <param name="serializationContext">Serialization content to use when serializing values</param>
        /// <param name="value">The value to serialize.</param>
        /// <returns>A Firestore Value proto.</returns>
        Value Serialize(SerializationContext serializationContext, object value);

        /// <summary>
        /// Serializes a map-based input to a dictionary of fields to values.
        /// This is effectively the map-only part of <see cref="ValueSerializer.Serialize"/>, but without wrapping the
        /// result in a Value.
        /// </summary>
        Dictionary<string, Value> SerializeMap(SerializationContext serializationContext, object value);
    }
}
