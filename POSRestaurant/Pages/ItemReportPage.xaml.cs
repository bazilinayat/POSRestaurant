using POSRestaurant.ViewModels;

namespace POSRestaurant.Pages;

/// <summary>
/// ItemReport of the Application
/// </summary>
public partial class ItemReportPage : ContentPage
{
    /// <summary>
    /// DIed property to handle the ItemReportViewModel
    /// </summary>
    private readonly ItemReportViewModel _itemReportViewModel;

    /// <summary>
    /// Initialize ItemReport Page
    /// </summary>
    /// <param name="itemReportViewModel">DI for ItemReportViewModel</param>
    public ItemReportPage(ItemReportViewModel itemReportViewModel)
    {
        InitializeComponent();
        _itemReportViewModel = itemReportViewModel;
        BindingContext = _itemReportViewModel;
        Initialize();
    }

    /// <summary>
    /// To load the initial data and add any other logic
    /// </summary>
    private async void Initialize()
    {
        await _itemReportViewModel.InitializeAsync();
    }
}