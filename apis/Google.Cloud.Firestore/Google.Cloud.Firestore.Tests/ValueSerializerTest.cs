﻿// Copyright 2017, Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.Cloud.Firestore.V1;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Dynamic;
using Xunit;
using static Google.Cloud.Firestore.Tests.SerializationTestData;
using wkt = Google.Protobuf.WellKnownTypes;

namespace Google.Cloud.Firestore.Tests
{
    // Note: deliberately not using the CreateValue helper to make the protos absolutely clear.

    public class ValueSerializerTest
    {
        // Data that can only be serialized: we're passing in int values for the score; when we deserialize, we get back long values.
        // Likewise we can't deserialize anonymous types.
        public static IEnumerable<object[]> SerializeOnlyData { get; } = new List<object[]>
        {
            { new Dictionary<string, object> { { "name", "Jon" }, { "score", 10 } },
                new Value { MapValue = new MapValue { Fields = { { "name", new Value { StringValue = "Jon" } }, { "score", new Value { IntegerValue = 10L } } } } } },
            { () => { dynamic d = new ExpandoObject(); d.name = "Jon"; d.score = 10; return d; },
                new Value { MapValue = new MapValue { Fields = { { "name", new Value { StringValue = "Jon" } }, { "score", new Value { IntegerValue = 10L } } } } } },
            { new { name = "Jon", score = 10 },
                new Value { MapValue = new MapValue { Fields = { { "name", new Value { StringValue = "Jon" } }, { "score", new Value { IntegerValue = 10L } } } } } },

            // Sentinel values
            { new { name = "Jon", lastUpdate = FieldValue.ServerTimestamp, score = FieldValue.Delete },
                new Value { MapValue = new MapValue { Fields = { { "name", new Value { StringValue = "Jon" } }, { "lastUpdate", SentinelValue.ServerTimestamp.ToProtoValue() }, { "score", SentinelValue.Delete.ToProtoValue() } } } } },
            // There's no attribute for deleted fields, so the score is just propagated.
            { new SentinelModel { Name = "Jon", LastUpdate = new Timestamp(99, 99), Score = 10 },
                new Value { MapValue = new MapValue { Fields = { { "name", new Value { StringValue = "Jon" } }, { "lastUpdate", SentinelValue.ServerTimestamp.ToProtoValue() }, { "score", new Value { IntegerValue = 10L } } } } } },
        };

        public static IEnumerable<object[]> SerializeMapTestData { get; } = new List<object[]>
        {
            { new Dictionary<string, object> { { "name", "Jon" }, { "score", 10 }  },
                new Dictionary<string, Value> { { "name", new Value { StringValue = "Jon" } }, { "score", new Value { IntegerValue = 10L } } } },
            // Use SortedDictionary to prove we accept any IDictionary<string,*>, not just Dictionary<string,*>
            { new SortedDictionary<string, int> { { "x", 10 }, { "y", 20 }  },
                new Dictionary<string, Value> { { "x", new Value { IntegerValue = 10 } }, { "y", new Value { IntegerValue = 20L } } } },
            { new { name = "Jon", score = 10 },
                new Dictionary<string, Value> { { "name", new Value { StringValue = "Jon" } }, { "score", new Value { IntegerValue = 10L } } } },
            { new SerializationTestData.GameResult { Name = "Jon", Score = 10 },
                new Dictionary<string, Value> { { "name", new Value { StringValue = "Jon" } }, { "Score", new Value { IntegerValue = 10L } } } },
            { () => { dynamic d = new ExpandoObject(); d.name = "Jon"; d.score = 10; return d; },
                new Dictionary<string, Value> { { "name", new Value { StringValue = "Jon" } }, { "score", new Value { IntegerValue = 10L } } } },

            // Sentinel values
            { new { name = "Jon", lastUpdate = FieldValue.ServerTimestamp, score = FieldValue.Delete },
                new Dictionary<string, Value> { { "name", new Value { StringValue = "Jon" } }, { "lastUpdate", SentinelValue.ServerTimestamp.ToProtoValue() }, { "score", SentinelValue.Delete.ToProtoValue() } } },
            // There's no attribute for deleted fields, so the score is just propagated.
            { new SentinelModel { Name = "Jon", LastUpdate = new Timestamp(99, 99), Score = 10 },
                new Dictionary<string, Value> { { "name", new Value { StringValue = "Jon" } }, { "lastUpdate", SentinelValue.ServerTimestamp.ToProtoValue() }, { "score", new Value { IntegerValue = 10L } } } },
            
            // Custom conversion
            { new SerializationTestData.CustomUser { Name = "test", Email = new SerializationTestData.Email("test@example.com"), HighScore = 10 },
                new Dictionary<string, Value> { { "Name", new Value { StringValue = "test" } }, { "Email", new Value { StringValue = "test@example.com" } }, { "HighScore", new Value { IntegerValue = 10L } } } },
            // Non-null values for both non-nullable and nullable
            {
                new SerializationTestData.GuidPair { Name = "test1", Guid = Guid.Parse("a7dc91a0-ef9b-4fc7-9f03-1763d9688dfa"), GuidOrNull = Guid.Parse("5e124acc-c53e-4d47-bec8-a6618cf0b2d9") },
                new Dictionary<string, Value>
                {
                    { "Name", new Value { StringValue = "test1" } },
                    { "Guid", new Value { StringValue = "a7dc91a0ef9b4fc79f031763d9688dfa" } },
                    { "GuidOrNull", new Value { StringValue = "5e124accc53e4d47bec8a6618cf0b2d9" } }
                }
            },
            // Null value for the nullable property
            {
                new SerializationTestData.GuidPair { Name = "test2", Guid = Guid.Parse("a7dc91a0-ef9b-4fc7-9f03-1763d9688dfa"), GuidOrNull = null },
                new Dictionary<string, Value>
                {
                    { "Name", new Value { StringValue = "test2" } },
                    { "Guid", new Value { StringValue = "a7dc91a0ef9b4fc79f031763d9688dfa" } },
                    { "GuidOrNull", new Value { NullValue = wkt::NullValue.NullValue } }
                }
            }
        };

