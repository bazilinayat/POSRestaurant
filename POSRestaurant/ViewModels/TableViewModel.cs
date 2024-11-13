using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using POSRestaurant.Data;
using POSRestaurant.Models;
using POSRestaurant.Pages;
using POSRestaurant.Utility;
using System.Windows.Input;

namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// ViewModel For Table Page
    /// </summary>
    public partial class TableViewModel : ObservableObject
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
        /// To check if ViewModel is already initialized
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// To get and display all the categories
        /// Made observable to use everywhere
        /// </summary>
        [ObservableProperty]
        private TableModel[] _tables = [];

        /// <summary>
        /// ServiceProvider for the DIs
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// DIed OrdersViewModel
        /// </summary>
        private readonly OrdersViewModel _ordersViewModel;

        /// <summary>
        /// DIed SettingService
        /// </summary>
        private readonly SettingService _settingService;

        /// <summary>
        /// DIed HomeViewModel
        /// </summary>
        private readonly HomeViewModel _homeViewModel;

        public ICommand NavigateToProfileCommand { get; }
        public ICommand NavigateToSettingsCommand { get; }

        /// <summary>
        /// Constructor for the TablesViewModel
        /// </summary>
        /// <param name="databaseService">DI for DatabaseService</param>
        /// <param name="homeViewModel">DI for HomeViewModel</param>
        /// <param name="ordersViewModel">DI for OrdersViewModel</param>
        /// <param name="settingService">DI for SettingService</param>
        public TableViewModel(IServiceProvider serviceProvider, DatabaseService databaseService, HomeViewModel homeViewModel, OrdersViewModel ordersViewModel, SettingService settingService)
        {
            _serviceProvider = serviceProvider;

            _databaseService = databaseService;
            _homeViewModel = homeViewModel;
            _ordersViewModel = ordersViewModel;
            _settingService = settingService;

            NavigateToProfileCommand = new Command(async () => await NavigateToProfile());
            NavigateToSettingsCommand = new Command(async () => await NavigateToSettings());
        }

        private async Task NavigateToProfile()
        {
            // Navigate to ProfilePage
            var tabv = _serviceProvider.GetRequiredService<OrdersViewModel>();
            await Application.Current.MainPage.Navigation.PushAsync(new OrdersPage(tabv));
        }

        private async Task NavigateToSettings()
        {
            // Navigate to SettingsPage
            await Application.Current.MainPage.Navigation.PushAsync(new ManageMenuItemPage());
        }

        /// <summary>
        /// Initialize the ViewModel
        /// Fetch data and assign
        /// </summary>
        /// <returns>Returns a Task object</returns>
        public async ValueTask InitializeAsync()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;

            IsLoading = true;

            Tables = (await _databaseService.GetTablesAsync())
                            .Select(TableModel.FromEntity)
                            .ToArray();

            IsLoading = false;
        }

        [RelayCommand]
        private async Task TableSelected(TableModel tableModel)
        {

        }
    }
}
