using SCSCodeGen.Models;
using System.Collections.Generic;

namespace SCSCodeGen.Services
{
    public interface IYamlDataService
    {
        IEnumerable<ScsYamlItem> ReadAllItems(string directoryPath);
    }
}