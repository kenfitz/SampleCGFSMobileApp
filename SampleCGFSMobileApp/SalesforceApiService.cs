using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SampleCGFSMobileApp.Services
{
    public class SalesforceApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _authUrl = "https://test.salesforce.com/services/oauth2/token";

        private string _accessToken;
        private string _instanceUrl;

        // Replace these with your actual Salesforce credentials
        private const string ClientId = "3MVG9U6cXtLQ6pLVLdDPH0OILg8cdlVyq7oyxHlh2r0dBYkB2qszyw9BDtLHQ.rYufU.MzxH6S9Lj.Qa0AyJs";
        private const string ClientSecret = "2A7B279136206A6101303B77B5BD58760A38F4F6D50B45A77DBFEE3E71EE4336";
        private const string Username = "integration_user@cgfs.com.stage";
        private const string Password = "K#3j@9zE!pWq7FgY$1R7KgrCLBFHakyJWTJAkhtW7xDj";

        public SalesforceApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<bool> AuthenticateAsync()
        {
            var requestBody = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("client_secret", ClientSecret),
                new KeyValuePair<string, string>("username", Username),
                new KeyValuePair<string, string>("password", Password)
            });

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(_authUrl, requestBody);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var authResult = JsonSerializer.Deserialize<SalesforceAuthResponse>(responseContent);
                    _accessToken = authResult.access_token;
                    _instanceUrl = authResult.instance_url;

                    Preferences.Set("AccessToken", _accessToken);
                    Preferences.Set("InstanceUrl", _instanceUrl);

                    return true;
                }
                else
                {
                    Console.WriteLine($"Salesforce Authentication Failed: {responseContent}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Authentication Error: {ex.Message}");
                return false;
            }
        }

        public async Task<RegisterDeviceResponse> RegisterDeviceAsync(string lastName, string birthdate, string phoneNumber)
        {
            _accessToken = Preferences.Get("AccessToken", string.Empty);
            _instanceUrl = Preferences.Get("InstanceUrl", string.Empty);

            if (string.IsNullOrEmpty(_accessToken) || string.IsNullOrEmpty(_instanceUrl))
            {
                bool authSuccess = await AuthenticateAsync();
                if (!authSuccess) throw new Exception("Failed to authenticate with Salesforce.");
            }

            var requestBody = new
            {
                lastName,
                birthdate, // Already formatted as YYYY-MM-DD before reaching here
                phoneNumber
            };

            string jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            string endpoint = $"{_instanceUrl}/services/apexrest/contact/registerdevice/v1/";

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(endpoint, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<RegisterDeviceResponse>(responseContent);
                }
                else
                {
                    Console.WriteLine($"RegisterDevice Failed: {responseContent}");
                    throw new Exception($"Error: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Request Error: {ex.Message}");
                throw;
            }
        }

        private class SalesforceAuthResponse
        {
            public string access_token { get; set; }
            public string instance_url { get; set; }
        }

        public class RegisterDeviceResponse
        {
            public string guid { get; set; }
            public string phoneNumber { get; set; }
            public string birthdate { get; set; }
            public bool headOfHousehold { get; set; }
        }
    }
}
