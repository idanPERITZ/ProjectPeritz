using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WcfService
{
    public class FirebaseAuthService 
    {
        private readonly string _apiKey = "AIzaSyCWUMSuRvd6E71bxH_JqjQc9e9gwb7NEhI";
        private readonly HttpClient _httpClient = new HttpClient();

        public class FirebaseSignInResult
        {
            public string IdToken { get; set; }
            public string Email { get; set; }
            public string RefreshToken { get; set; }
            public string ExpiresIn { get; set; }
            public string LocalId { get; set; }

            public string ErrorMessage { get; set; }
            public bool Success { get { return string.IsNullOrEmpty(ErrorMessage); } }
        }

        public async Task<string> SignUp(string email, string password)
        {
            try
            {
                var payload = new
                {
                    email = email,
                    password = password,
                    returnSecureToken = true
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                string url = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=" + _apiKey;

                var response = await _httpClient.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return "ההרשמה בוצעה בהצלחה!";

                return "שגיאה בהרשמה: " + result;
            }
            catch (Exception ex)
            {
                return "שגיאה כללית: " + ex.Message;
            }
        }

        public async Task<FirebaseSignInResult> SignIn(string email, string password)
        {
            try
            {
                var payload = new
                {
                    email = email,
                    password = password,
                    returnSecureToken = true
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                string url = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=" + _apiKey;

                var response =  _httpClient.PostAsync(url, content).Result;
                var resultJson = response.Content.ReadAsStringAsync().Result;

                if (!response.IsSuccessStatusCode)
                {
                    return new FirebaseSignInResult
                    {
                        ErrorMessage = resultJson
                    };
                }

                // Deserialize Firebase response
                var data = JsonConvert.DeserializeObject<FirebaseSignInResult>(resultJson);

                return data;
            }

            catch (Exception ex)
            {
                return new FirebaseSignInResult
                {
                    ErrorMessage = ex.Message
                };
            }
        }

    }
}
