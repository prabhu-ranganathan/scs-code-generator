using System;
using System.Collections.Generic;

namespace SCSCodeGen.Models
{
    public class SitecoreItem
    {
        public string PhysicalFilePath { get; set; }
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Namespace { get; set; }
        public SitecoreItem Parent { get; set; }
        public Guid TemplateId { get; set; }
        public string TemplateName { get; set; }
        public Dictionary<string, string> SitecoreFields { get; set; } = new Dictionary<string, string>();
        public string TargetProjectName { get; set; } = string.Empty;
    }

    public class SitecoreTemplate : SitecoreItem
    {
        public List<SitecoreField> Fields { get; set; } = new List<SitecoreField>();
        public List<SitecoreTemplate> BaseTemplates { get; set; } = new List<SitecoreTemplate>();
        public string Data { get; set; } = string.Empty;
    }

    public class SitecoreField : SitecoreItem
    {
        public string Type { get; set; }
        public string Data { get; set; } = string.Empty;
    }

    public class ProjectHeader
    {
        public string BaseNamespace { get; set; }
    }
}
