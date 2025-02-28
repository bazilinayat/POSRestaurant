﻿using CommunityToolkit.Mvvm.ComponentModel;
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
    public partial class InventoryRowModel : ObservableObject, INotifyPropertyChanged
    {
        /// <summary>
        /// To pass and use for items and saving the data
        /// </summary>
        DatabaseService _databaseService;

        /// <summary>
        /// Types of expense item, to category
        /// </summary>
        public ObservableCollection<ExpenseTypeModel> ExpenseItemTypes { get; set; } = new();

        /// <summary>
        /// To entry the expense item which we are buying
        /// </summary>
        public string ExpenseItem { get; set; }

        /// <summary>
        /// List of staff members, co-owner
        /// </summary>
        public ObservableCollection<Staff> StaffMembers { get; set; } = new();

        /// <summary>
        /// List of payment modes for expense items to choose from
        /// </summary>
        public ObservableCollection<ValueForPicker> PaymentModes { get; set; } = new();

        /// <summary>
        /// Expense Iten Type, which is selected from Picker
        /// </summary>

        private ExpenseTypeModel _selectedExpenseItemType;

        /// <summary>
        /// To load the items when type is selected
        /// </summary>
        public ExpenseTypeModel SelectedExpenseItemType
        {
            get => _selectedExpenseItemType;
            set
            {
                _selectedExpenseItemType = value;
                OnPropertyChanged();
            }
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
        /// To handle the payment mode entry
        /// </summary>
        private ValueForPicker _selectedPaymentMode;
        public ValueForPicker SelectedPaymentMode
        {
            get => _selectedPaymentMode;
            set { _selectedPaymentMode = value; OnPropertyChanged(); }
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
        public async Task Save()
        {
            Inventory inventory = new Inventory
            {
                EntryDate = DateTime.Now,
                IsWeighted = true,
                ExpenseTypeId = SelectedExpenseItemType.Id,
                ExpenseTypeName = SelectedExpenseItemType.Name,
                QuantityOrWeight = WeightOrQuantity,
                StaffId = SelectedPayer.Id,
                TotalPrice = AmountPaid,
                ExpenseItemName = ExpenseItem,
                StaffName = SelectedPayer.Name,
                PaymentMode = (ExpensePaymentModes)SelectedPaymentMode.Key,
                PaymentModeName = SelectedPaymentMode.Value
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
        /// To handle the property changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
