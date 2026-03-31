using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace SCSCodeGen.Services
{
    public class CodeGenerationService : ICodeGenerationService
    {
        private readonly IYamlDataService _dataService;
        private readonly ISitecoreMappingService _mappingService;
        private readonly ITemplateGenerationService _generationService;
        private const string DefaultNamespace = "yourdomain";

        public CodeGenerationService(
            IYamlDataService dataService,
            ISitecoreMappingService mappingService,
            ITemplateGenerationService generationService)
        {
            _dataService = dataService;
            _mappingService = mappingService;
            _generationService = generationService;
        }
        public void ExecutePipeline(string helixRootPath, string t4Folder, string targetModule, Dictionary<string, string> customMappings)
        {
            Console.WriteLine($"Reading all SCS YAML globally from: {helixRootPath}");
            var globalRawItems = _dataService.ReadAllItems(helixRootPath);

            Console.WriteLine("Mapping global Sitecore inheritance graph...");
            var globalTemplates = _mappingService.MapToTemplates(globalRawItems, DefaultNamespace, customMappings).ToList();

            string[] layers = { "Foundation", "Feature", "Project" };

            foreach (var layer in layers)
            {
                string layerPath = Path.Combine(helixRootPath, layer);
                if (!Directory.Exists(layerPath)) continue;

                foreach (var moduleDir in Directory.GetDirectories(layerPath))
                {
                    string moduleName = new DirectoryInfo(moduleDir).Name;
                    string currentModuleIdentifier = $"{layer}.{moduleName}";

                    if (!targetModule.Equals("All", StringComparison.OrdinalIgnoreCase) &&
                        !currentModuleIdentifier.Equals(targetModule, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    string serializationPath = Path.Combine(moduleDir, "serialization");
                    string codePath = Path.Combine(moduleDir, "code");

                    if (!Directory.Exists(serializationPath) || !Directory.Exists(codePath)) continue;

                    var moduleTemplates = globalTemplates.Where(t =>
                        !string.IsNullOrEmpty(t.PhysicalFilePath) &&
                        t.PhysicalFilePath.StartsWith(serializationPath, StringComparison.OrdinalIgnoreCase)
                    ).ToList();

                    if (!moduleTemplates.Any()) continue;

                    Console.WriteLine($"Processing Module: {currentModuleIdentifier}");
                    string outputFile = Path.Combine(codePath, "Template.cs");
                    Console.WriteLine($"  -> Generating {moduleTemplates.Count()} templates to {outputFile}");

                    _generationService.Generate(moduleTemplates, t4Folder, outputFile, DefaultNamespace);
                }
            }
        }
    }
}
