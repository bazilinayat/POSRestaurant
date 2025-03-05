using POSRestaurant.ViewModels;

namespace POSRestaurant.Pages;

/// <summary>
/// Page for managing kots for running orders
/// </summary>
public partial class ManageKOTPage : ContentPage
{
    /// <summary>
    /// DIed ManageKOTViewModel
    /// </summary>
    private readonly ManageKOTViewModel _manageKOTViewModel;

    /// <summary>
    /// Initialize Manage KOT page
    /// </summary>
    /// <param name="manageKOTViewModel">DI ManageKOTViewModel</param>
    public ManageKOTPage(ManageKOTViewModel manageKOTViewModel)
	{
		InitializeComponent();
        _manageKOTViewModel = manageKOTViewModel;
        BindingContext = _manageKOTViewModel;

        Initialize();
    }

    /// <summary>
    /// To load the initial data and add any other logic
    /// </summary>
    private async void Initialize()
    {
        await _manageKOTViewModel.InitializeAsync();
    }
}