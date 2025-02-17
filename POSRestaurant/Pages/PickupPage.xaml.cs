using Microsoft.Maui.Controls;
using POSRestaurant.Data;
using POSRestaurant.Models;
using POSRestaurant.ViewModels;

namespace POSRestaurant.Pages;

/// <summary>
/// Page where the order will be placed
/// </summary>
public partial class PickupPage : ContentPage
{
    /// <summary>
    /// DIed property to handle the HomeViewModel
    /// </summary>
    private readonly PickupViewModel _pickupViewModel;


    /// <summary>
    /// Initialize MainPage
    /// </summary>
    /// <param name="homeViewModel">HomeViewModel for the content page and handle actions</param>
    public PickupPage(PickupViewModel pickupViewModel)
    {
        InitializeComponent();
        _pickupViewModel = pickupViewModel;
        BindingContext = _pickupViewModel;

        Initialize();
    }

    /// <summary>
    /// To load the initial data and add any other logic
    /// </summary>
    private async void Initialize()
    {
        await _pickupViewModel.InitializeAsync();
    }

    /// <summary>
    /// Event called when category is selected
    /// </summary>
    /// <param name="category">Selected Category</param>
    private async void CategoriesListControl_OnCategorySelected(Models.MenuCategoryModel category)
    {
        await _pickupViewModel.SelectCategoryCommand.ExecuteAsync(category.Id);
    }

    /// <summary>
    /// Event called when menu item is selected
    /// </summary>
    /// <param name="category">Selected MenuItem</param>
    private void MenuItemsListControl_OnMenuItemSelected(Data.ItemOnMenu menuItem)
    {
        _pickupViewModel.AddToCartCommand.Execute(menuItem);
    }

    /// <summary>
    /// When place order button is clicked
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">EventArgs</param>
    private void PlaceOrder_Clicked(object sender, EventArgs e)
    {
        _pickupViewModel.PlaceOrderCommand.Execute(null);
    }

    /// <summary>
    /// Event called when SearchBox text changes, this is used to search for items
    /// </summary>
    /// <param name="sender">SearchBox as sender</param>
    /// <param name="e">EventArgs</param>
    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        _pickupViewModel.SearchItemsCommand.Execute(e.NewTextValue);
    }

    /// <summary>
    /// When cancel button is clicked
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">EventArgs</param>
    private async void CancelButton_Clicked(object sender, EventArgs e)
    {
        await Application.Current.MainPage.Navigation.PopAsync();
    }
}
