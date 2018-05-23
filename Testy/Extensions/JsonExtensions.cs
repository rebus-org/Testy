using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Testy.Extensions
{
    /// <summary>
    /// Nifty JSON extensions
    /// </summary>
    public static class JsonExtensions
    {
        static readonly JsonSerializerSettings FullTypeNameInfo = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        /// <summary>
        /// Serializes the object to JSON
        /// </summary>
        public static string ToJson(this object obj) => JsonConvert.SerializeObject(obj);

        /// <summary>
        /// Serializes the object to indented JSON
        /// </summary>
        public static string ToPrettyJson(this object obj)
        {
            if (obj is string text && text.IsJson())
            {
                return JsonConvert.DeserializeObject<JObject>(text)
                    .ToString(Formatting.Indented);
            }

            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        /// <summary>
        /// Checks whether the given <paramref name="text"/> contains valid JSON
        /// </summary>
        public static bool IsJson(this string text)
        {
            try
            {
                JObject.Parse(text);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Serializes and deserialized the given <paramref name="instance"/>, effectively cloning the object.
        /// Uses <see cref="TypeNameHandling.All"/> to preserve as much .NET type information as possible.
        /// Will of course not work, if the object is not JSON roundtrippable.
        /// </summary>
        public static T Clone<T>(this T instance) => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(instance, FullTypeNameInfo), FullTypeNameInfo);
    }
}