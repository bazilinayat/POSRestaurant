using POSRestaurant.ViewModels;

namespace POSRestaurant.Pages;

/// <summary>
/// Page to mange the expense item in the org
/// </summary>
public partial class ExpenseItemManagementPage : ContentPage
{
    /// <summary>
    /// View Model for this page
    /// To Be DIed
    /// </summary>
    private readonly ExpenseItemViewModel _expenseItemViewModel;

    /// <summary>
    /// Constructor to intialize things
    /// </summary>
    /// <param name="expenseItemViewModel">DIed ExpenseItemViewModel</param>
    public ExpenseItemManagementPage(ExpenseItemViewModel expenseItemViewModel)
	{
        InitializeComponent();
        _expenseItemViewModel = expenseItemViewModel;
        BindingContext = _expenseItemViewModel;

        Initialize();
    }

    /// <summary>
    /// To load the initial data and add any other logic
    /// </summary>
    private async void Initialize()
    {
        await _expenseItemViewModel.InitializeAsync();
    }

    /// <summary>
    /// Event called when we save a Expense Item
    /// </summary>
    /// <param name="expenseItemModel">ExpenseItem to save or update</param>
    private async void SaveExpenseItemControl_OnSaveExpenseItem(Models.ExpenseItemEditModel expenseItemModel)
    {
        await _expenseItemViewModel.SaveExpenseItemCommand.ExecuteAsync(expenseItemModel);
    }

    /// <summary>
    /// Event called when we delete a staff
    /// </summary>
    /// <param name="expenseItemModel">Staff to delete</param>
    private async void SaveExpenseItemControl_OnDeleteItem(Models.ExpenseItemEditModel expenseItemModel)
    {
        await _expenseItemViewModel.DeleteItemCommand.ExecuteAsync(expenseItemModel);
    }

    /// <summary>
    /// Event called when cancel button is clicked
    /// </summary>
    private void SaveExpenseItemControl_OnCancel()
    {
        _expenseItemViewModel.CancelCommand.Execute(null);
    }
}