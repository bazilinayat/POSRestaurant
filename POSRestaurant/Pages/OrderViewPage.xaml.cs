using POSRestaurant.Models;
using POSRestaurant.ViewModels;

namespace POSRestaurant.Pages;

/// <summary>
/// Page where order can be viewed with kots
/// </summary>
public partial class OrderViewPage : ContentPage
{
    /// <summary>
    /// DIed property to handle the HomeViewModel
    /// </summary>
    private readonly OrderViewViewModel _viewOrderViewModel;

    /// <summary>
    /// DIed TableModel for table info and update
    /// </summary>
    private readonly TableModel _tableModel;

    /// <summary>
    /// DIed TableModel for table info and update
    /// </summary>
    private readonly OrdersViewModel _orderViewModel;

    /// <summary>
    /// Initialize OrderViewPage
    /// </summary>
    /// <param name="viewOrderViewModel">ViewOrderViewModel for the content page and handle actions</param>
    /// <param name="orderViewModel">OrderViewModel for order related queries</param>
    /// <param name="tableModel">TableViewModel to add table details with orders</param>
    public OrderViewPage(OrderViewViewModel viewOrderViewModel, OrdersViewModel orderViewModel, TableModel tableModel)
	{
        InitializeComponent();
        _viewOrderViewModel = viewOrderViewModel;

        _viewOrderViewModel.TableModel = tableModel;
        BindingContext = _viewOrderViewModel;

        _tableModel = tableModel;
        _orderViewModel = orderViewModel;

        Initialize();
    }

    /// <summary>
    /// To load the initial data and add any other logic
    /// </summary>
    private async void Initialize()
    {
        await _viewOrderViewModel.InitializeAsync();
    }

    /// <summary>
    /// Event called when category is selected
    /// </summary>
    /// <param name="category">Selected Category</param>
    private async void CategoriesListControl_OnCategorySelected(Models.MenuCategoryModel category)
    {
        await _viewOrderViewModel.SelectCategoryCommand.ExecuteAsync(category.Id);
    }

    /// <summary>
    /// Event called when menu item is selected
    /// </summary>
    /// <param name="category">Selected MenuItem</param>
    private void MenuItemsListControl_OnMenuItemSelected(Data.ItemOnMenu menuItem)
    {
        _viewOrderViewModel.AddToCartCommand.Execute(menuItem);
    }

    /// <summary>
    /// Event called when SearchBox text changes, this is used to search for items
    /// </summary>
    /// <param name="sender">SearchBox as sender</param>
    /// <param name="e">EventArgs</param>
    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        _viewOrderViewModel.SearchItemsCommand.Execute(e.NewTextValue);
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