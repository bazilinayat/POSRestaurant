using POSRestaurant.Models;
using POSRestaurant.ViewModels;

namespace POSRestaurant.Pages;

/// <summary>
/// Page to display our bill details and send for printing
/// </summary>
public partial class BillPage : ContentPage
{
    /// <summary>
    /// DIed property to handle the HomeViewModel
    /// </summary>
    private readonly BillViewModel _billViewModel;

    /// <summary>
    /// Constructor for the billing page
    /// </summary>
    /// <param name="billViewModel">BillViewModel for bill related details</param>
    /// <param name="orderViewModel">OrderViewModel for order related queries</param>
    /// <param name="tableModel">TableViewModel to add table details with orders</param>
    public BillPage(BillViewModel billViewModel, TableModel tableModel)
	{
        InitializeComponent();
        _billViewModel = billViewModel;

        _billViewModel.TableModel = tableModel;
        BindingContext = _billViewModel;

        Initialize();

        //_billViewModel.RegisterForPrinting();
    }

    /// <summary>
    /// To load the initial data and add any other logic
    /// </summary>
    private async void Initialize()
    {
        await _billViewModel.InitializeAsync();
    }

    protected virtual void OnNavigatedFrom(Microsoft.Maui.Controls.NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        //_billViewModel.UnregisterForPrinting();
    }

}