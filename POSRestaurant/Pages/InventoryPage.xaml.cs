using POSRestaurant.ViewModels;

namespace POSRestaurant.Pages;

/// <summary>
/// Inventory Page of the Application
/// </summary>
public partial class InventoryPage : ContentPage
{
    /// <summary>
    /// DIed property to handle the InventoryViewModel
    /// </summary>
    private readonly InventoryViewModel _inventoryViewModel;

    /// <summary>
    /// Initialize Inventory Page
    /// </summary>
    /// <param name="inventoryViewModel">DI for InventoryViewModel</param>
    public InventoryPage(InventoryViewModel inventoryViewModel)
    {
        InitializeComponent();
        _inventoryViewModel = inventoryViewModel;
        BindingContext = _inventoryViewModel;
        Initialize();
    }

    /// <summary>
    /// To load the initial data and add any other logic
    /// </summary>
    private async void Initialize()
    {
        await _inventoryViewModel.InitializeAsync();
    }
}