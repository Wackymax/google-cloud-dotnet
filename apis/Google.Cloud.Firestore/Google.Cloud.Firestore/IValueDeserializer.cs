using System.Collections.Generic;
using Google.Cloud.Firestore.V1;

namespace Google.Cloud.Firestore
{
    /// <summary>
    /// Interface to handle value deserialization
    /// </summary>
    public interface IValueDeserializer
    {
        /// <summary>
        /// Deserializes from a Firestore Value proto to a .NET type.
        /// </summary>
        /// <param name="context">The context for the deserialization operation. Never null.</param>
        /// <param name="value">The value to deserialize. Must not be null.</param>
        /// <param name="targetType">The target type. The method tries to convert to this type. If the type is
        /// object, it uses the default representation of the value.</param>
        /// <returns>The deserialized value</returns>
        object Deserialize(DeserializationContext context, Value value, System.Type targetType);

        /// <summary>
        /// Deserializes dictionary from a Firestore proto to a .NET type.
        /// </summary>
        /// <param name="context">The context for the deserialization operation. Never null.</param>
        /// <param name="values">The dictionary to deserialize. Must not be null.</param>
        /// <param name="targetType">The target type. The method tries to convert to this type. If the type is
        /// object, it uses the default representation of the value.</param>
        /// <returns>The deserialized value</returns>
        object DeserializeMap(DeserializationContext context, IDictionary<string, Value> values, System.Type targetType);
    }
}
