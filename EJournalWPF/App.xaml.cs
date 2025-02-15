using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using EJournalWPF.Data;
using EJournalAutomate.Data;

namespace EJournalWPF
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private AuthRepository _auth;
        private SettingsRepository _settings;
        private DataRepository _data;

        internal AuthRepository GetAuthRepository { get { return _auth; } }
        internal SettingsRepository GetSettingsRepository { get { return _settings; } }
        internal DataRepository GetDataRepository { get { return _data; } }

        public App()
        {
            _auth = new AuthRepository();
            _settings = new SettingsRepository();
            _data = new DataRepository(_auth, _settings);
        }
    }
}
