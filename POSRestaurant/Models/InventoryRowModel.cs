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

        DatabaseService _databaseService;
        public ObservableCollection<ValueForPicker> ExpenseItemTypes { get; set; } = new();
        public ObservableCollection<ExpenseItem> ExpenseItems { get; set; } = new();
        public ObservableCollection<Staff> StaffMembers { get; set; } = new();

        private ValueForPicker _selectedExpenseItemType;
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

        private ExpenseItem _selectedExpenseItem;
        public ExpenseItem SelectedExpenseItem
        {
            get => _selectedExpenseItem;
            set { _selectedExpenseItem = value; OnPropertyChanged(); }
        }

        private double _weightOrQuantity;
        public double WeightOrQuantity
        {
            get => _weightOrQuantity;
            set { _weightOrQuantity = value; OnPropertyChanged(); }
        }

        private double _amountPaid;
        public double AmountPaid
        {
            get => _amountPaid;
            set { _amountPaid = value; OnPropertyChanged(); }
        }

        private Staff _selectedPayer;
        public Staff SelectedPayer
        {
            get => _selectedPayer;
            set { _selectedPayer = value; OnPropertyChanged(); }
        }

        public InventoryRowModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

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
                TotalPrice = AmountPaid
            };

            var error = await _databaseService.InventoryOperations.SaveInventoryEntryAsync(inventory);

            if (error != null)
            {
                await Shell.Current.DisplayAlert("Inventory Entry", error, "OK");
            }

            // Logic to save the row
            IsSaved = true;
        }

        private bool _isSaved;
        public bool IsSaved
        {
            get => _isSaved;
            set { _isSaved = value; OnPropertyChanged(); }
        }

        private void UpdateExpenseItems(int expenseType)
        {
            // Logic to update ExpenseItems based on SelectedExpenseItemType
            PopulateItems(expenseType);
        }

        private async Task PopulateItems(int expenseType)
        {
            var items = await _databaseService.InventoryOperations.GetExpenseItemBasedOnTypeAsync((ExpenseItemTypes)expenseType);

            ExpenseItems.Clear();

            foreach(var item in items)
            {
                ExpenseItems.Add(item);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
