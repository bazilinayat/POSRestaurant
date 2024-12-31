using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// ViewModel for Item Report operations
    /// </summary>
    public partial class InventoryViewModel : ObservableObject
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
        /// Rows which we will be able to edit
        /// </summary>
        public ObservableCollection<InventoryRowModel> Rows { get; set; } = new ObservableCollection<InventoryRowModel>();

        /// <summary>
        /// Type of expense items, we need to choose one
        /// </summary>
        public ObservableCollection<ValueForPicker> ExpenseItemTypes { get; set; } = new();

        /// <summary>
        /// List of expense items to choose from
        /// </summary>
        public ObservableCollection<ExpenseItem> ExpenseItems { get; set; } = new();

        /// <summary>
        /// List of staff members, co-owners
        /// </summary>
        public ObservableCollection<Staff> StaffMembers { get; set; } = new();

        /// <summary>
        /// Constructor for the InventoryViewModel
        /// </summary>
        /// <param name="databaseService">DI for DatabaseService</param>
        public InventoryViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            InitializeAsync();
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

            // Populate ExpenseItemTypes
            foreach (ValueForPicker desc in EnumExtensions.GetAllDescriptions<ExpenseItemTypes>())
            {
                ExpenseItemTypes.Add(desc);
            }

            // Populate ExpenseItems (mock data for now)
            var expenseItems = await _databaseService.InventoryOperations.GetAllExpenseItemsAsync();
            foreach(var expenseItem in expenseItems)
            {
                ExpenseItems.Add(expenseItem);
            }

            // Populate StaffMembers (mock data for now)
            var coowners = await _databaseService.StaffOperaiotns.GetStaffBasedOnRole(StaffRole.CoOwner);
            foreach (var coowner in coowners)
            {
                StaffMembers.Add(coowner);
            }

            InitializeRows(10);

            IsLoading = false;
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
                    ExpenseItems = new ObservableCollection<ExpenseItem> { new ExpenseItem { Id = 0, Name = "Select Item" } },
                    StaffMembers = StaffMembers
                });
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
    }
}
