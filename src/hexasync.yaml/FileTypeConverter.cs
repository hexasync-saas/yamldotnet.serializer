using System.IO;
using YamlDotNet.Serialization;
using System;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using System.Collections.Generic;
using System.Linq;

namespace hexasync.yaml
{
    public class FileTypeConverter : IYamlTypeConverter
    {
        private string filePath;
        private readonly IDeserializer deserializer;
        private readonly IEnumerable<ITag> tags;

        public FileTypeConverter(IDeserializer deserializer, IEnumerable<ITag> tags, string filePath)
        {
            this.filePath = filePath;
            this.deserializer = deserializer;
            this.tags = tags;
        }

        public bool Accepts(Type type)
        {
            return tags.Any(t => t.GetType().IsAssignableFrom(type));
        }

        public object ReadYaml(IParser parser, Type type)
        {
            var key = parser.Consume<Scalar>();
            FileAttributes attr = File.GetAttributes(filePath);
            string rootPath = string.Empty;
            if (attr.HasFlag(FileAttributes.Directory))
            {
                rootPath = filePath;
            }
            else
            {
                rootPath = Path.GetDirectoryName(filePath);
            }

            var targetFile = Path.Combine(rootPath, key.Value);
            var input = File.ReadAllText(targetFile);
            var tag = tags.FirstOrDefault(t => t.GetType() == type);
            var data = deserializer.Deserialize(input, tag.OutType);
            return data;
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
        }
    }
}