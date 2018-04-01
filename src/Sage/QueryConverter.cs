using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Sage
{
    class QueryConverter : JsonConverter<List<Query>>
    {
        public override bool CanRead => true;
        public override bool CanWrite => false;

        public override List<Query> ReadJson(JsonReader reader, Type objectType, List<Query> existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                var queries = serializer.Deserialize<Query[]>(reader);
                return new List<Query>(queries);
            }
            else if (reader.TokenType == JsonToken.StartObject)
            {
                var query = serializer.Deserialize<Query>(reader);
                return new List<Query>
                {
                    query
                };
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override void WriteJson(JsonWriter writer, List<Query> value, JsonSerializer serializer) =>
            throw new NotImplementedException();
    }
}
