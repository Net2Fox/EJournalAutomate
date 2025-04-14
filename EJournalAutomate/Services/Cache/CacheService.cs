using EJournalAutomate.Exceptions;
using EJournalAutomate.Models.Domain;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;

namespace EJournalAutomate.Services.Cache
{
    public class CacheService : ICacheService
    {
        private readonly string _savePath = Path.Combine(Environment.CurrentDirectory, "cache.json");
        private readonly ILogger<CacheService> _logger;

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            IncludeFields = true
        };

        public CacheService(ILogger<CacheService> logger)
        {
            _logger = logger;

            _logger.LogInformation("CacheService инициализирован");
        }

        public bool IsCacheAvailable => System.IO.File.Exists(_savePath);

        public async Task<(List<User>, List<StudentGroup>)> LoadCache()
        {
            _logger.LogInformation("Попытка загрузить данные из кэша");
            if (IsCacheAvailable)
            {
                string json = await System.IO.File.ReadAllTextAsync(_savePath);
                if (IsCacheValid(json))
                {
                    (List<User> users, List<StudentGroup> groups) = JsonSerializer.Deserialize<(List<User>, List<StudentGroup>)>(json, _jsonOptions);
                    if (users != null && users.Count > 0 && groups != null && groups.Count > 0)
                    {
                        _logger.LogInformation("Данные из кэша успешно загружены");
                        return (users, groups);
                    }
                    else
                    {
                        var ex = new EmptyFileException("Файл с кэшем пустой");
                        _logger.LogError(ex, "Кэш пустой");
                        throw ex;
                    }
                }
                else
                {
                    var ex = new FileFormatException("Файл с кэшем повреждён");
                    _logger.LogError(ex, "Кэш повреждён");
                    throw ex;
                }
            }
            else
            {
                var ex = new FileNotFoundException("Файл с кэшем отсутствует");
                _logger.LogError(ex, "Кэш отсутствует");
                throw ex;
            }
        }

        public async void SaveCache(List<User> users, List<StudentGroup> groups)
        {
            _logger.LogInformation("Попытка сохранить кэш");

            try
            {
                string json = JsonSerializer.Serialize<(List<User>, List<StudentGroup>)>((users, groups), _jsonOptions);
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

        public bool IsCacheValid(string json)
        {
            try
            {
                using var jsonDoc = JsonDocument.Parse(json);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
