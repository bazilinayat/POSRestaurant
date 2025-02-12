using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.Models;

namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// View Model for settings page
    /// </summary>
    public partial class SettingsViewModel : ObservableObject
    {
        /// <summary>
        /// DIed variable for DatabaseService
        /// </summary>
        private readonly DatabaseService _databaseService;

        /// <summary>
        /// To indicate that the ViewModel data is loading
        /// </summary>
        [ObservableProperty]
        private bool _isLoading;

        /// <summary>
        /// To show the list of settings the user can change
        /// </summary>
        public List<TextSelectModel> SettingsToSet;

        /// <summary>
        /// Property to observe the selected setting on UI
        /// </summary>
        [ObservableProperty]
        private TextSelectModel _selectedSetting;

        /// <summary>
        /// To update gst number on the application
        /// </summary>
        [ObservableProperty]
        private string _gstIn;

        /// <summary>
        /// To update fassai number on the application
        /// </summary>
        [ObservableProperty]
        private string _fassai;

        /// <summary>
        /// To update restaurant name on the application
        /// </summary>
        [ObservableProperty]
        private string _name;

        /// <summary>
        /// To update restaurant address on the application
        /// </summary>
        [ObservableProperty]
        private string _address;

        /// <summary>
        /// To update restaurant phone number on the application
        /// </summary>
        [ObservableProperty]
        private string _phone;

        /// <summary>
        /// Constructor for the view model
        /// </summary>
        /// <param name="databaseService">DIed Database Service</param>
        public SettingsViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            SettingsToSet = new List<TextSelectModel>
            {
                new TextSelectModel
                {
                    Text = "Restaurant Info",
                    IsSelected = false,
                },
                new TextSelectModel
                {
                    Text = "GST",
                    IsSelected = false,
                },
                new TextSelectModel
                {
                    Text = "Fassai",
                    IsSelected = false,
                }
            };
        }

        /// <summary>
        /// To Initialize the Page components and update the data
        /// </summary>
        /// <returns></returns>
        public async ValueTask InitializeAsync()
        {
            var restaurantInfo = await _databaseService.SettingsOperation.GetRestaurantInfo();

            if (restaurantInfo == null)
            {
                GstIn = Fassai = Address = Phone = "";
            }
            else
            {
                GstIn = restaurantInfo.GSTIN;
                Fassai = restaurantInfo.FSSAI;
                Name = restaurantInfo.Name;
                Address = restaurantInfo.Address;
                Phone = restaurantInfo.PhoneNumber;
            }
        }

        /// <summary>
        /// Command to chagne the selection of the 
        /// </summary>
        [RelayCommand]
        private async Task SaveRestaurantInfoAsync()
        {
            var info = new RestaurantInfo
            {
                GSTIN = GstIn,
                FSSAI = Fassai,
                Name = Name,
                Address = Address,
                PhoneNumber = Phone,
            };
            
            IsLoading = true;

            var errorMessage = await _databaseService.SettingsOperation.SaveRestaurantInfo(info);

            if (errorMessage != null)
            {
                await Shell.Current.DisplayAlert("Save Error", "Error Saving Restaurant Info", "Ok");
                return;
            }

            await Shell.Current.DisplayAlert("Successful", "Restaurant Info Save Successfully", "Ok");

            IsLoading = false;
        }
    }
}
