using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace SCSCodeGen.Models
{
    public class ScsYamlItem
    {
        [YamlIgnore] public string PhysicalFilePath { get; set; }

        [YamlMember(Alias = "ID")] public string ID { get; set; }
        [YamlMember(Alias = "Parent")] public string Parent { get; set; }
        [YamlMember(Alias = "Template")] public string Template { get; set; }
        [YamlMember(Alias = "Path")] public string Path { get; set; }
        [YamlMember(Alias = "SharedFields")] public List<ScsYamlField> SharedFields { get; set; } = new List<ScsYamlField>();
        [YamlMember(Alias = "UnversionedFields")] public List<ScsYamlField> UnversionedFields { get; set; } = new List<ScsYamlField>();
        [YamlMember(Alias = "Languages")] public List<ScsYamlLanguage> Languages { get; set; } = new List<ScsYamlLanguage>();
    }

    public class ScsYamlLanguage
    {
        [YamlMember(Alias = "Language")] public string Language { get; set; }
        [YamlMember(Alias = "Versions")] public List<ScsYamlVersion> Versions { get; set; } = new List<ScsYamlVersion>();
    }

    public class ScsYamlVersion
    {
        [YamlMember(Alias = "Version")] public int Version { get; set; }
        [YamlMember(Alias = "Fields")] public List<ScsYamlField> Fields { get; set; } = new List<ScsYamlField>();
    }

    public class ScsYamlField
    {
        [YamlMember(Alias = "ID")] public string ID { get; set; }
        [YamlMember(Alias = "Hint")] public string Hint { get; set; }
        [YamlMember(Alias = "Value")] public string Value { get; set; }
    }


}