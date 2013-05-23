using System;
using Raven.Imports.Newtonsoft.Json;

namespace DWH.Raven.Tests
{
    public class UriConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var uri = (Uri)value;
            serializer.Serialize(writer, uri == null ? null : uri.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var uriString = serializer.Deserialize<string>(reader);
            return ParseToUri(uriString);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Uri);
        }

        private static Uri ParseToUri(string uriString)
        {
            if (uriString == null)
                return null;

            if (Uri.IsWellFormedUriString(uriString, UriKind.Absolute))
                return new Uri(uriString, UriKind.Absolute);

            if (Uri.IsWellFormedUriString(uriString, UriKind.Relative))
                return new Uri(uriString, UriKind.Relative);

            return new Uri(uriString, UriKind.RelativeOrAbsolute);
        }

    }
}