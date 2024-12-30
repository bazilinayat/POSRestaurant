using CommunityToolkit.Mvvm.ComponentModel;
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

        public ObservableCollection<InventoryRowModel> Rows { get; set; } = new ObservableCollection<InventoryRowModel>();
        public ObservableCollection<ValueForPicker> ExpenseItemTypes { get; set; } = new();
        public ObservableCollection<ExpenseItem> ExpenseItems { get; set; } = new();
        public ObservableCollection<Staff> StaffMembers { get; set; } = new();
        public ICommand AddRowsCommand { get; }

        /// <summary>
        /// Constructor for the InventoryViewModel
        /// </summary>
        /// <param name="databaseService">DI for DatabaseService</param>
        public InventoryViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            InitializeAsync();
            InitializeRows(10);
            AddRowsCommand = new Command(AddRows);
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

            IsLoading = false;
        }

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

        private void AddRows()
        {
            InitializeRows(10);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        
    }
}
