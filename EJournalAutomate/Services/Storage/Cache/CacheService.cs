using EJournalAutomateMVVM.Models.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace EJournalAutomate.Services.Storage.Cache
{
    public class CacheService : ICacheService
    {
        private readonly string _savePath = Path.Combine(Environment.CurrentDirectory, "cache.json");

        public bool IsCacheAvailable => System.IO.File.Exists(_savePath);

        public async Task<List<User>> LoadCache()
        {
            if (IsCacheAvailable)
            {
                string json = await System.IO.File.ReadAllTextAsync(_savePath);
                List<User> data = JsonSerializer.Deserialize<List<User>>(json);
                if (data != null && data.Count > 0)
                {
                    return data;
                }
                else
                {
                    throw new FileFormatException("Файл с кэшем отсутсвует, загрузите данные из API");
                }

            }
            else
            {
                throw new FileNotFoundException("Файл с кэшем отсутсвует, загрузите данные из API");
            }
        }

        public async void SaveCache(List<User> data)
        {

            string json = JsonSerializer.Serialize<object>(data);
            await System.IO.File.WriteAllTextAsync(_savePath, json);
        }
    }
}
