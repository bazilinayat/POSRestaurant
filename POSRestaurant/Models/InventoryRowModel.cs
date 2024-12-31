using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POSRestaurant.Data;
using POSRestaurant.DBO;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace POSRestaurant.Models
{
    /// <summary>
    /// Model for inventory row, to edit and save in inventory table
    /// </summary>
    public partial class InventoryRowModel : INotifyPropertyChanged
    {
        /// <summary>
        /// To pass and use for items and saving the data
        /// </summary>
        DatabaseService _databaseService;

        /// <summary>
        /// Types of expense item, to category
        /// </summary>
        public ObservableCollection<ValueForPicker> ExpenseItemTypes { get; set; } = new();

        /// <summary>
        /// List of expense items to choose from, will be given values when type is selected
        /// </summary>
        public ObservableCollection<ExpenseItem> ExpenseItems { get; set; } = new();

        /// <summary>
        /// List of staff members, co-owner
        /// </summary>
        public ObservableCollection<Staff> StaffMembers { get; set; } = new();

        /// <summary>
        /// Expense Iten Type, which is selected from Picker
        /// </summary>

        private ValueForPicker _selectedExpenseItemType;

        /// <summary>
        /// To load the items when type is selected
        /// </summary>
        public ValueForPicker SelectedExpenseItemType
        {
            get => _selectedExpenseItemType;
            set
            {
                _selectedExpenseItemType = value;
                OnPropertyChanged();
                UpdateExpenseItems(value.Key);
            }
        }

        /// <summary>
        /// To handle change of selection of expense item from Picker
        /// </summary>
        private ExpenseItem _selectedExpenseItem;
        public ExpenseItem SelectedExpenseItem
        {
            get => _selectedExpenseItem;
            set { _selectedExpenseItem = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// To handle the weight or quantity entry
        /// </summary>
        private double _weightOrQuantity;
        public double WeightOrQuantity
        {
            get => _weightOrQuantity;
            set { _weightOrQuantity = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// To handle the amount paid entry
        /// </summary>
        private double _amountPaid;
        public double AmountPaid
        {
            get => _amountPaid;
            set { _amountPaid = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// To handle change of selection of Payer from Picker
        /// </summary>
        private Staff _selectedPayer;
        public Staff SelectedPayer
        {
            get => _selectedPayer;
            set { _selectedPayer = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool _isSaved;
        public bool IsSaved
        {
            get => _isSaved;
            set { _isSaved = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Constructor of the RowModel
        /// </summary>
        /// <param name="databaseService">Object of Database to use for items and save</param>
        public InventoryRowModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// To handle the save command for each row
        /// </summary>
        /// <returns>Returns a Task object</returns>
        [RelayCommand]
        private async Task Save()
        {
            Inventory inventory = new Inventory
            {
                EntryDate = DateTime.Now,
                ExpenseItemId = SelectedExpenseItem.Id,
                IsWeighted = true,
                ItemType = (ExpenseItemTypes)SelectedExpenseItemType.Key,
                QuantityOrWeight = WeightOrQuantity,
                StaffId = SelectedPayer.Id,
                TotalPrice = AmountPaid,
                ExpenseItemName = SelectedExpenseItem.Name,
                StaffName = SelectedPayer.Name,
            };

            var error = await _databaseService.InventoryOperations.SaveInventoryEntryAsync(inventory);

            if (error != null)
            {
                await Shell.Current.DisplayAlert("Inventory Entry", error, "OK");
            }

            // Logic to save the row
            IsSaved = true;
        }

        /// <summary>
        /// To update the list of expense items
        /// </summary>
        /// <param name="expenseType">Int, expense type</param>
        private void UpdateExpenseItems(int expenseType)
        {
            PopulateItems(expenseType);
        }

        /// <summary>
        /// Database call and load items
        /// </summary>
        /// <param name="expenseType">Int, expense type</param>
        /// <returns>Returns a Task object</returns>
        private async Task PopulateItems(int expenseType)
        {
            var items = await _databaseService.InventoryOperations.GetExpenseItemBasedOnTypeAsync((ExpenseItemTypes)expenseType);

            ExpenseItems.Clear();

            foreach(var item in items)
            {
                ExpenseItems.Add(item);
            }
        }

        /// <summary>
        /// To handle the property changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
