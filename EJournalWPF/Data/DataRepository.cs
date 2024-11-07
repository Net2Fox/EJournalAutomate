using EJournalWPF.Model;
using EJournalWPF.Pages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Lifetime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EJournalWPF.Data
{
    internal class DataRepository
    {
        private static DataRepository _instance;
        private static readonly object _lock = new object();

        private readonly CookieContainer _cookies = new CookieContainer();

        private List<Group> _groups;
        private List<Student> _students;
        private List<Mail> _mails;

        private string _saveDirectory = $"{Environment.CurrentDirectory}\\Письма";

        public List<Group> Groups { get { return _groups; } }
        public List<Student> Students { get { return _students; } }
        public List<Mail> Mails { get { return _mails; } }

        public string SaveDirectory { get { return _saveDirectory; } }

        internal delegate void LoadDataSuccessHandler(List<Mail> mails);
        internal event LoadDataSuccessHandler LoadDataSuccessEvent;

        internal delegate void BeginDataLoadingHandler(string message);
        internal event BeginDataLoadingHandler BeginDataLoadingEvent;

        internal delegate void DataLoadingErrorHandler(string errorMsg);
        internal event DataLoadingErrorHandler DataLoadingErrorEvent;

        internal delegate void DownloadingFinishHandler();
        internal event DownloadingFinishHandler DownloadingFinishEvent;

        private DataRepository(List<CefSharp.Cookie> cefSharpCookies)
        {
            foreach (var cookie in cefSharpCookies)
            {
                _cookies.Add(new Uri("https://kip.eljur.ru"), new System.Net.Cookie(cookie.Name, cookie.Value));
            }
            
            Task.Run(async () =>
            {
                if (System.IO.File.Exists($"{Environment.CurrentDirectory}/cache.json"))
                {
                    await LoadCacheData();
                    return;
                }
                await LoadDataAPI();
            });
        }

        internal async Task LoadDataAPI()
        {
            await GetStudentsFromAPI();
            if (_students.Count > 0)
            {
                await GetMailsFromAPI();
            }
            else
            {
                DataLoadingErrorEvent?.Invoke("При загрузке списка студентов произошла ошибка, перезапустите программу!");
            }
            await SaveCacheData();
        }

        internal async Task GetStudentsFromAPI()
        {
            try
            {
                BeginDataLoadingEvent?.Invoke("Загрузка данных, пожалуйста, подождите...");
                JObject recipient_structure = JObject.Parse(await SendRequestAsync("https://kip.eljur.ru/journal-api-messages-action?method=messages.get_recipient_structure", _cookies));
                _groups = JsonConvert.DeserializeObject<List<Group>>(recipient_structure["structure"][0]["data"][5]["data"].ToString());

                _students = new List<Student>();
                var studentTasks = _groups.Select(async group =>
                {
                    JObject requset = JObject.Parse(await SendRequestAsync($"https://kip.eljur.ru/journal-api-messages-action?method=messages.get_recipients_list&key1=school&key2=students&key3=2024%2F2025_1_{System.Web.HttpUtility.UrlEncode(group.Name)}%23%23%23%23%23{group.Key}&dep=null", _cookies));
                    List<Student> students = JsonConvert.DeserializeObject<List<Student>>(requset["user_list"].ToString());
                    foreach (var student in students)
                    {
                        student.Group = group;
                    }
                    _students.AddRange(students);
                });

                await Task.WhenAll(studentTasks);
            }
            catch (Exception ex)
            {
                DataLoadingErrorEvent?.Invoke(ex.Message);
                return;
            }
        }

        internal List<Student> GetStudents()
        {
            return _students;
        }

        internal async Task GetMailsFromAPI(int limit = 20, int offset = 0, Status status = Status.all)
        {
            try
            {
                BeginDataLoadingEvent?.Invoke("Загрузка данных, пожалуйста, подождите...");
                string jsonResponse = await SendRequestAsync($"https://kip.eljur.ru/journal-api-messages-action?method=messages.get_list&category=inbox&search=&limit={limit}&offset={offset}&teacher=0&status={(status == Status.all ? "" : status.ToString())}&companion=&minDate=0", _cookies);
                JObject jsonData = JObject.Parse(jsonResponse);
                _mails = JsonConvert.DeserializeObject<List<Mail>>(jsonData["list"].ToString());
                _mails = _mails.Where(m => m.FromUser != null).ToList();
                LoadDataSuccessEvent?.Invoke(_mails);
            }
            catch (Exception ex)
            {
                DataLoadingErrorEvent?.Invoke(ex.Message);
                return;
            }
        }

        internal List<Mail> GetMails()
        {
            return _mails;
        }

        internal async Task<string> SendRequestAsync(string url, CookieContainer cookies)
        {
            using (HttpClientHandler handler = new HttpClientHandler { CookieContainer = cookies, UseCookies = true })
            using (HttpClient client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/118.0.0.0 Safari/537.36");
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        internal async void DownloadFile(List<Mail> mailsToDownload)
        {
            BeginDataLoadingEvent?.Invoke($"Скачивание {mailsToDownload.Count} писем...");
            
            if (!Directory.Exists(_saveDirectory))
            {
                Directory.CreateDirectory(_saveDirectory);
            }

            using (WebClient client = new WebClient())
            {
                foreach (var mail in mailsToDownload)
                {
                    string group = null;
                    string student = null;
                    string subDirectory = null;
                    string fileName = null;

                    group = mail.FromUser.Group.Name;
                    if (!Directory.Exists($"{_saveDirectory}/{group}"))
                    {
                        Directory.CreateDirectory($"{_saveDirectory}/{group}");
                    }

                    student = $"{mail.FromUser.LastName} {mail.FromUser.FirtsName}";
                    if (!Directory.Exists($"{_saveDirectory}/{group}/{student}"))
                    {
                        Directory.CreateDirectory($"{_saveDirectory}/{group}/{student}");
                    }

                    if (mail.Files.Count > 1)
                    {
                        subDirectory = Regex.Replace(mail.Subject, @"[<>:""|?*]", string.Empty);
                        if (subDirectory.Length > 30)
                        {
                            subDirectory = subDirectory.Remove(30);
                        }
                        if (!Directory.Exists($"{_saveDirectory}/{group}/{student}/{subDirectory}"))
                        {
                            Directory.CreateDirectory($"{_saveDirectory}/{group}/{student}/{subDirectory}");
                        }
                    }

                    foreach (var file in mail.Files)
                    {
                        fileName = Regex.Replace(file.Filename, @"[<>:""|?*]", string.Empty);
                        if (subDirectory != null)
                        {
                            fileName = $"{_saveDirectory}/{group}/{student}/{subDirectory}/{fileName}";
                        }
                        else
                        {
                            fileName = $"{_saveDirectory}/{group}/{student}/{fileName}";
                        }

                        if (System.IO.File.Exists(fileName))
                        {
                            break;
                        }

                        byte[] fileBytes = client.DownloadData(file.URL);
                        System.IO.File.WriteAllBytes(fileName, fileBytes);
                    }
                    await SendRequestAsync($"https://kip.eljur.ru/journal-api-messages-action?method=messages.note_read&idsString={mail.ID}", _cookies);
                    CheckStatusMail(mail.ID);
                }
            }
            DownloadingFinishEvent?.Invoke();
        }

        private void CheckStatusMail(long id)
        {
            Mail mail = _mails.Find(m => m.ID == id);
            mail.Status = Status.read;
            mail.IsSelected = false;
        }

        private async Task SaveCacheData()
        {
            CacheData cacheData = new CacheData(_students, _groups);
            System.IO.File.WriteAllText($"{Environment.CurrentDirectory}/cache.json", JsonConvert.SerializeObject(cacheData));
        }

        private async Task LoadCacheData()
        {
            try
            {
                CacheData cache = JsonConvert.DeserializeObject<CacheData>(System.IO.File.ReadAllText($"{Environment.CurrentDirectory}/cache.json"));
                _groups = cache.Groups;
                _students = cache.Students;
                if (_students.Count > 0)
                {
                    await GetMailsFromAPI();
                }
                else
                {
                    await LoadDataAPI();
                }
            }
            catch
            {
                await LoadDataAPI();
            }
        }

        internal void SetSaveDirectory(string directory)
        {
            _saveDirectory = directory;
        }

        public static void Initialize(List<CefSharp.Cookie> cefSharpCookies)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new DataRepository(cefSharpCookies);
                    }
                }
            }
        }
        
        public static DataRepository GetInstance()
        {
            if (_instance == null)
            {
                throw new InvalidOperationException("DataRepository не был инициализирован. Вызовите Initialize перед первым использованием.");
            }
            return _instance;
        }
    }
}
