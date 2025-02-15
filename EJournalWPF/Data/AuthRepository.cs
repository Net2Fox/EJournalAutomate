using EJournalWPF.Data;
using EJournalWPF.Model.API.AuthModel;
using EJournalWPF.Model.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalAutomate.Data
{
    internal class AuthRepository
    {
        private string _authToken = string.Empty;

        internal string GetToken {  get { return _authToken; } }

        internal delegate void AuthHandler(bool isSuccess, string error);
        internal event AuthHandler AuthEvent;

        internal AuthRepository()
        {
            _authToken = TokenStorage.LoadToken();
        }

        internal async Task Auth(string login, string password)
        {
            Result<AuthResponse> result = await APIv3.Auth(login, password);
            if (result.Success)
            {
                AuthResponse authResult = result.Data;
                _authToken = authResult.Token;
                TokenStorage.SaveToken(_authToken);
                AuthEvent?.Invoke(true, null);
            }
            else
            {
                AuthEvent?.Invoke(false, result.Error);
            }
        }

        internal bool IsAuthorized()
        {
            if (_authToken != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