        [Theory]
        [MemberData(nameof(SerializationTestData.BclAndValues), MemberType = typeof(SerializationTestData))]
        [MemberData(nameof(SerializeOnlyData))]
        public void Serialize(object input, Value expectedOutput)
        {
            var actual = SerializationContext.Serializer.Serialize(SerializationContext, input);
            Assert.Equal(expectedOutput, actual);
        }

        [Theory]
        [MemberData(nameof(SerializationTestData.ProtoValues), MemberType = typeof(SerializationTestData))]
        public void ValueIsCloned(IMessage proto, Func<Value, IMessage> selector)
        {
            // Protos should be accepted, but cloned (as they're mutable, and we mutate things too)
            var value = SerializationContext.Serializer.Serialize(SerializationContext, proto);
            var actual = selector(value);
            Assert.NotSame(proto, actual);
            Assert.Equal(proto, actual);
        }

        [Fact]
        public void Serialize_Invalid()
        {
            // It's unlikely that we'll ever support serializing System.Type...
            Assert.Throws<ArgumentException>(() => SerializationContext.Serializer.Serialize(SerializationContext, typeof(ValueSerializer)));
        }

        [Theory]
        [MemberData(nameof(SerializeMapTestData))]
        public void SerializeMap(object input, Dictionary<string, Value> expectedOutput)
        {
            var actual = SerializationContext.Serializer.SerializeMap(SerializationContext, input);
            Assert.Equal(expectedOutput, actual);
        }

        [Theory]
        [MemberData(nameof(SerializeMapTestData))]
        public void SerializeValue_SameAsMap(object input, Dictionary<string, Value> expectedMap)
        {
            var actual = SerializationContext.Serializer.Serialize(SerializationContext, input);
            var expectedValue = new Value { MapValue = new MapValue { Fields = { expectedMap } } };
            Assert.Equal(expectedValue, actual);
        }

        [Fact]
        public void SerializeMap_Invalid()
        {
            // It's unlikely that we'll ever support serializing System.Type...
            Assert.Throws<ArgumentException>(() => SerializationContext.Serializer.Serialize(SerializationContext, typeof(ValueSerializer)));
        }

        [Fact]
        public void UInt64Overflow()
        {
            ulong value = long.MaxValue;
            value++;
            Assert.Throws<OverflowException>(() => SerializationContext.Serializer.Serialize(SerializationContext, value));
        }

        [Theory]
        [InlineData(DateTimeKind.Unspecified)]
        [InlineData(DateTimeKind.Local)]
        public void BadDateTimeKind(DateTimeKind kind)
        {
            var date = new DateTime(2017, 10, 6, 1, 2, 3, kind);
            Assert.Throws<ArgumentException>(() => SerializationContext.Serializer.Serialize(SerializationContext, date));
        }

        [Fact]
        public void ArrayInArray()
        {
            var badArray = new[] { new int[10] };
            Assert.Throws<ArgumentException>(() => SerializationContext.Serializer.Serialize(SerializationContext, badArray));
        }

        private SerializationContext SerializationContext => new SerializationContext(ValueSerializer.Instance);

        [FirestoreData]
        private class SentinelModel
        {
            [FirestoreProperty("name")]
            public string Name { get; set; }

            [FirestoreProperty("lastUpdate"), ServerTimestamp]
            public Timestamp LastUpdate { get; set; }

            [FirestoreProperty("score")]
            public int Score { get; set; }
        }
    }
}
