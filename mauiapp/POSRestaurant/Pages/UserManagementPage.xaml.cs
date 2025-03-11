using POSRestaurant.Models;
using POSRestaurant.ViewModels;

namespace POSRestaurant.Pages;

/// <summary>
/// Page to mange the user role in the org
/// </summary>
public partial class UserManagementPage : ContentPage
{
	/// <summary>
	/// View Model for this page
	/// To Be DIed
	/// </summary>
	private readonly UserManagementViewModel _userViewModel;

    /// <summary>
    /// Constructor to intialize things
    /// </summary>
    /// <param name="userViewModel">DIed UserManagementViewModel</param>
    public UserManagementPage(UserManagementViewModel userViewModel)
	{
		InitializeComponent();
		_userViewModel = userViewModel;
		BindingContext = _userViewModel;

        Initialize();
    }

    /// <summary>
    /// To load the initial data and add any other logic
    /// </summary>
    private async void Initialize()
    {
        await _userViewModel.InitializeAsync();
    }

    private void OnRoleSelectionChanged(object sender, CheckedChangedEventArgs e)
    {
        if (!e.Value) return; // Only handle selection, not deselection

        // Get the selected role
        var radioButton = sender as RadioButton;
        var role = radioButton.BindingContext as UserRoleModel;

        // Call the ViewModel method to update the selection
        _userViewModel.UpdateRoleSelection(role);
    }
}