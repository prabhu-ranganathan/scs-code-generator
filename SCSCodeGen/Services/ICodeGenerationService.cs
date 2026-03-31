using System.Collections.Generic;

namespace SCSCodeGen.Services
{
    public interface ICodeGenerationService
    {
        void ExecutePipeline(string helixRootPath, string t4Folder, string targetModule, Dictionary<string, string> customMappings);
    }
}
