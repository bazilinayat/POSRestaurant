using POSRestaurant.ViewModels;

namespace POSRestaurant.Pages;

/// <summary>
/// InventoryReport of the Application
/// </summary>
public partial class InventoryReport : ContentPage
{
    /// <summary>
    /// DIed property to handle the InventoryReportViewModel
    /// </summary>
    private readonly InventoryReportViewModel _inventoryReportViewModel;

    /// <summary>
    /// Initialize ItemReport Page
    /// </summary>
    /// <param name="inventoryReportViewModel">DI for InventoryReportViewModel</param>
    public InventoryReport(InventoryReportViewModel inventoryReportViewModel)
	{
		InitializeComponent();
        _inventoryReportViewModel = inventoryReportViewModel;
        BindingContext = _inventoryReportViewModel;

        Initialize();
	}

    /// <summary>
    /// To load the initial data and add any other logic
    /// </summary>
    private async void Initialize()
    {
        await _inventoryReportViewModel.InitializeAsync();
    }
}