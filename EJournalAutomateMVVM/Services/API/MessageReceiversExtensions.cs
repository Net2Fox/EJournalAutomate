using EJournalAutomateMVVM.Models.API.Responses;
using EJournalAutomateMVVM.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EJournalAutomateMVVM.Services.API
{
    public static class MessageReceiversExtensions
    {
        public static List<Group> GetStudentGroups(this MessageReceiversResult messageReceivers)
        {
            var studentsGroup = messageReceivers.Groups.FirstOrDefault(g => string.Equals(g.Key, "students", StringComparison.OrdinalIgnoreCase));
            return studentsGroup.SubGroups;
        }

        public static Dictionary<string, List<User>> GetAllStudents(this MessageReceiversResult messageReceivers, string groupName = null)
        {
            var result = new Dictionary<string, List<User>>();
            var studentGroups = messageReceivers.GetStudentGroups();

            foreach (var group in studentGroups)
            {
                if (!string.IsNullOrEmpty(groupName) && !string.Equals(group.Name, groupName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (group.Users != null && group.Users.Any())
                {
                    result[group.Name] = group.Users;
                }

                if (!string.IsNullOrEmpty(groupName) && result.ContainsKey(groupName))
                {
                    break;
                }
            }

            return result;
        }

        public static List<User> GetStudentsList(this MessageReceiversResult messageReceivers)
        {
            return messageReceivers.GetStudentGroups()
                .Where(g => g.Users != null)
                .SelectMany(g => g.Users.Select(u => {
                    u.GroupName = g.Name;
                    return u;
                }))
                .ToList();
        }

        public static async Task<List<User>> ExtractStudentsDirectlyAsync(string jsonContent)
        {
            var students = new List<User>();

            // Обрабатываем JSON как документ для оптимизации
            using JsonDocument document = JsonDocument.Parse(jsonContent);

            // Извлекаем только нужную часть JSON
            JsonElement root = document.RootElement;

            // Навигация к группе "students"
            if (root.TryGetProperty("response", out JsonElement response) &&
                response.TryGetProperty("result", out JsonElement result) &&
                result.TryGetProperty("groups", out JsonElement groups))
            {
                // Ищем группу students
                foreach (JsonElement group in groups.EnumerateArray())
                {
                    if (group.TryGetProperty("key", out JsonElement keyElement) &&
                        keyElement.GetString() == "students" &&
                        group.TryGetProperty("subgroups", out JsonElement subgroups))
                    {
                        // Обрабатываем каждую подгруппу
                        foreach (JsonElement subgroup in subgroups.EnumerateArray())
                        {
                            string groupName = string.Empty;
                            if (subgroup.TryGetProperty("name", out JsonElement nameElement))
                            {
                                groupName = nameElement.GetString();
                            }

                            if (subgroup.TryGetProperty("users", out JsonElement users))
                            {
                                foreach (JsonElement userElement in users.EnumerateArray())
                                {
                                    // Создаем объект пользователя
                                    var user = new User
                                    {
                                        GroupName = groupName
                                    };

                                    // Заполняем данные пользователя
                                    if (userElement.TryGetProperty("name", out JsonElement nameEl))
                                        user.Name = nameEl.ValueKind == JsonValueKind.Number ?
                                            nameEl.GetInt32().ToString() : nameEl.GetString();

                                    if (userElement.TryGetProperty("lastname", out JsonElement lastnameEl))
                                        user.LastName = lastnameEl.GetString();

                                    if (userElement.TryGetProperty("firstname", out JsonElement firstnameEl))
                                        user.FirstName = firstnameEl.GetString();

                                    if (userElement.TryGetProperty("middlename", out JsonElement middlenameEl))
                                        user.MiddleName = middlenameEl.GetString();

                                    students.Add(user);
                                }
                            }
                        }

                        // После обработки всех студентов можно прервать цикл
                        break;
                    }
                }
            }

            return students;
        }
    }
}
