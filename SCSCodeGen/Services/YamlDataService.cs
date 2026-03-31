using SCSCodeGen.Models;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;

namespace SCSCodeGen.Services
{
    public class YamlDataService : IYamlDataService
    {
        public IEnumerable<ScsYamlItem> ReadAllItems(string directoryPath)
        {
            var deserializer = new DeserializerBuilder().IgnoreUnmatchedProperties().Build();
            var items = new List<ScsYamlItem>();

            foreach (var file in Directory.GetFiles(directoryPath, "*.yml", SearchOption.AllDirectories))
            {
                try
                {
                    var item = deserializer.Deserialize<ScsYamlItem>(File.ReadAllText(file));
                    if (item != null && !string.IsNullOrEmpty(item.ID))
                    {
                        item.PhysicalFilePath = file;
                        items.Add(item);
                    }
                }
                catch { /* Skip unparseable files */ }
            }
            return items;
        }
    }
}