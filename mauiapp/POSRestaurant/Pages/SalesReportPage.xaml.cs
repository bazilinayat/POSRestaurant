using POSRestaurant.ViewModels;

namespace POSRestaurant.Pages;

/// <summary>
/// SalesReport of the Application
/// </summary>
public partial class SalesReportPage : ContentPage
{
    /// <summary>
    /// DIed property to handle the SalesReportViewModel
    /// </summary>
    private readonly SalesReportViewModel _salesReportViewModel;

    public SalesReportPage(SalesReportViewModel salesReportViewModel)
    {
		InitializeComponent();
        _salesReportViewModel = salesReportViewModel;
        BindingContext = _salesReportViewModel;

        Initialize();
    }
    /// <summary>
    /// To load the initial data and add any other logic
    /// </summary>
    private async void Initialize()
    {
        await _salesReportViewModel.InitializeAsync();
    }

}