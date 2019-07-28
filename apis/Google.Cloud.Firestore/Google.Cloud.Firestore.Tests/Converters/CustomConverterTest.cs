﻿// Copyright 2018, Google LLC
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

using Google.Cloud.Firestore.Converters;
using Google.Cloud.Firestore.V1;
using System;
using System.Collections.Generic;
using Xunit;

namespace Google.Cloud.Firestore.Tests.Converters
{
    /// <summary>
    /// Tests for CustomConverter, mostly for error checking. Most tests for custom serialization
    /// are in <see cref="ValueSerializerTest"/> and <see cref="ValueDeserializerTest"/>.
    /// </summary>
    public class CustomConverterTest
    {
        [Fact]
        public void NullReturnsProhibited()
        {
            var db = FirestoreDb.Create("proj", "db", new FakeFirestoreClient());
            var context = new DeserializationContext(db.Document("a/b"));
            var converter = CustomConverter.ForConverterType(typeof(NullReturningConverter), typeof(string));
            Assert.Throws<InvalidOperationException>(() => converter.Serialize(SerializationContext, ""));
            Assert.Throws<InvalidOperationException>(() => converter.SerializeMap(SerializationContext, "", new Dictionary<string, Value>()));
            Assert.Throws<InvalidOperationException>(() => converter.DeserializeValue(context, new Value { StringValue = "" }));
            Assert.Throws<InvalidOperationException>(() => converter.DeserializeMap(context, new Dictionary<string, Value>()));
        }

        [Fact]
        public void InvalidConverterType()
        {
            Assert.Throws<InvalidOperationException>(() => CustomConverter.ForConverterType(typeof(NullReturningConverter), typeof(int)));
        }

        public class NullReturningConverter : IFirestoreConverter<string>
        {
            public string FromFirestore(object value) => null;
            public object ToFirestore(string value) => null;
        }
        private SerializationContext SerializationContext => new SerializationContext(ValueSerializer.Instance);
    }
}
