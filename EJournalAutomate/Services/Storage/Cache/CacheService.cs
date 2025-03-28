using EJournalAutomate.Models.Domain;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;

namespace EJournalAutomate.Services.Storage.Cache
{
    public class CacheService : ICacheService
    {
        private readonly string _savePath = Path.Combine(Environment.CurrentDirectory, "cache.json");
        private readonly ILogger<CacheService> _logger;

        public CacheService(ILogger<CacheService> logger)
        {
            _logger = logger;

            _logger.LogInformation("CacheService инициализирован");
        }

        public bool IsCacheAvailable => System.IO.File.Exists(_savePath);

        public async Task<List<User>> LoadCache()
        {
            _logger.LogInformation("Попытка загрузить данные из кэша");
            if (IsCacheAvailable)
            {
                string json = await System.IO.File.ReadAllTextAsync(_savePath);
                List<User> data = JsonSerializer.Deserialize<List<User>>(json);
                if (data != null && data.Count > 0)
                {
                    _logger.LogInformation("Данные из кэша успешно загружены");
                    return data;
                }
                else
                {
                    var ex = new FileFormatException("Файл с кэшем отсутсвует, загрузите данные из API");
                    _logger.LogError(ex, "Кэш не существует или пустой");
                    throw ex;
                }

            }
            else
            {
                var ex = new FileNotFoundException("Файл с кэшем отсутсвует, загрузите данные из API");
                _logger.LogError(ex, "Кэш не существует или пустой");
                throw ex;
            }
        }

        public async void SaveCache(List<User> data)
        {
            _logger.LogInformation("Попытка сохранить кэш");

            try
            {
                string json = JsonSerializer.Serialize<object>(data);
                await System.IO.File.WriteAllTextAsync(_savePath, json);
            }
            catch (Exception ex)
            {
                var exception = new Exception("Не удалось сохранить кэш", ex);
                _logger.LogError(exception, "Не удалось сохранить кэш");
                throw exception;
            }

            _logger.LogInformation("Кэш успешно сохранён");
        }
    }
}
