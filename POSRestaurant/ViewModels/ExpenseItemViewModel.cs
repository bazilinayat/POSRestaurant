using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// ViewModel to be used with ExpenseItemManagementPage
    /// </summary>
    public partial class ExpenseItemViewModel : ObservableObject
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
        /// List of expense types for the drop down
        /// </summary>
        public List<ExpenseTypeModel> ExpenseTypes { get; set; }

        /// <summary>
        /// ObservableCollection for orders
        /// </summary>
        public ObservableCollection<ExpenseItemModel> ExpenseItems { get; set; } = new();

        /// <summary>
        /// Property to observe the selected staff on UI
        /// </summary>
        [ObservableProperty]
        private ExpenseItemEditModel _expenseItem = new();

        [ObservableProperty]
        private bool isWeighted;

        [ObservableProperty]
        private bool isQuantity = true;

        /// <summary>
        /// Constructor for the ExpenseItemViewModel
        /// </summary>
        /// <param name="databaseService">DI for DatabaseService</param>
        public ExpenseItemViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            var expenseTypes = Enum.GetValues(typeof(ExpenseItemTypes))
               .Cast<ExpenseItemTypes>()
               .Select(t => new ExpenseTypeModel { Id = (int)t, ExpenseTypeName = t.ToString(), IsSelected = false }).ToList();

            ExpenseTypes = new List<ExpenseTypeModel>();
            foreach(var expenseType in expenseTypes)
            {
                if (expenseType.Id == 0) continue;
                ExpenseTypes.Add(expenseType);
            }
        }

        /// <summary>
        /// Initialize the ViewModel
        /// Fetch data and assign
        /// </summary>
        /// <returns>Returns a Task object</returns>
        public async ValueTask InitializeAsync()
        {
            IsLoading = true;

            ExpenseItems.Clear();
            var allExpenseItems = await _databaseService.InventoryOperations.GetAllExpenseItemsAsync();
            var expenseItems = allExpenseItems.Select(o => new ExpenseItemModel
            {
                Id = o.Id,
                Name = o.Name,
                IsWeighted = o.IsWeighted,
                ItemType = o.ItemType,
                IsSelected = false
            });

            foreach (var expenseItem in expenseItems)
            {
                ExpenseItems.Add(expenseItem);
            }
            foreach (var expenseType in ExpenseTypes)
            {
                expenseType.IsSelected = false;
            }

            IsLoading = false;
        }

        /// <summary>
        /// Command to call when the Expense Item is selected
        /// </summary>
        /// <param name="expenseItemModel">Selected Expense Item</param>
        [RelayCommand]
        private async Task SelectExpenseItemAsync(ExpenseItemModel expenseItemModel)
        {
            var prevSelectedOrder = ExpenseItems.FirstOrDefault(o => o.IsSelected);
            if (prevSelectedOrder != null)
            {
                prevSelectedOrder.IsSelected = false;
                if (prevSelectedOrder.Id == expenseItemModel.Id)
                {
                    Cancel();
                    return;
                }
            }

            expenseItemModel.IsSelected = true;

            foreach (var expenseType in ExpenseTypes)
            {
                if (expenseType.Id == (int)expenseItemModel.ItemType)
                    expenseType.IsSelected = true;
                else
                    expenseType.IsSelected = false;
            }

            IsWeighted = expenseItemModel.IsWeighted;
            IsQuantity = !expenseItemModel.IsWeighted;

            ExpenseItem = new ExpenseItemEditModel
            {
                Id = expenseItemModel.Id,
                Name = expenseItemModel.Name,
                ItemType = expenseItemModel.ItemType,
                IsWeighted = expenseItemModel.IsWeighted,
            };
        }

        /// <summary>
        /// To save or update the expense item sent by the control
        /// </summary>
        /// <param name="expenseItem">Expense item to save or update</param>
        /// <returns>Returns a Task object</returns>
        [RelayCommand]
        private async Task SaveExpenseItemAsync(ExpenseItemEditModel expenseItem)
        {
            IsLoading = true;

            var expenseItemModel = new ExpenseItemModel
            {
                Id = expenseItem.Id,
                Name = expenseItem.Name,
                ItemType = expenseItem.ItemType,
                IsWeighted = expenseItem.IsWeighted,
            };

            var errorMessage = await _databaseService.InventoryOperations.SaveStaffAsync(expenseItemModel);

            if (errorMessage != null)
            {
                await Shell.Current.DisplayAlert("Error", errorMessage, "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Successful", "Expense item saved successfully", "OK");

                await InitializeAsync();

                // Push for change in staff info
                // WeakReferenceMessenger.Default.Send(StaffChangedMessage.From(expenseItemModel));

                Cancel();
            }

            IsLoading = false;
        }

        /// <summary>
        /// To delete the expense item sent by the control
        /// </summary>
        /// <param name="expenseItem">ExpenseItem to delete</param>
        /// <returns>Returns a Task object</returns>
        [RelayCommand]
        private async Task DeleteItemAsync(ExpenseItemEditModel expenseItem)
        {
            var expenseItemModel = new ExpenseItemModel
            {
                Id = expenseItem.Id,
                Name = expenseItem.Name,
                IsWeighted = expenseItem.IsWeighted,
                ItemType = expenseItem.ItemType,
            };

            if (await _databaseService.InventoryOperations.DeleteStaffAsync(expenseItemModel) > 0)
            {
                await Shell.Current.DisplayAlert("Successful", $"{expenseItem.Name} deleted successfully", "OK");

                await InitializeAsync();

                Cancel();
            }
        }

        /// <summary>
        /// To handle cancel button click on the control
        /// </summary>
        [RelayCommand]
        private void Cancel()
        {
            ExpenseItem = new();
            foreach (var expenseType in ExpenseTypes)
            {
                expenseType.IsSelected = false;
            }

            var prevSelectedOrder = ExpenseItems.FirstOrDefault(o => o.IsSelected);
            if (prevSelectedOrder != null)
            {
                prevSelectedOrder.IsSelected = false;
            }
        }

        /// <summary>
        /// To start with adding new expense item
        /// </summary>
        [RelayCommand]
        private void AddNewExpenseItem()
        {
            ExpenseItem = new ExpenseItemEditModel
            {
                Id = 0
            };
            foreach (var expenseType in ExpenseTypes)
            {
                expenseType.IsSelected = false;
            }
        }
    }
}
