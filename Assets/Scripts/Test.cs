using System;
using System.IO;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.TextCore.Text;
using VYaml.Annotations;
using VYaml.Emitter;
using VYaml.Parser;
using VYaml.Serialization;
using Object = UnityEngine.Object;

[Microsoft.CodeAnalysis.Generator]
public class VYamlIncrementalSourceGenerator : IIncrementalGenerator
{
    
}
internal class UnityFormatter : IYamlFormatter<Object>
{
    public void Serialize(ref Utf8YamlEmitter emitter, Object value, YamlSerializationContext context)
    {
    }

    public Object Deserialize(ref YamlParser parser, YamlDeserializationContext context)
    {
        throw new System.NotImplementedException();
    }
}

public class X
{
    public int Value = 1;
    public Vector3 Value2 = Vector3.up;
}

public class Test : MonoBehaviour
{
    [SerializeField] private FontAsset font;

    private async void Start()
    {
        var x = new Asset
        {
            FontAsset = font
        };
        // //AssetDatabase.GetMainAssetTypeFromGUID()
        // // var x = new X();
        // // var json = JsonConvert.SerializeObject(x);
        // var json = EditorJsonUtility.ToJson(x);
        // await File.WriteAllTextAsync(Path.Combine(Application.dataPath, "temp.json"), json);
        YamlSerializer.DefaultOptions.Resolver = CompositeResolver.Create(Array.Empty<IYamlFormatter>(),
            new IYamlFormatterResolver[]
            {
                MainResolver.Instance,
                StandardResolver.Instance
            });
        var yaml = YamlSerializer.SerializeToString(x);
        var yamlPath = Path.Combine(Application.dataPath, "temp.yaml");
        await File.WriteAllTextAsync(yamlPath, yaml);
        await using var s = File.OpenRead(yamlPath);
        var deserializeAsync = await YamlSerializer.DeserializeAsync<Asset>(s);
        
    }
}
[YamlObject]
partial class Asset
{
    public FontAsset FontAsset;
}