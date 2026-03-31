using SCSCodeGen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCSCodeGen.Services
{
    public interface ISitecoreMappingService
    {
        IEnumerable<SitecoreTemplate> MapToTemplates(IEnumerable<ScsYamlItem> rawItems, string defaultNamespace, Dictionary<string, string> namespaceMappings);
    }
}
