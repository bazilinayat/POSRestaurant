using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using POSRestaurant.Controls;
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
        /// DIed HomeViewModel
        /// </summary>
        private readonly HomeViewModel _homeViewModel;

        /// <summary>
        /// DIed SettingService
        /// </summary>
        private readonly SettingService _settingService;

        /// <summary>
        /// Constructor for the TablesViewModel
        /// </summary>
        /// <param name="serviceProvider">DI for IServiceProvider</param>
        /// <param name="databaseService">DI for DatabaseService</param>
        /// <param name="homeViewModel">DI for HomeViewModel</param>
        /// <param name="ordersViewModel">DI for OrdersViewModel</param>
        /// <param name="settingService">DI for SettingService</param>
        public TableViewModel(IServiceProvider serviceProvider, DatabaseService databaseService, HomeViewModel homeViewModel, OrdersViewModel ordersViewModel, SettingService settingService)
        {
            _serviceProvider = serviceProvider;
            _databaseService = databaseService;
            _ordersViewModel = ordersViewModel;
            _homeViewModel = homeViewModel;
            _settingService = settingService;
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

            await GetTablesAsync();

            IsLoading = false;
        }

        /// <summary>
        /// To get or update the table details
        /// </summary>
        /// <returns>Returns a Task Object</returns>
        public async ValueTask GetTablesAsync()
        {
            var tables = (await _databaseService.GetTablesAsync())
                            .Select(TableModel.FromEntity)
                            .ToList();

            Tables = tables.ToArray();

            TransformTables(tables);
        }

        /// <summary>
        /// To be called each time we get the tables from database
        /// This will transform and modify the data in table list, to be used for the UI
        /// </summary>
        /// <param name="tables">List of TableModel</param>
        private void TransformTables(List<TableModel> tables)
        {
            foreach(var table in tables)
            {
                switch(table.Status)
                {
                    case TableOrderStatus.NoOrder:
                        // Used to reset everything
                        table.BorderColour = "Brown";
                        table.ActionButtonImageIcon = "check_circle_regular_24.png";
                        table.ActionButtonEnabled = false;
                        break;
                    case TableOrderStatus.Running:
                        table.BorderColour = "Yellow";
                        table.ActionButtonImageIcon = "invoice.png";
                        table.ActionButtonEnabled = true;
                        break;
                    case TableOrderStatus.Printed:
                        table.BorderColour = "Green";
                        table.ActionButtonImageIcon = "diskette.png";
                        table.ActionButtonEnabled = true;
                        break;
                    case TableOrderStatus.Paid:
                        table.BorderColour = "Green";
                        break;
                }
            }
        }

        /// <summary>
        /// When a table is selected, show menu popup
        /// </summary>
        /// <param name="tableModel">TableModel for selected table</param>
        /// <returns>Returns a Task Object</returns>
        [RelayCommand]
        private async Task TableSelected(TableModel tableModel)
        {
            var helpPopup = new MainPagePopup(_homeViewModel, tableModel);
            await Shell.Current.ShowPopupAsync(helpPopup);
        }
    }
}
