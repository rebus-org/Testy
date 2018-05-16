using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Testy.Extensions
{
    /// <summary>
    /// Nifty JSON extensions
    /// </summary>
    public static class JsonExtensions
    {
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
    }
}