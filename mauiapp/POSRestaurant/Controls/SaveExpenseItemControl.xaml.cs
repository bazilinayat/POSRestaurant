using CommunityToolkit.Mvvm.Input;
using POSRestaurant.Data;
using POSRestaurant.Models;
using System.Data;

namespace POSRestaurant.Controls;

/// <summary>
/// Control for editing the expense item entries
/// </summary>
public partial class SaveExpenseItemControl : ContentView
{
    /// <summary>
    /// constructor for the editing control
    /// </summary>
    public SaveExpenseItemControl()
	{
		InitializeComponent();
	}

    /// <summary>
    /// BindableProperty for Item to be used on UI
    /// </summary>
    public static readonly BindableProperty ExpenseItemToSaveProperty =
        BindableProperty.Create(nameof(ExpenseItemToSave), typeof(ExpenseItemEditModel), typeof(SaveExpenseItemControl), new ExpenseItemEditModel());

    /// <summary>
    /// For the specific item this control is responsible for
    /// </summary>
    public ExpenseItemEditModel ExpenseItemToSave
    {
        get => (ExpenseItemEditModel)GetValue(ExpenseItemToSaveProperty);
        set
        {
            SetValue(ExpenseItemToSaveProperty, value);
            if (value.IsWeighted)
                IsWeighted = true;
            else
                IsQuantity = true;
        }
    }

    /// <summary>
    /// BindableProperty for Item to be used on UI
    /// </summary>
    public static readonly BindableProperty ExpenseItemTypesProperty =
        BindableProperty.Create(nameof(ExpenseItemTypes), typeof(List<ExpenseTypeModel>), typeof(SaveExpenseItemControl), new List<ExpenseTypeModel>());

    /// <summary>
    /// For the specific item this control is responsible for
    /// </summary>
    public List<ExpenseTypeModel> ExpenseItemTypes
    {
        get => (List<ExpenseTypeModel>)GetValue(ExpenseItemTypesProperty);
        set => SetValue(ExpenseItemTypesProperty, value);
    }

    /// <summary>
    /// For property for is weighted
    /// </summary>
    public static readonly BindableProperty IsWeightedProperty = BindableProperty.Create(
        nameof(IsWeighted),
        typeof(bool),
        typeof(SaveExpenseItemControl),
        false,
        BindingMode.TwoWay);

    /// <summary>
    /// For property of is quantity
    /// </summary>
    public static readonly BindableProperty IsQuantityProperty = BindableProperty.Create(
        nameof(IsQuantity),
        typeof(bool),
        typeof(SaveExpenseItemControl),
        true,
        BindingMode.TwoWay);

    /// <summary>
    /// For property of is weighted
    /// </summary>
    public bool IsWeighted
    {
        get => (bool)GetValue(IsWeightedProperty);
        set => SetValue(IsWeightedProperty, value);
    }

    /// <summary>
    /// For property of is quantity
    /// </summary>
    public bool IsQuantity
    {
        get => (bool)GetValue(IsQuantityProperty);
        set => SetValue(IsQuantityProperty, value);
    }

    /// <summary>
    /// To change the header
    /// In case it is to be added or updated
    /// </summary>
    public string SaveExpenseItemHeader
    {
        get => CanBeDeleted == 0 ? "Add Expense Item Details" : "Update Expense Item Details";
    }

    /// <summary>
    /// BindableProperty for staff to be used on UI
    /// </summary>
    public static readonly BindableProperty CanBeDeletedProperty =
        BindableProperty.Create(nameof(CanBeDeleted), typeof(int), typeof(SaveExpenseItemControl), -1);

    /// <summary>
    /// A public property to pass when we use the control
    /// </summary>
    public int CanBeDeleted
    {
        get
        {
            var val = (int)GetValue(CanBeDeletedProperty);
            return val > 0 ? 1 : 0;
        }
        set
        {
            SetValue(CanBeDeletedProperty, value);
        }
    }

    /// <summary>
    /// Event to work with save button click on the control
    /// </summary>
    public event Action<ExpenseItemEditModel> OnSaveExpenseItem;

    /// <summary>
    /// Command called when Save button is clicked
    /// </summary>
    /// <returns>Returns a task object</returns>
    [RelayCommand]
    private async Task SaveMenuItem()
    {
        // Validation

        if (string.IsNullOrWhiteSpace(ExpenseItemToSave.Name))
        {
            await ErrorAlertAsync("Enter a name for the item");
            return;
        }

        if (!IsWeighted && !IsQuantity)
        {
            await ErrorAlertAsync("Select if item is weighted or not");
            return;
        }

        ExpenseItemToSave.IsWeighted = IsWeighted;

        OnSaveExpenseItem?.Invoke(ExpenseItemToSave);

        static async Task ErrorAlertAsync(string message) =>
            await Shell.Current.DisplayAlert("Validation Error", message, "OK");
    }

    /// <summary>
    /// Event to work with delete button click on the control
    /// </summary>
    public event Action<ExpenseItemEditModel> OnDeleteItem;

    /// <summary>
    /// Command called when delete button is clicked
    /// </summary>
    /// <returns>Returns a task object</returns>
    [RelayCommand]
    private async Task DeleteMenuItem()
    {
        if (await Shell.Current.DisplayAlert("Delete Item?", $"Do you really want to delete {ExpenseItemToSave.Name}?", "Yes", "No"))
            OnDeleteItem?.Invoke(ExpenseItemToSave);
    }

    /// <summary>
    /// Event to work with cancel button click on the control
    /// </summary>
    public event Action? OnCancel;

    /// <summary>
    /// Command to handle the cancel button click
    /// </summary>
    [RelayCommand]
    private void Cancel() => OnCancel?.Invoke();

    /// <summary>
    /// Command to toggle expense types
    /// </summary>
    /// <param name="expenseTypeModel">ExpenseTypeModel toggled</param>
    [RelayCommand]
    private void ToggleTypeSelection(ExpenseTypeModel expenseTypeModel)
    {
        if (expenseTypeModel.IsSelected)
            return;

        var prevSelectedType = ExpenseItemTypes.FirstOrDefault(o => o.IsSelected);
        if (prevSelectedType != null)
        {
            prevSelectedType.IsSelected = false;
        }

        expenseTypeModel.IsSelected = true;
        ExpenseItemToSave.ItemType = (ExpenseItemTypes)expenseTypeModel.Id;
    }
}