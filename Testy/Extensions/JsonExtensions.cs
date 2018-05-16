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
        public static string ToPrettyJson(this object obj) => JsonConvert.SerializeObject(obj, Formatting.Indented);

        /// <summary>
        /// Pretty-formats the given JSON text
        /// </summary>
        public static string IndentJson(this string jsonText) => JObject.Parse(jsonText).ToString(Formatting.Indented);

        /// <summary>
        /// Serializes and deserialized the given <paramref name="instance"/>, effectively cloning the object.
        /// Uses <see cref="TypeNameHandling.All"/> to preserve as much .NET type information as possible.
        /// Will of course not work, if the object is not JSON roundtrippable.
        /// </summary>
        public static T Clone<T>(this T instance) => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(instance, FullTypeNameInfo), FullTypeNameInfo);
    }
}