using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Models
{
    public sealed class StringValueDictionary : IEquatable<StringValueDictionary>
    {
        private readonly Dictionary<string, string?> _dictionary = new();

        private StringValueDictionary(IDictionary<string, string?> dictionary)
        {
            foreach (var pair in dictionary)
            {
                _dictionary[pair.Key] = pair.Value;
            }
        }

        private StringValueDictionary(StringValueDictionary dictionary)
        {
            foreach (var key in dictionary.Keys)
            {
                _dictionary[key] = dictionary[key];
            }
        }

        public static StringValueDictionary Create(object obj)
        {
            if (obj is null) throw new ArgumentNullException(nameof(obj));
            if (obj is IDictionary<string, string?> dictionary)
            {
                return new StringValueDictionary(dictionary);
            }
            if (obj is IDictionary<string, object?> dictionary2)
            {
                return new StringValueDictionary(dictionary2.ToDictionary(p => p.Key, p => p.Value?.ToString()));
            }
            if (obj is StringValueDictionary dictionary3)
            {
                return new StringValueDictionary(dictionary3);
            }
            return new StringValueDictionary(obj.GetType().GetProperties()
                .ToDictionary(p => p.Name, p => p.GetValue(obj)?.ToString()));
        }

        public static StringValueDictionary CreateFromJsonString(string json)
        {
            Guard.NotNull(json, nameof(json));
            var dic = json.JsonToObject<Dictionary<string, object?>>()
                .ToDictionary(x => x.Key, x => x.Value?.ToString());
            return new StringValueDictionary(dic);
        }

        public int Count => _dictionary.Count;

        public bool ContainsKey(string key) => _dictionary.ContainsKey(key) ? _dictionary.ContainsKey(key) : throw new ArgumentOutOfRangeException(nameof(key));

        public string? this[string key] => _dictionary[key];

        public Dictionary<string, string>.KeyCollection Keys => _dictionary.Keys!;

        public bool Equals(StringValueDictionary? other)
        {
            if (other is null) return false;
            if (other.Count != Count) return false;
            foreach (var key in _dictionary.Keys)
            {
                if (!other.ContainsKey(key))
                {
                    return false;
                }
                if (_dictionary[key] != other[key])
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as StringValueDictionary);
        }

        public override int GetHashCode()
        {
            var stringBuilder = new StringBuilder();
            foreach (var pair in _dictionary)
            {
                stringBuilder.Append($"{pair.Key}={pair.Value}_");
            }
            return stringBuilder.ToString().GetHashCode();
        }

        public static bool operator ==(StringValueDictionary? current, StringValueDictionary? other)
        {
            return current?.Equals(other) == true;
        }

        public static bool operator !=(StringValueDictionary? current, StringValueDictionary? other)
        {
            return current?.Equals(other) != true;
        }

        public static implicit operator Dictionary<string, string?>(StringValueDictionary dictionary)
        {
            return dictionary._dictionary;
        }
    }
}
