using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using POSRestaurant.ChangedMessages;
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
        /// To know if restaurant is using Gst on the application
        /// </summary>
        [ObservableProperty]
        private bool _usingGST;

        /// <summary>
        /// To update gst number on the application
        /// </summary>
        [ObservableProperty]
        private string _gstIn;

        /// <summary>
        /// To update cgst number on the application
        /// </summary>
        [ObservableProperty]
        private decimal _cgst;

        /// <summary>
        /// To update sgst number on the application
        /// </summary>
        [ObservableProperty]
        private decimal _sgst;

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
                UsingGST = false;
                GstIn = Fassai = Address = Phone = "";
                Cgst = Sgst = 0;
            }
            else
            {
                Fassai = restaurantInfo.FSSAI;
                Name = restaurantInfo.Name;
                Address = restaurantInfo.Address;
                Phone = restaurantInfo.PhoneNumber;

                UsingGST = restaurantInfo.UsingGST;
                GstIn = restaurantInfo.GSTIN;
                Cgst = restaurantInfo.CGST;
                Sgst = restaurantInfo.SGST;
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
                FSSAI = Fassai,
                Name = Name,
                Address = Address,
                PhoneNumber = Phone,

                UsingGST = UsingGST,
                GSTIN = GstIn,
                CGST = Cgst,
                SGST = Sgst
            };
            
            IsLoading = true;

            var errorMessage = await _databaseService.SettingsOperation.SaveRestaurantInfo(info);

            if (errorMessage != null)
            {
                await Shell.Current.DisplayAlert("Save Error", "Error Saving Restaurant Info", "Ok");
                return;
            }

            await Shell.Current.DisplayAlert("Successful", "Restaurant Info Save Successfully", "Ok");

            WeakReferenceMessenger.Default.Send(TaxChangedMessage.From(true));

            IsLoading = false;
        }
    }
}
