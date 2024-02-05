using System.Collections.Generic;
using StarkMatchmaking;

namespace StarkNetwork
{
    public static class TypeTransformUtils
    {
        public static Dictionary<string, string> ToDictionary(this Map_string_string target)
        {
            var result = new Dictionary<string, string>();
            foreach (var keyValuePair in target)
            {
                result.Add(keyValuePair.Key, keyValuePair.Value);
            }
            return result;
        }

        public static List<ulong> ToList(this Vec_ulong target)
        {
            var result = new List<ulong>();
            foreach (var item in target)
            {
                result.Add(item);
            }
            return result;
        }
    }
}