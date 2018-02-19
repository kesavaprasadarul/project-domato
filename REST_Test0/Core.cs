using Flurl.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace REST_Test0
{
    public static class Core
    {
        private static string ProjectID;
        private static string APIKey;
        public static User CurrentUser;

        #region PublicMethods

        public static void InitializeCore(string projectID, string apiKey)
        {
            ProjectID = projectID;
            APIKey = apiKey;
        }

        public static async Task SignUpWithEmailPasswordAsync(string email, string password)
        {
            CurrentUser = await AuthUsingEmailPassword(email, password, true);
        }

        public static async Task SignInWithEmailPasswordAsync(string email, string password)
        {
            CurrentUser = await AuthUsingEmailPassword(email, password);
        }

        public static async Task<User> RefreshUserToken(User _user)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));
                var keyValues = new Dictionary<string, object>()
                {
                    {"grant_type","refresh_token" },
                    {"refresh_token", _user.refreshToken }
                };
                var url = "https://securetoken.googleapis.com/v1/token?key=" + APIKey;
                try
                {
                    var refreshTokenResponse = await url
                   .WithHeader("Accept", "application/json")
                   .PostJsonAsync(keyValues)
                   .ReceiveJson<RefreshTokenResponse>();
                    _user.expiresIn = refreshTokenResponse.expires_in;
                    _user.refreshToken = refreshTokenResponse.refresh_token;
                    _user.idToken = refreshTokenResponse.id_token;
                    return _user;
                }
                catch (FlurlHttpException ex)
                {
                    var error = ex.GetResponseJson<ErrorBlock>();
                    Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(error.error.message);
                    await dialog.ShowAsync();
                }
                return null;
            }
        }

        public static async Task ResetPassword(string email)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));
                var keyValues = new Dictionary<string, object>()
                {
                    {"requestType","PASSWORD_RESET" },
                    {"email",email }
                };
                var url = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/getOobConfirmationCode?key=" + APIKey;
                try
                {
                    await url
                   .WithHeader("Accept", "application/json")
                   .PostJsonAsync(keyValues);
                    MessageDialog dialog = new MessageDialog("Please continue your process through the link that we sent to your E-Mail Address.");
                    await dialog.ShowAsync();
                }
                catch (FlurlHttpException ex)
                {
                    var error = ex.GetResponseJson<ErrorBlock>();
                    Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(error.error.message);
                    await dialog.ShowAsync();
                }
            }
        }

        public static async Task DeleteUserAccount(User _user)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));
                var keyValues = new Dictionary<string, object>()
                {
                    {"idToken",_user.idToken }
                };
                var url = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/deleteAccount?key=" + APIKey;
                try
                {
                    var refreshTokenResponse = await url
                   .WithHeader("Accept", "application/json")
                   .PostJsonAsync(keyValues)
                   .ReceiveJson<RefreshTokenResponse>();
                }
                catch (FlurlHttpException ex)
                {
                    var error = ex.GetResponseJson<ErrorBlock>();
                    Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(error.error.message);
                    await dialog.ShowAsync();
                }
            }
        }

        #endregion

        #region PrivateMethods

        private async static Task<User> AuthUsingEmailPassword(string email, string password, bool isSignUp = false)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Add(new HttpMediaTypeWithQualityHeaderValue("application/json"));
                var keyValues = new Dictionary<string, object>()
                {
                    {"email",email },
                    {"password", password },
                    {"returnSecureToken", true }
                };
                string url = string.Empty;
                if (isSignUp) url = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/signupNewUser?key=" + APIKey;
                else url = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=" + APIKey;
                User user = new User();
                try
                {
                    user = await url
                   .WithHeader("Accept", "application/json")
                   .PostJsonAsync(keyValues)
                   .ReceiveJson<User>();
                    user.isSuccess = true;
                }
                catch (FlurlHttpException ex)
                {
                    var error = ex.GetResponseJson<ErrorBlock>();
                    Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(error.error.message);
                    await dialog.ShowAsync();
                }
                return user;
            }
        }

        #endregion
    }

    public class RefreshTokenResponse
    {
        public string expires_in { get; set; }
        public string token_type { get; set; }
        public string refresh_token { get; set; }
        public string id_token { get; set; }
        public string user_id { get; set; }
        public string project_id { get; set; }
    }

    public class User
    {
        public string kind { get; set; }
        public string idToken { get; set; }
        public string email { get; set; }
        public string refreshToken { get; set; }
        public string expiresIn { get; set; }
        public string localId { get; set; }
        public bool registered { get; set; }
        public DateTime created = DateTime.Now;
        public bool isExpired
        {
            get
            {
                if (DateTime.Now > created.Add(new TimeSpan(0, 0, int.Parse(expiresIn)))) return true;
                else return false;
            }
        }
        public bool isSuccess { get; set; }
        public async void RefreshUserToken()
        {
            await Core.RefreshUserToken(this);
        }

        public async void DeleteAccount()
        {
            await Core.DeleteUserAccount(this);
        }
    }

    public class ErrorBlock
    {
        public ErrorMessage error { get; set; }
    }

    public class ErrorMessage
    {
        public Error[] errors { get; set; }
        public int code { get; set; }
        public string message { get; set; }
    }

    public class Error
    {
        public string domain { get; set; }
        public string reason { get; set; }
        public string message { get; set; }
    }

}
