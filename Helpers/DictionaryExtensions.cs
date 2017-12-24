using System.Collections.Generic;

namespace UnifiControllerCommunicator.Helpers
{
    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : class
        {
            TValue val;
            if(dictionary.TryGetValue(key, out val))
            {
                return val;
            }
            return default(TValue);
        }

        public static TValue? GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue? defaultValue) where TValue : struct
        {
            TValue val;
            if (dictionary.TryGetValue(key, out val))
            {
                return val;
            }
            return defaultValue;
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue) where TValue : struct
        {
            TValue val;
            if (dictionary.TryGetValue(key, out val))
            {
                return val;
            }
            return defaultValue;
        }
    }
}
