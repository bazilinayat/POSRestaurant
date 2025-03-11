using Microsoft.Maui.Controls;
using POSRestaurant.ViewModels;

namespace POSRestaurant.Pages;

/// <summary>
/// ManageMenuItem Page of Application
/// </summary>
public partial class ManageMenuItemPage : ContentPage
{
    /// <summary>
    /// DIed property to handle the HomeViewModel
    /// </summary>
    private readonly ManageMenuItemViewModel _manageMenuViewModel;

    /// <summary>
    /// Initialize ManageMenuItemPage
    /// </summary>
    public ManageMenuItemPage(ManageMenuItemViewModel manageMenuViewModel)
	{
		InitializeComponent();
        _manageMenuViewModel = manageMenuViewModel;
        BindingContext = _manageMenuViewModel;
        Initialize();
    }

    /// <summary>
    /// To load the initial data and add any other logic
    /// </summary>
    private async void Initialize()
    {
        await _manageMenuViewModel.InitializeAsync();
    }

    /// <summary>
    /// Event to call the control for category listing
    /// </summary>
    /// <param name="menuCategory">Sender Object</param>
    
    private async void CategoriesListControl_OnCategorySelected(Models.MenuCategoryModel menuCategory)
    {
        await _manageMenuViewModel.SelectCategoryCommand.ExecuteAsync(menuCategory.Id);
    }

    /// <summary>
    /// Event to call the control for items listing
    /// </summary>
    /// <param name="menuItem">Sener Object</param>
    /// <returns>Returns a Task Object</returns>
    private async void MenuItemsListControl_OnMenuItemSelected(Data.ItemOnMenu menuItem)
    {
        await _manageMenuViewModel.EditMenuItemCommand.ExecuteAsync(menuItem);
    }

    /// <summary>
    /// To handle the cancel button click on the the control
    /// </summary>
    private void SaveMenuItemFormControl_OnCancel()
    {
        _manageMenuViewModel.CancelCommand.Execute(null);
    }

    /// <summary>
    /// To handle the save button click on the control
    /// </summary>
    /// <param name="menuItem">MenuItem to be saved or updated</param>
    private async void SaveMenuItemFormControl_OnSaveItem(Models.ItemOnMenuModel menuItem)
    {
        await _manageMenuViewModel.SaveItemCommand.ExecuteAsync(menuItem);
    }

    /// <summary>
    /// To handle the delete button click on the control
    /// </summary>
    /// <param name="menuItem">MenuItem to be deleted</param>
    private async void SaveMenuItemFormControl_OnDeleteItem(Models.ItemOnMenuModel menuItem)
    {
        await _manageMenuViewModel.DeleteItemCommand.ExecuteAsync(menuItem);
    }
}