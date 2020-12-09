using System;

namespace hexasync.yaml
{
    public sealed class YamlTagAttribute: Attribute
    {
        public string TagName { get; }
        public YamlTagAttribute(string tagName)
        {
            TagName = tagName;
        }
    }
}