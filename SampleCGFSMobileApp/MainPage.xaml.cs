using System;
using Microsoft.Maui.Controls;

namespace SampleCGFSMobileApp
{
    public partial class MainPage : ContentPage
    {
        private bool isRegistered;
        private bool isHeadOfHousehold;
        private bool canCheckIn;
        private string guid;
        private string birthdate;

        public MainPage()
        {
            InitializeComponent();
            LoadUserData();
            UpdateUI();
        }

        private async Task LoadUserData()
        {
            isRegistered = Preferences.ContainsKey("GUID");

            if (isRegistered)
            {
                guid = Preferences.Get("GUID", string.Empty);
                isHeadOfHousehold = Preferences.Get("IsHeadOfHousehold", false);
                birthdate = Preferences.Get("Birthdate", string.Empty);

                if (DateTime.TryParse(birthdate, out DateTime birthDateParsed))
                {
                    int age = DateTime.Now.Year - birthDateParsed.Year;
                    canCheckIn = age >= 18;
                }

                await DisplayAlert("Loaded Data", $"GUID: {guid}\nHead of Household: {isHeadOfHousehold}\nBirthdate: {birthdate}", "OK");
            }
        }

        private void UpdateUI()
        {
            StartRegistrationButton.IsVisible = !isRegistered;
            ProfileButton.IsVisible = isRegistered;
            CheckInButton.IsVisible = isRegistered && canCheckIn; // Only visible if user is registered and 18+
            CheckInButton.IsEnabled = canCheckIn;
            HouseholdButton.IsVisible = isRegistered && isHeadOfHousehold;
            AllocationsButton.IsVisible = isRegistered;
            ClearGuidButton.IsVisible = isRegistered;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadUserData();
            UpdateUI();
        }

        private async void OnStartRegistrationClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegistrationPage());
        }

        private void OnClearGuidClicked(object sender, EventArgs e)
        {
            Preferences.Clear(); // Clears all stored preferences
            LoadUserData();
            UpdateUI();
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Profile", "Profile clicked!", "OK");
        }

        private async void OnCheckInClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Check-In", "Check-In clicked!", "OK");
        }

        private async void OnHouseholdClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Household", "View Household clicked!", "OK");
        }

        private async void OnAllocationsClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Allocations", "View Allocations clicked!", "OK");
        }
    }
}
