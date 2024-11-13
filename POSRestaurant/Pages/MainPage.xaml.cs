using POSRestaurant.Data;
using POSRestaurant.ViewModels;

namespace POSRestaurant.Pages;

/// <summary>
/// MainPage of the Application
/// </summary>
public partial class MainPage : ContentPage
{
    /// <summary>
    /// DIed property to handle the HomeViewModel
    /// </summary>
    private readonly HomeViewModel _homeViewModel;

    /// <summary>
    /// Initialize MainPage
    /// </summary>
    /// <param name="homeViewModel">DI for HomeViewModel</param>
    public MainPage(HomeViewModel homeViewModel)
    {
        InitializeComponent();
        _homeViewModel = homeViewModel;
        BindingContext = _homeViewModel;
        Initialize();
    }

    /// <summary>
    /// To load the initial data and add any other logic
    /// </summary>
    private async void Initialize()
    {
        await _homeViewModel.InitializeAsync();
    }

    /// <summary>
    /// Event called when category is selected
    /// </summary>
    /// <param name="category">Selected Category</param>
    private async void CategoriesListControl_OnCategorySelected(Models.MenuCategoryModel category)
    {
        await _homeViewModel.SelectCategoryCommand.ExecuteAsync(category.Id);
    }

    /// <summary>
    /// Event called when menu item is selected
    /// </summary>
    /// <param name="category">Selected MenuItem</param>
    private void MenuItemsListControl_OnMenuItemSelected(Data.ItemOnMenu menuItem)
    {
        _homeViewModel.AddToCartCommand.Execute(menuItem);
    }

    /// <summary>
    /// Event called when SearchBox text changes, this is used to search for items
    /// </summary>
    /// <param name="sender">SearchBox as sender</param>
    /// <param name="e">EventArgs</param>
    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        _homeViewModel.SearchItemsCommand.Execute(e.NewTextValue);
    }
}
