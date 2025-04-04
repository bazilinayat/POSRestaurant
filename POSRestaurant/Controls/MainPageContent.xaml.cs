using CommunityToolkit.Maui.Views;
using POSRestaurant.Models;
using POSRestaurant.ViewModels;

namespace POSRestaurant.Controls;

/// <summary>
/// Class for Main Content
/// </summary>
public partial class MainPageContent : ContentView
{
    /// <summary>
    /// DIed property to handle the HomeViewModel
    /// </summary>
    public HomeViewModel _homeViewModel;

    /// <summary>
    /// DIed TableModel for table info and update
    /// </summary>
    private readonly TableModel _tableModel;

    /// <summary>
    /// To interact with the popup where this content is showed
    /// </summary>
    private readonly Popup _popup;

    /// <summary>
    /// Contructor for the MainPage for binding and init
    /// </summary>
    /// <param name="popup">Popup where this content is showed</param>
    /// <param name="homeViewModel">HomeViewModel for the content page and handle actions</param>
    /// <param name="tableModel">TableViewModel to add table details with orders</param>
    public MainPageContent(Popup popup, HomeViewModel homeViewModel, TableModel tableModel)
	{
        InitializeComponent();
        _homeViewModel = homeViewModel;
        BindingContext = _homeViewModel;

        _popup = popup;

        _tableModel = tableModel;

        Initialize();
    }

    /// <summary>
    /// To load the initial data and add any other logic
    /// </summary>
    private async void Initialize()
    {
        //await _homeViewModel.InitializeAsync();
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

    /// <summary>
    /// When place order button is clicked
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">EventArgs</param>
    private void PlaceOrder_Clicked(object sender, EventArgs e)
    {
        _homeViewModel.PlaceOrderCommand.Execute(_tableModel);
    }

    /// <summary>
    /// When cancel button is clicked
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">EventArgs</param>
    private void CancelButton_Clicked(object sender, EventArgs e)
    {
        _popup.Close();
    }
}