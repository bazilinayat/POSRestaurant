using POSRestaurant.ViewModels;

namespace POSRestaurant.Pages;

/// <summary>
/// InventoryEdit page of the Application
/// </summary>
public partial class InventoryEdit : ContentPage
{
    /// <summary>
    /// DIed property to handle the InventoryEditViewModel
    /// </summary>
    private readonly InventoryEditViewModel _inventoryEditViewModel;

    /// <summary>
    /// Initialize ItemReport Page
    /// </summary>
    /// <param name="inventoryEditViewModel">DI for InventoryEditViewModel</param>
    public InventoryEdit(InventoryEditViewModel inventoryEditViewModel)
	{
		InitializeComponent();

        _inventoryEditViewModel = inventoryEditViewModel;
        BindingContext = _inventoryEditViewModel;

        Initialize();
    }

    /// <summary>
    /// To load the initial data and add any other logic
    /// </summary>
    private async void Initialize()
    {
        await _inventoryEditViewModel.InitializeAsync();
    }
}