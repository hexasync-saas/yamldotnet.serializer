# yaml-dotnet-serializer

## Register custom tags
```C#
[YamlTag("customTag1")]
public sealed class YourCustomTag1 : ITag
{
    public Type OutType => typeof(string);
}

[YamlTag("customTag2")]
public sealed class YourCustomTag2 : ITag
{
    public Type OutType => typeof(string);
}

# Register instances
YamlConfiguration.RegisterTags(new YourCustomTag1(), new YourCustomTag2);

# Register generic
YamlConfiguration.RegisterTag<YourCustomTag1>();

# Register all tags that in assembly
YamlConfiguration.RegisterTagsFromAssembly(typeof(YourCustomTag).Assembly);
```

# Deserializer:
```C#
<the string>.DeserializeFromYaml(typeof(t), [nullable]"C:\\Path\To\The\File")
<the string>.DeserializeFromYaml<TGeneric>([nullable]"C:\\Path\To\The\File")
```

# Serialize:
```C#
TObject.SerializeToYaml()
```