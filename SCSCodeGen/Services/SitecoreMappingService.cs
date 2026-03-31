using SCSCodeGen.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCSCodeGen.Services
{
    public class SitecoreMappingService : ISitecoreMappingService
    {
        private const string TemplateTemplateId = "ab86861a-6030-46c5-b394-e8f99e8b87db";
        private const string TemplateSectionId = "e269fbb5-3750-427a-9149-7aa950b49301";
        private const string TemplateFieldId = "455a3e98-a627-4b40-8035-e683a0331ac7";
        private const string FieldTypeFieldId = "ab162cc0-dc80-4abf-8871-998ee5d7ba32";
        private const string BaseTemplateFieldId = "12c33f3f-86c5-43a5-aeb4-5598cec45116";
        private const string RenderingParametersTemplateId = "8ca06d6a-b353-44e8-bc31-b528c7306971";

        public IEnumerable<SitecoreTemplate> MapToTemplates(IEnumerable<ScsYamlItem> rawItems, string defaultNamespace, Dictionary<string, string> namespaceMappings)
        {
            var allItems = rawItems.ToDictionary(x => x.ID, StringComparer.OrdinalIgnoreCase);
            var tdsTemplates = new Dictionary<string, SitecoreTemplate>(StringComparer.OrdinalIgnoreCase);

            var rawTemplates = allItems.Values.Where(x => CleanId(x.Template).Equals(TemplateTemplateId, StringComparison.OrdinalIgnoreCase));

            foreach (var rawTemplate in rawTemplates)
            {
                var sitecoreTemplate = new SitecoreTemplate
                {
                    PhysicalFilePath = rawTemplate.PhysicalFilePath,
                    ID = Guid.Parse(rawTemplate.ID),
                    Name = GetItemNameFromPath(rawTemplate.Path),
                    Path = rawTemplate.Path,
                    TemplateId = Guid.Parse(TemplateTemplateId),
                    TemplateName = "Template",
                    Namespace = GetNamespaceFromPath(rawTemplate.Path, namespaceMappings),
                    TargetProjectName = defaultNamespace
                };

                var sections = allItems.Values.Where(x => CleanId(x.Parent).Equals(CleanId(rawTemplate.ID), StringComparison.OrdinalIgnoreCase)
                                                       && CleanId(x.Template).Equals(TemplateSectionId, StringComparison.OrdinalIgnoreCase));

                foreach (var section in sections)
                {
                    var fields = allItems.Values.Where(x => CleanId(x.Parent).Equals(CleanId(section.ID), StringComparison.OrdinalIgnoreCase)
                                                         && CleanId(x.Template).Equals(TemplateFieldId, StringComparison.OrdinalIgnoreCase));

                    foreach (var rawField in fields)
                    {
                        sitecoreTemplate.Fields.Add(new SitecoreField
                        {
                            ID = Guid.Parse(rawField.ID),
                            Name = GetItemNameFromPath(rawField.Path),
                            Path = rawField.Path,
                            Type = GetFieldValue(rawField, FieldTypeFieldId) ?? "Single-Line Text"
                        });
                    }
                }
                tdsTemplates[rawTemplate.ID] = sitecoreTemplate;
            }

            ResolveBaseTemplates(tdsTemplates, allItems);
            return tdsTemplates.Values;
        }
        private void ResolveBaseTemplates(Dictionary<string, SitecoreTemplate> tdsTemplates, Dictionary<string, ScsYamlItem> allItems)
        {
            foreach (var kvp in tdsTemplates)
            {
                var rawTemplate = allItems[kvp.Key];
                string baseTemplateValue = GetFieldValue(rawTemplate, BaseTemplateFieldId);
                bool hasStandardRenderingBase = false;

                if (!string.IsNullOrWhiteSpace(baseTemplateValue))
                {
                    var baseIds = baseTemplateValue.Split(new[] { '|', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var baseId in baseIds)
                    {
                        string cleanBaseId = CleanId(baseId);
                        if (tdsTemplates.TryGetValue(cleanBaseId, out var baseTdsTemplate))
                        {
                            kvp.Value.BaseTemplates.Add(baseTdsTemplate);
                        }
                        else if (cleanBaseId.Equals(CleanId(RenderingParametersTemplateId), StringComparison.OrdinalIgnoreCase))
                        {
                            kvp.Value.BaseTemplates.Add(new SitecoreTemplate { ID = Guid.Parse(RenderingParametersTemplateId) });
                            hasStandardRenderingBase = true;
                        }
                    }
                }

                if (!hasStandardRenderingBase && kvp.Value.Name.EndsWith("Parameters", StringComparison.OrdinalIgnoreCase))
                {
                    if (!kvp.Value.BaseTemplates.Any(x => x.ID == Guid.Parse(RenderingParametersTemplateId)))
                    {
                        kvp.Value.BaseTemplates.Add(new SitecoreTemplate { ID = Guid.Parse(RenderingParametersTemplateId) });
                    }
                }
            }
        }

        private string GetFieldValue(ScsYamlItem item, string fieldId)
        {
            string cleanId = CleanId(fieldId);

            var field = item.SharedFields?.FirstOrDefault(f => CleanId(f.ID).Equals(cleanId, StringComparison.OrdinalIgnoreCase))
                     ?? item.UnversionedFields?.FirstOrDefault(f => CleanId(f.ID).Equals(cleanId, StringComparison.OrdinalIgnoreCase));

            if (field != null) return field.Value;

            if (item.Languages != null)
            {
                foreach (var version in item.Languages.Where(l => l.Versions != null).SelectMany(l => l.Versions))
                {
                    field = version.Fields?.FirstOrDefault(f => CleanId(f.ID).Equals(cleanId, StringComparison.OrdinalIgnoreCase));
                    if (field != null) return field.Value;
                }
            }
            return null;
        }

        private string CleanId(string id) => string.IsNullOrWhiteSpace(id) ? string.Empty : id.Replace("{", "").Replace("}", "").Trim();
        private string GetItemNameFromPath(string path) => path.Split('/').Last();

        private string GetNamespaceFromPath(string path, Dictionary<string, string> customMappings)
        {
            if (customMappings != null && customMappings.Any())
            {
                var match = customMappings
                    .Where(m => path.StartsWith(m.Key, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(m => m.Key.Length)
                    .FirstOrDefault();

                if (match.Key != null) return match.Value;
            }

            var segments = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            int startIndex = Array.FindIndex(segments, s => s.Equals("templates", StringComparison.OrdinalIgnoreCase));
            if (startIndex >= 0 && segments.Length > startIndex + 2)
            {
                return $"{segments[startIndex + 1]}.{segments[startIndex + 2]}";
            }
            return "";
        }
    }
}
