using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using SampleCGFSMobileApp.Services;

namespace SampleCGFSMobileApp
{
    public partial class RegistrationPage : ContentPage
    {
        private readonly SalesforceApiService _salesforceApiService;

        public RegistrationPage()
        {
            InitializeComponent();
            _salesforceApiService = new SalesforceApiService();
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            string lastName = LastNameEntry.Text?.Trim();
            string birthdateInput = BirthdateEntry.Text?.Trim();
            string phoneInput = PhoneNumberEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(birthdateInput) ||
                string.IsNullOrWhiteSpace(phoneInput))
            {
                await DisplayAlert("Error", "Last Name, Birthdate, and Phone Number are required.", "OK");
                return;
            }

            // Convert MM/DD/YYYY to YYYY-MM-DD
            if (!DateTime.TryParseExact(birthdateInput, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthdateParsed))
            {
                await DisplayAlert("Error", "Invalid birthdate format. Use MM/DD/YYYY.", "OK");
                return;
            }
            // Ensure the string is exactly "yyyy-MM-dd" (no time)
            string formattedBirthdate = birthdateParsed.ToString("yyyy-MM-dd");

            // Extract only digits from the phone number input
            string digitsOnly = new string(phoneInput.Where(char.IsDigit).ToArray());
            if (digitsOnly.Length != 10)
            {
                await DisplayAlert("Error", "Please enter a valid 10-digit phone number.", "OK");
                return;
            }

            try
            {
                var response = await _salesforceApiService.RegisterDeviceAsync(lastName, formattedBirthdate, digitsOnly);

                // Save values from the service response
                Preferences.Set("GUID", response.guid);
                Preferences.Set("Birthdate", response.birthdate);
                Preferences.Set("IsHeadOfHousehold", response.headOfHousehold);
                Preferences.Set("PhoneNumber", response.phoneNumber);

                await DisplayAlert("Registration Complete",
                    $"GUID: {response.guid}\nPhone: {response.phoneNumber}\nHead of Household: {response.headOfHousehold}",
                    "OK");

                await Navigation.PopAsync(); // Return to main page
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to register: {ex.Message}", "OK");
            }
        }
    }
}
