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
    // Service class for handling Firebase authentication operations
    // Communicates with Firebase Identity Toolkit REST API
    public class FirebaseAuthService
    {
        // Field: Firebase project API key for authentication requests
        private readonly string _apiKey = "AIzaSyCWUMSuRvd6E71bxH_JqjQc9e9gwb7NEhI";
        // Field: HTTP client for making requests to Firebase API
        private readonly HttpClient _httpClient = new HttpClient();

        // Class: Represents the result of a Firebase sign-in operation
        public class FirebaseSignInResult
        {
            // Property: Firebase ID token for authenticated requests
            public string IdToken { get; set; }
            // Property: Email address of the authenticated user
            public string Email { get; set; }
            // Property: Token used to refresh the ID token when it expires
            public string RefreshToken { get; set; }
            // Property: Number of seconds until the ID token expires
            public string ExpiresIn { get; set; }
            // Property: Unique Firebase user ID (used to match with database)
            public string LocalId { get; set; }
            // Property: Error message if authentication failed (null if successful)
            public string ErrorMessage { get; set; }
            // Property: Returns true if no error message exists (sign-in succeeded)
            public bool Success { get { return string.IsNullOrEmpty(ErrorMessage); } }
        }

        // Method: Registers a new user with Firebase authentication
        // Returns a success message in Hebrew or an error message
        public async Task<string> SignUp(string email, string password)
        {
            try
            {
                // Build the request payload with user credentials
                var payload = new
                {
                    email = email,
                    password = password,
                    returnSecureToken = true
                };

                // Serialize payload to JSON and create HTTP content
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Build Firebase sign-up URL with API key
                string url = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=" + _apiKey;

                // Send POST request to Firebase
                var response = await _httpClient.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();

                // Return success or error message
                if (response.IsSuccessStatusCode)
                    return "ההרשמה בוצעה בהצלחה!";

                return "שגיאה בהרשמה: " + result;
            }
            catch (Exception ex)
            {
                // Return general error message if request fails
                return "שגיאה כללית: " + ex.Message;
            }
        }

        // Method: Signs in an existing user with Firebase authentication
        // Returns a FirebaseSignInResult with user data or error message
        public async Task<FirebaseSignInResult> SignIn(string email, string password)
        {
            try
            {
                // Build the request payload with user credentials
                var payload = new
                {
                    email = email,
                    password = password,
                    returnSecureToken = true
                };

                // Serialize payload to JSON and create HTTP content
                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Build Firebase sign-in URL with API key
                string url = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=" + _apiKey;

                // Send POST request to Firebase (synchronous wait)
                var response = _httpClient.PostAsync(url, content).Result;
                var resultJson = response.Content.ReadAsStringAsync().Result;

                // If response is not successful, return result with error message
                if (!response.IsSuccessStatusCode)
                {
                    return new FirebaseSignInResult
                    {
                        ErrorMessage = resultJson
                    };
                }

                // Deserialize Firebase response into FirebaseSignInResult object
                var data = JsonConvert.DeserializeObject<FirebaseSignInResult>(resultJson);

                // Return the sign-in result with user data
                return data;
            }
            catch (Exception ex)
            {
                // Return result with error message if request fails
                return new FirebaseSignInResult
                {
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}