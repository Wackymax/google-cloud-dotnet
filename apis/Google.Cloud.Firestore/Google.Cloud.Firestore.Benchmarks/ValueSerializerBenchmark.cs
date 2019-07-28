// Copyright 2018 Google LLC
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

using BenchmarkDotNet.Attributes;
using Google.Cloud.Firestore.V1;
using System.Collections.Generic;

namespace Google.Cloud.Firestore.Benchmarks
{
    public class ValueSerializerBenchmark
    {
        private SerializationContext SerializationContext => new SerializationContext(ValueSerializer.Instance);

        [Benchmark]
        public Dictionary<string, Value> SerializeMap_Attributed() =>
            SerializationContext.Serializer.SerializeMap(SerializationContext, SampleData.Attributed);

        [Benchmark]
        public Value Serialize_Attributed() =>
            SerializationContext.Serializer.Serialize(SerializationContext, SampleData.Attributed);

        [Benchmark]
        public Dictionary<string, Value> SerializeMap_Anonymous() =>
            SerializationContext.Serializer.SerializeMap(SerializationContext, SampleData.Anonymous);

        [Benchmark]
        public Value Serialize_Anonymous() =>
            SerializationContext.Serializer.Serialize(SerializationContext, SampleData.Anonymous);

        [Benchmark]
        public Dictionary<string, Value> SerializeMap_Dictionary() =>
            SerializationContext.Serializer.SerializeMap(SerializationContext, SampleData.Dictionary);

        [Benchmark]
        public Value Serialize_Dictionary() =>
            SerializationContext.Serializer.Serialize(SerializationContext, SampleData.Dictionary);

        [Benchmark]
        public Dictionary<string, Value> SerializeMap_Expando() =>
            SerializationContext.Serializer.SerializeMap(SerializationContext, SampleData.Expando);

        [Benchmark]
        public Value Serialize_Expando() =>
            SerializationContext.Serializer.Serialize(SerializationContext, SampleData.Expando);
    }
}
