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
        private readonly IEnumerable<ITag> tags;

        public FileTypeConverter(IEnumerable<ITag> tags, string filePath)
        {
            this.filePath = filePath;
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
            targetFile = Path.GetFullPath(targetFile);
            var input = File.ReadAllText(targetFile);
            var typeName = type.FullName;
            var tag = tags.FirstOrDefault(t => t.GetType().FullName == type.FullName);
            // var deserializer = YamlConfiguration.GetDeserializer(filePath);
            // var data = deserializer.Deserialize(input, tag.OutType);
            var nextDirectory = Path.GetDirectoryName(targetFile);
            var data = input.DeserializeFromYaml(tag.OutType, nextDirectory);
            return data;
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
        }
    }
}