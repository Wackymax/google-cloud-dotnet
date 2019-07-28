﻿// Copyright 2019, Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.Api.Gax;

namespace Google.Cloud.Firestore
{
    /// <summary>
    /// Provides context for deserialization operations.
    /// </summary>
    public sealed class DeserializationContext
    {
        /// <summary>
        /// The database containing the document being deserialized. This is never null.
        /// </summary>
        internal FirestoreDb Database => DocumentReference.Database;

        internal IValueDeserializer Deserializer => Database.Deserializer;
        /// <summary>
        /// The document being deserialized. This is never null.
        /// </summary>
        internal DocumentReference DocumentReference { get; }

        /// <summary>
        /// Constructs a new context.
        /// </summary>
        /// <param name="reference">The document being deserialized. Must not be null.</param>
        internal DeserializationContext(DocumentReference reference)
        {
            DocumentReference = GaxPreconditions.CheckNotNull(reference, nameof(reference));
        }
    }
}
