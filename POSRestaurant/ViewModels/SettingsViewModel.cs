using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using POSRestaurant.ChangedMessages;
using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using POSRestaurant.Service.LoggerService;

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
        /// DIed LogService
        /// </summary>
        private readonly LogService _logger;

        /// <summary>
        /// To indicate that the ViewModel data is loading
        /// </summary>
        [ObservableProperty]
        private bool _isLoading;

        /// <summary>
        /// To list the different settings options we have
        /// </summary>
        [ObservableProperty]
        private ValueForPickerSelection[] _settings = [];

        /// <summary>
        /// Property to observe the selected category on UI
        /// </summary>
        [ObservableProperty]
        private ValueForPickerSelection _selectedSetting;

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
        /// To know if the restaurant info is initialized
        /// </summary>
        public bool InfoInitialized = false;

        /// <summary>
        /// To show the restaurant info
        /// </summary>
        [ObservableProperty]
        private bool _showInfo;

        /// <summary>
        /// To show the expense types
        /// </summary>
        [ObservableProperty]
        private bool _showExpenseTypes;

        /// <summary>
        /// A list of expense type to display
        /// </summary>
        [ObservableProperty]
        private ExpenseTypes[] _expenseTypesToDisplay = [];

        /// <summary>
        /// Constructor for the view model
        /// </summary>
        /// <param name="databaseService">DIed Database Service</param>
        public SettingsViewModel(LogService logger, DatabaseService databaseService)
        {
            _logger = logger;
            _databaseService = databaseService;
        }

        /// <summary>
        /// To Initialize the Page components and update the data
        /// </summary>
        /// <returns></returns>
        public async ValueTask InitializeAsync()
        {
            try
            {
                Settings = EnumExtensions.GetAllDescriptions<ApplicationSettings>()
                                .Select(ValueForPickerSelection.FromEntity)
                                .ToArray();
                Settings[0].IsSelected = true;
                SelectedSetting = Settings[0];
                ShowInfo = true;
                ShowExpenseTypes = false;

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
            catch (Exception ex)
            {
                _logger.LogError("SettingsVM-InitializeAsync Error", ex);
                throw;
            }
        }

        /// <summary>
        /// When add new expense type button is clicked, take value and add new expense type
        /// </summary>
        /// <returns>Returns a Task Object</returns>
        [RelayCommand]
        private async Task AddExpenseType()
        {
            try
            {
                var result = await Shell.Current.DisplayPromptAsync("ExpenseType", "Enter the Expense Type name.", placeholder: "Enter Expense Type Name");
                if (!string.IsNullOrWhiteSpace(result))
                {
                    var errorMessage = await _databaseService.SettingsOperation.SaveExpenseType(result);
                    if (errorMessage == null)
                    {
                        await Shell.Current.DisplayAlert("Success", $"{result} Added Successfully", "OK");
                        await LoadExpenseTypes();
                        WeakReferenceMessenger.Default.Send(ExpenseTypeChangedMessage.From(true));
                        return;
                    }
                    await Shell.Current.DisplayAlert("Error", errorMessage, "OK");
                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SettingsVM-AddExpenseType Error", ex);
                throw;
            }
        }

        /// <summary>
        /// Change data as per selected category
        /// Working as a relay command
        /// </summary>
        /// <param name="settingId">CategoryId of the MenuCategory selected</param>
        [RelayCommand]
        private async Task SelectSettingAsync(int settingId)
        {
            try
            {
                if (SelectedSetting.Key == settingId) return;

                IsLoading = true;

                var existingSelectedCategory = Settings.First(o => o.IsSelected);
                existingSelectedCategory.IsSelected = false;

                var newSelectedCategory = Settings.First(o => o.Key == settingId);
                newSelectedCategory.IsSelected = true;

                SelectedSetting = newSelectedCategory;

                switch (SelectedSetting.Key)
                {
                    case (int)ApplicationSettings.RestaurantInfo:
                        ShowInfo = true;
                        ShowExpenseTypes = false;
                        break;
                    case (int)ApplicationSettings.ExpenseItems:
                        await LoadExpenseTypes();
                        ShowInfo = false;
                        ShowExpenseTypes = true;
                        break;
                }

                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("SettingsVM-SelectSettingAsync Error", ex);
                throw;
            }
        }

        /// <summary>
        /// To load all the expense types
        /// </summary>
        /// <returns>Returns a task</returns>
        private async Task LoadExpenseTypes()
        {
            try
            {
                IsLoading = true;

                ExpenseTypesToDisplay = await _databaseService.SettingsOperation.GetExpenseTypes();

                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("SettingsVM-LoadExpenseTypes Error", ex);
                throw;
            }
        }

        /// <summary>
        /// Command to delete the expense type
        /// </summary>
        /// <param name="expenseType">Expense type to delete</param>
        /// <returns>Returns a task</returns>
        [RelayCommand]
        private async Task DeleteExpenseTypeAsync(ExpenseTypes expenseType)
        {
            try
            {
                if (await Shell.Current.DisplayAlert("Delete?", $"Do you really want to delete this type?", "Yes", "No"))
                {
                    var errorMessage = await _databaseService.SettingsOperation.DeleteExpenseType(expenseType);

                    if (errorMessage != null)
                    {
                        await Shell.Current.DisplayAlert("Error", errorMessage, "OK");
                        return;
                    }
                    await LoadExpenseTypes();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("SettingsVM-DeleteExpenseTypeAsync Error", ex);
                throw;
            }
        }

        /// <summary>
        /// Command to chagne the selection of the 
        /// </summary>
        [RelayCommand]
        private async Task SaveRestaurantInfoAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Name))
                {
                    await Shell.Current.DisplayAlert("Invalid Input", "Please enter restaurant name", "Ok");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Address))
                {
                    await Shell.Current.DisplayAlert("Invalid Input", "Please enter restaurant address", "Ok");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Phone))
                {
                    await Shell.Current.DisplayAlert("Invalid Input", "Please enter restaurant contact number", "Ok");
                    return;
                }

                if (string.IsNullOrWhiteSpace(Fassai))
                {
                    await Shell.Current.DisplayAlert("Invalid Input", "Please enter restaurant FASSAI number", "Ok");
                    return;
                }

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

                InfoInitialized = true;

                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("SettingsVM-SaveRestaurantInfoAsync Error", ex);
                throw;
            }
        }
    }
}
