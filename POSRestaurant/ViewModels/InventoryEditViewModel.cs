using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using POSRestaurant.Service.LoggerService;
using System.Collections.ObjectModel;

namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// ViewModel for Inventory Edit operations
    /// </summary>
    public partial class InventoryEditViewModel : ObservableObject
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
        /// To check if ViewModel is already initialized
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// Selected date from the Order page
        /// </summary>
        [ObservableProperty]
        private DateTime _selectedDate = DateTime.Now;

        /// <summary>
        /// ObservableCollection for KotItems
        /// </summary>
        public ObservableCollection<InventoryReportModel> InventoryReportData { get; set; } = new();

        /// <summary>
        /// Constructor for the InventoryEditViewModel
        /// </summary>
        /// <param name="databaseService">DI for DatabaseService</param>
        public InventoryEditViewModel(LogService logger, DatabaseService databaseService)
        {
            _logger = logger;
            _databaseService = databaseService;
        }

        /// <summary>
        /// Initialize the ViewModel
        /// Fetch data and assign
        /// </summary>
        /// <returns>Returns a Task object</returns>
        public async ValueTask InitializeAsync()
        {
            try
            {
                //Reset page
                if (_isInitialized)
                {
                    SelectedDate = DateTime.Now;

                    InventoryReportData.Clear();

                    return;
                }

                _isInitialized = true;
                IsLoading = true;

                await MakeInventoryReport();

                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("InventoryEditVM-InitializeAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Inventory Report Screen", "OK");
            }
        }

        /// <summary>
        /// To get the data for inventory report from the database
        /// </summary>
        /// <returns></returns>
        private async ValueTask MakeInventoryReport()
        {
            try
            {
                InventoryReportData.Clear();
                var inventoryEntries = await _databaseService.InventoryOperations.GetInventoryItemsAsync(SelectedDate);

                if (inventoryEntries.Length > 0)
                {
                    var inventoryItems = inventoryEntries.Select(InventoryReportModel.FromEntity)
                                        .ToList();

                    foreach (var inventory in inventoryItems)
                    {
                        InventoryReportData.Add(inventory);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("InventoryEditVM-MakeInventoryReport Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Generating Inventory Report", "OK");
            }
        }

        /// <summary>
        /// To search the order with given paramters
        /// </summary>
        /// <returns>Returns a task</returns>
        [RelayCommand]
        private async Task Search()
        {
            await MakeInventoryReport();
        }

        /// <summary>
        /// To delete the inventory entry made
        /// </summary>
        /// <param name="inventoryReportModel">The inventory entry to delete</param>
        /// <returns>Returns a Task</returns>
        [RelayCommand]
        private async Task DeleteInventoryEntry(InventoryReportModel inventoryReportModel)
        {
            try
            {
                var inventoryToDelete = Inventory.FromEntity(inventoryReportModel);

                await _databaseService.InventoryOperations.DeleteInventoryAsync(inventoryToDelete);
            }
            catch (Exception ex)
            {
                _logger.LogError("InventoryEditVM-DeleteInventoryEntry Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Generating Inventory Report", "OK");
            }
        }
    }
}
