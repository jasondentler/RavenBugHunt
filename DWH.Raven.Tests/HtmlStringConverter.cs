using System;
using System.Web.Mvc;
using Raven.Imports.Newtonsoft.Json;

namespace DWH.Raven.Tests
{
    public class HtmlStringConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var htmlString = (MvcHtmlString)value;
            serializer.Serialize(writer, htmlString.ToHtmlString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var htmlString = serializer.Deserialize<string>(reader);
            return MvcHtmlString.Create(htmlString);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MvcHtmlString);
        }
    }
}