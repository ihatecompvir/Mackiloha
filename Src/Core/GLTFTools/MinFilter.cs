﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GLTFTools
{
    [JsonConverter(typeof(MinFilterConverter))]
    public enum MinFilter : int
    {
        Nearest = 9728,
        Linear,
        NearestMipMapNearest = 9984,
        LinearMipMapNearest,
        NearestMipMapLinear,
        LinearMipMapLinear
    }

    internal class MinFilterConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.Integer)
                throw new JsonReaderException($"\'{reader.Path}\': Value must be a number!");

            var value = Convert.ToInt32(reader.Value);
            if (!Enum.IsDefined(typeof(MinFilter), value))
                throw new JsonReaderException($"\'{reader.Path}\': Value of \'{value}\' is not supported!");

            return (MinFilter)value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value.GetType() != typeof(MinFilter))
                throw new JsonWriterException($"\'{writer.Path}\': Value must be a MinFilter!");

            if (!Enum.IsDefined(typeof(MinFilter), value))
                throw new JsonWriterException($"\'{writer.Path}\': Value of \'{value}\' is not supported!");

            writer.WriteValue((int)value);
        }
    }
}
