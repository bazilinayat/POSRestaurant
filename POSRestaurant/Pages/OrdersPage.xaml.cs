using POSRestaurant.ViewModels;

namespace POSRestaurant.Pages;

/// <summary>
/// OrdersPage of the Application
/// </summary>
public partial class OrdersPage : ContentPage
{
    /// <summary>
    /// DIed property to handle the OrderViewModel
    /// </summary>
    private readonly OrdersViewModel _ordersViewModel;

    /// <summary>
    /// Initialize OrdersPage
    /// </summary>
    public OrdersPage(OrdersViewModel ordersViewModel)
	{
		InitializeComponent();
        _ordersViewModel = ordersViewModel;
        BindingContext = _ordersViewModel;
        Initialize();
    }

    /// <summary>
    /// To load the initial data and add any other logic
    /// </summary>
    private async void Initialize()
    {
        await _ordersViewModel.InitializeAsync();
    }
}