using POSRestaurant.ViewModels;

namespace POSRestaurant.Pages;

/// <summary>
/// The code behind for login screen
/// </summary>
public partial class LoginPage : ContentPage
{
    /// <summary>
    /// DI for LoginViewModel
    /// </summary>
    private readonly LoginViewModel _loginViewModel;

    /// <summary>
    /// Constructor for login page
    /// </summary>
    /// <param name="loginViewModel">Died LoginViewModel</param>
    public LoginPage(LoginViewModel loginViewModel)
	{
		InitializeComponent();
        _loginViewModel = loginViewModel;
        BindingContext = _loginViewModel;
    }

    /// <summary>
    /// Prevent back navigation when on login page
    /// </summary>
    /// <returns>Returns bool</returns>
    protected override bool OnBackButtonPressed()
    {
        return true;
    }

    /// <summary>
    /// Make sure we can't navigate away from login screen
    /// </summary>
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Shell.SetNavBarIsVisible(this, true);
        Shell.SetTabBarIsVisible(this, true);
    }

    /// <summary>
    /// Make sure we can't navigate away from login screen
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        //Shell.SetNavBarIsVisible(this, false);
        //Shell.SetTabBarIsVisible(this, false);
    }
}