using SCSCodeGen.Models;
using Mono.TextTemplating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SCSCodeGen.Services
{
    public class TemplateGenerationService : ITemplateGenerationService
    {
        public void Generate(IEnumerable<SitecoreTemplate> templates, string t4Directory, string outputFile, string defaultNamespace)
        {
            var generator = new TemplateGenerator();
            generator.IncludePaths.Add(t4Directory);
            generator.Refs.Add(typeof(SitecoreItem).Assembly.Location);
            generator.Refs.Add(typeof(Enumerable).Assembly.Location);

            StringBuilder finalOutput = new StringBuilder();
            string tempOutputFile = Path.GetTempFileName();

            try
            {
                var headerSession = generator.GetOrCreateSession();
                headerSession["Model"] = new ProjectHeader { BaseNamespace = "" };
                headerSession["DefaultNamespace"] = defaultNamespace;

                if (generator.ProcessTemplateAsync(Path.Combine(t4Directory, "header.tt"), tempOutputFile).Result)
                {
                    finalOutput.AppendLine(File.ReadAllText(tempOutputFile));
                }
                else
                {
                    PrintT4Errors("header.tt", generator);
                }

                foreach (var template in templates)
                {
                    var itemSession = generator.GetOrCreateSession();
                    itemSession["Model"] = template;
                    itemSession["DefaultNamespace"] = defaultNamespace;

                    if (generator.ProcessTemplateAsync(Path.Combine(t4Directory, "item.tt"), tempOutputFile).Result)
                    {
                        finalOutput.AppendLine(File.ReadAllText(tempOutputFile));
                    }
                    else
                    {
                        PrintT4Errors($"item.tt (Template: {template.Name})", generator);
                    }
                }

                string cleanedOutput = Regex.Replace(finalOutput.ToString(), @"(\r\n|\n|\r){3,}", Environment.NewLine + Environment.NewLine);
                File.WriteAllText(outputFile, cleanedOutput);
            }
            finally
            {
                if (File.Exists(tempOutputFile)) File.Delete(tempOutputFile);
            }
        }

        private void PrintT4Errors(string templateName, TemplateGenerator generator)
        {
            Console.WriteLine($"\n[T4 COMPILATION FAILED] -> {templateName}");
            foreach (System.CodeDom.Compiler.CompilerError error in generator.Errors)
            {
                Console.WriteLine($"  Line {error.Line}: {error.ErrorText}");
            }
        }
    }
}
