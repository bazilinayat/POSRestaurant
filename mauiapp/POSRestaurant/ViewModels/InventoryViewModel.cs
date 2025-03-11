using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using POSRestaurant.ChangedMessages;
using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using POSRestaurant.Pages;
using POSRestaurant.Service.LoggerService;
using System.Collections.ObjectModel;

namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// ViewModel for Item Report operations
    /// </summary>
    public partial class InventoryViewModel : ObservableObject, IRecipient<StaffChangedMessage>, IRecipient<ExpenseTypeChangedMessage>
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
        /// ServiceProvider for the DIs
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

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
        /// Rows which we will be able to edit
        /// </summary>
        public ObservableCollection<InventoryRowModel> Rows { get; set; } = new ObservableCollection<InventoryRowModel>();

        /// <summary>
        /// Type of expense items, we need to choose one
        /// </summary>
        public ObservableCollection<ExpenseTypeModel> ExpenseItemTypes { get; set; } = new();

        /// <summary>
        /// List of expense items to choose from
        /// </summary>
        public ObservableCollection<ExpenseItem> ExpenseItems { get; set; } = new();

        /// <summary>
        /// List of staff members, co-owners
        /// </summary>
        public ObservableCollection<Staff> StaffMembers { get; set; } = new();

        /// <summary>
        /// Type of payment mode, we need to choose one
        /// </summary>
        public ObservableCollection<ValueForPicker> PaymentModes { get; set; } = new();

        /// <summary>
        /// Constructor for the InventoryViewModel
        /// </summary>
        /// <param name="databaseService">DI for DatabaseService</param>
        /// <param name="databaseService">DI for DatabaseService</param>
        public InventoryViewModel(IServiceProvider serviceProvider, LogService logger, DatabaseService databaseService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _databaseService = databaseService;

            WeakReferenceMessenger.Default.Register<StaffChangedMessage>(this);
            WeakReferenceMessenger.Default.Register<ExpenseTypeChangedMessage>(this);
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
                _isInitialized = true;
                IsLoading = true;

                ExpenseItems.Clear();
                StaffMembers.Clear();
                PaymentModes.Clear();

                var expenseItems = (await _databaseService.SettingsOperation.GetExpenseTypes()).ToList()
                                    .Select(ExpenseTypeModel.FromEntity)
                                    .ToList();
                foreach (var coowner in expenseItems)
                {
                    ExpenseItemTypes.Add(coowner);
                }

                if (ExpenseItemTypes.Count() == 0)
                {
                    await Shell.Current.DisplayAlert(
                        "Error",
                        $"Add Expense Types in Settings",
                        "OK");
                    var settingVM = _serviceProvider.GetRequiredService<SettingsViewModel>();
                    await Application.Current.MainPage.Navigation.PushAsync(new SettingsPage(settingVM));
                }
                else
                {
                    ExpenseItemTypes[0].IsSelected = true;
                }

                // Populate StaffMembers (mock data for now)
                var coowners = await _databaseService.StaffOperaiotns.GetStaffBasedOnRole(StaffRole.CoOwner);
                foreach (var coowner in coowners)
                {
                    StaffMembers.Add(coowner);
                }

                // Populate ExpenseItemTypes
                foreach (ValueForPicker desc in EnumExtensions.GetAllDescriptions<ExpensePaymentModes>())
                {
                    PaymentModes.Add(desc);
                }

                Rows.Clear();
                InitializeRows(10);

                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("InventoryVM-InitializeAsync Error", ex);
            }
        }

        /// <summary>
        /// To initialize 10 rows on the UI
        /// Which can be edited to put data in inventory
        /// </summary>
        /// <param name="count">Count of the rows we need to add</param>
        private void InitializeRows(int count)
        {
            for (int i = 0; i < count; i++)
            {
                Rows.Add(new InventoryRowModel(_databaseService)
                {
                    ExpenseItemTypes = ExpenseItemTypes,
                    ExpenseItem = "",
                    StaffMembers = StaffMembers,
                    PaymentModes = PaymentModes
                });
            }
        }

        /// <summary>
        /// Refresh staff details when received
        /// </summary>
        /// <param name="message">StaffChangedMessage</param>
        public async void Receive(StaffChangedMessage message)
        {
            try
            {
                StaffMembers.Clear();
                // Populate StaffMembers (mock data for now)
                var coowners = await _databaseService.StaffOperaiotns.GetStaffBasedOnRole(StaffRole.CoOwner);
                foreach (var coowner in coowners)
                {
                    StaffMembers.Add(coowner);
                }

                for (int i = 0; i < Rows.Count; i++)
                {
                    Rows[i].StaffMembers = StaffMembers;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("InventoryVM-Receive StaffChangedMessage Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error while Changing Staff Details", "OK");
            }
        }

        /// <summary>
        /// Refresh expense type details when received
        /// </summary>
        /// <param name="message">StaffChangedMessage</param>
        public async void Receive(ExpenseTypeChangedMessage message)
        {
            try
            {
                ExpenseItemTypes.Clear();
                var expenseItems = (await _databaseService.SettingsOperation.GetExpenseTypes()).ToList()
                                    .Select(ExpenseTypeModel.FromEntity)
                                    .ToList();
                foreach (var coowner in expenseItems)
                {
                    ExpenseItemTypes.Add(coowner);
                }
                ExpenseItemTypes[0].IsSelected = true;

                for (int i = 0; i < Rows.Count; i++)
                {
                    Rows[i].ExpenseItemTypes = ExpenseItemTypes;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("InventoryVM-Receive ExpenseTypeChangedMessage Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Adding new Rows", "OK");
            }
        }

        /// <summary>
        /// To handle the add rows command from the UI
        /// </summary>
        [RelayCommand]
        private void AddRows()
        {
            InitializeRows(10);
        }

        // Add this method to InventoryViewModel.cs
        private bool IsRowValid(InventoryRowModel row)
        {
            return row.SelectedExpenseItemType != null
                && !string.IsNullOrWhiteSpace(row.ExpenseItem)
                && row.WeightOrQuantity > 0
                && row.AmountPaid > 0
                && row.SelectedPaymentMode != null
                && row.SelectedPayer != null
                && !row.IsSaved; // Only validate unsaved rows
        }

        [RelayCommand]
        private async Task SaveAll()
        {
            try
            {
                var validRows = Rows.Where(IsRowValid).ToList();

                if (!validRows.Any())
                {
                    await Shell.Current.DisplayAlert(
                        "Save Failed",
                        "No valid rows found to save. Please ensure all required fields are filled.",
                        "OK");
                    return;
                }

                foreach (var row in validRows)
                {
                    await row.Save();
                }

                await Shell.Current.DisplayAlert(
                    "Success",
                    $"Successfully saved {validRows.Count} rows.",
                    "OK");

                await InitializeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("InventoryVM-SendAll Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Saving Inventory Data", "OK");
            }
        }
    }
}
