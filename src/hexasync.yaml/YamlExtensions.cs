using System.Dynamic;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace hexasync.yaml
{
    public static class YamlConfiguration
    {
        private static List<ITag> RegisteredTags { get; } = new List<ITag>();
        public static void RegisterTagsFromAssembly(Assembly assembly)
        {
            var type = typeof(ITag);
            var types = assembly.GetTypes()
                .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract && !p.IsGenericType);
            foreach (var t in types) {
                RegisteredTags.Add((ITag)Activator.CreateInstance(t));
            }
        }

        public static void RegisterTags(params ITag[] tag)
        {
            RegisteredTags.AddRange(tag);
        }

        public static void RegisterTag<TTag>()
            where TTag : ITag
        {
            RegisteredTags.Add(Activator.CreateInstance<TTag>());
        }

        public static IDeserializer GetDeserializer(string filePath = null)
        {
            var builder = new DeserializerBuilder();
            builder = builder
                .IgnoreFields()
                .IgnoreUnmatchedProperties()
                .WithNamingConvention(CamelCaseNamingConvention.Instance);
            if (RegisteredTags?.Count > 0) {
                foreach (var t in RegisteredTags) {
                    var type = t.GetType();
                    var tagKey = type.GetCustomAttributes(typeof(YamlTagAttribute), true)
                        .FirstOrDefault() is YamlTagAttribute att
                        ? att.TagName
                        : null;

                    builder = builder.WithTagMapping($"!{tagKey}", type);
                }

                builder = builder
                    .WithTypeConverter(
                    new FileTypeConverter(builder.Build(), RegisteredTags, filePath),
                    s => s.OnTop());
            }
            
            return builder.Build();
        }
    }
    public static class YamlExtensions
    {
        public static string SerializeToYaml(this object t)
        {
            if (t == null)
            {
                return null;
            }
            var target = JToken.FromObject(t);
            if (target is JValue)
            {
                return target.ToString();
            }
            var serializer = new SerializerBuilder()
                .IgnoreFields()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var result =
                target is JArray
                    ? serializer.Serialize(target.ToObject<List<ExpandoObject>>())
                    : serializer.Serialize(target.ToObject<ExpandoObject>());
            return result;
        }

        public static T DeserializeFromYaml<T>(this string t, string filePath = null)
        {
            return (T)DeserializeFromYaml(t, typeof(T), filePath);
        }

        public static object DeserializeFromYaml(this string t, Type to, string filePath = null)
        {
            var serializer = YamlConfiguration.GetDeserializer(filePath);
            var result = serializer.Deserialize(t, to);
            return result;
        }
    }
}