using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Utilities.SerializationExtensionMethods
{
    /// <summary>
    /// Exposes extension methods to help with JSON serialization.
    /// </summary>
    public static class SerializationExtensionMethods
    {
        /// <summary>
        /// Serializes a given object into its JSON representation.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(this object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        /// <summary>
        /// Deserializes a JSON string into a given object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }


    }
}
