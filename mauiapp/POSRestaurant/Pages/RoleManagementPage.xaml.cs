using POSRestaurant.ViewModels;

namespace POSRestaurant.Pages;

/// <summary>
/// Page to mange the user role in the org
/// </summary>
public partial class RoleManagementPage : ContentPage
{
	/// <summary>
	/// View Model for this page
	/// To Be DIed
	/// </summary>
	private readonly RoleManagementViewModel _roleViewModel;

    /// <summary>
    /// Constructor to intialize things
    /// </summary>
    /// <param name="roleViewModel">DIed RoleManagementViewModel</param>
    public RoleManagementPage(RoleManagementViewModel roleViewModel)
	{
		InitializeComponent();
		_roleViewModel = roleViewModel;
		BindingContext = _roleViewModel;

        Initialize();
    }

    /// <summary>
    /// To load the initial data and add any other logic
    /// </summary>
    private async void Initialize()
    {
        await _roleViewModel.InitializeAsync();
    }
}