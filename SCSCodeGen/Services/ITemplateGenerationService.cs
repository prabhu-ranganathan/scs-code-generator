using SCSCodeGen.Models;
using System.Collections.Generic;

namespace SCSCodeGen.Services
{
    public interface ITemplateGenerationService
    {
        void Generate(IEnumerable<SitecoreTemplate> templates, string t4Directory, string outputFile, string defaultNamespace);
    }
}
