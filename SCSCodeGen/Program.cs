using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using SCSCodeGen.Services;
using Newtonsoft.Json;

namespace SCSCodeGen
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Starting SCS to T4 Code Generation ---");

            try
            {
                // 1. Setup Environment
                string baseExeDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string helixRootPath = FindHelixRoot(baseExeDirectory);
                string t4Folder = FindT4Folder(baseExeDirectory);
                string targetModule = args.Length > 0 ? args[0] : "All";
                var customMappings = LoadCustomMappings(baseExeDirectory);

                Console.WriteLine($"Helix Root Discovered: {helixRootPath}");
                Console.WriteLine($"Target: {(targetModule.Equals("All", StringComparison.OrdinalIgnoreCase) ? "All Modules" : targetModule)}\n");

                // 2. Resolve Services via Factory
                IServiceFactory factory = new ServiceFactory();
                ICodeGenerationService codeGenService = factory.CreateCodeGenerationService();

                // 3. Execute Pipeline
                codeGenService.ExecutePipeline(helixRootPath, t4Folder, targetModule, customMappings);

                Console.WriteLine("\nCode Generation Pipeline Completed!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[FATAL ERROR] Pipeline Failed: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }

        private static Dictionary<string, string> LoadCustomMappings(string baseDirectory)
        {
            string mappingsFile = Path.Combine(baseDirectory, "namespace-mappings.json");
            if (File.Exists(mappingsFile))
            {
                string json = File.ReadAllText(mappingsFile);
                var parsedJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                var mappings = parsedJson != null
                    ? new Dictionary<string, string>(parsedJson, StringComparer.OrdinalIgnoreCase)
                    : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                Console.WriteLine($"Loaded {mappings.Count} custom namespace mappings.");
                return mappings;
            }
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        private static string FindHelixRoot(string startPath)
        {
            var dir = new DirectoryInfo(startPath);
            while (dir != null)
            {
                if (Directory.Exists(Path.Combine(dir.FullName, "Foundation")) && Directory.Exists(Path.Combine(dir.FullName, "Feature")) &&
                    Directory.Exists(Path.Combine(dir.FullName, "Project"))) 
                {
                    return dir.FullName;
                }

                if (Directory.Exists(Path.Combine(dir.FullName, "src", "Foundation")) &&
                    Directory.Exists(Path.Combine(dir.FullName, "src", "Feature")) &&
                    Directory.Exists(Path.Combine(dir.FullName, "src", "Project"))) 
                {
                    return Path.Combine(dir.FullName, "src");
                }

                dir = dir.Parent;
            }
            throw new DirectoryNotFoundException("Could not auto-discover the Helix root.");
        }

        private static string FindT4Folder(string baseExeDirectory)
        {
            var localTemplate = Directory.GetFiles(baseExeDirectory + "T4Templates", "header.tt", SearchOption.AllDirectories).FirstOrDefault();
            if (localTemplate != null) return Path.GetDirectoryName(localTemplate);

            var dir = new DirectoryInfo(baseExeDirectory);
            while (dir != null)
            {
                var projectTemplate = Directory.GetFiles(dir.FullName, "header.tt", SearchOption.AllDirectories).FirstOrDefault();
                if (projectTemplate != null) return Path.GetDirectoryName(projectTemplate);

                if (Directory.Exists(Path.Combine(dir.FullName, "src")) || Directory.Exists(Path.Combine(dir.FullName, "Foundation"))) break;
                dir = dir.Parent;
            }

            throw new FileNotFoundException("Could not locate the T4 templates.");
        }
    }
}
