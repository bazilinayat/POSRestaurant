using CommunityToolkit.Maui.Views;
using POSRestaurant.Controls;
using POSRestaurant.ViewModels;

namespace POSRestaurant.Pages;

public partial class TablePage : ContentPage
{
    /// <summary>
    /// DIed property to handle the TablesViewModel
    /// </summary>
    private readonly TableViewModel _tableViewModel;

    /// <summary>
    /// DIed HomeViewModel
    /// </summary>
    private readonly HomeViewModel _homeViewModel;

    /// <summary>
    /// Initialize OrdersPage
    /// </summary>
    /// <param name="tablesViewModel">DI for TablesViewModel</param>
    public TablePage(HomeViewModel homeViewModel, TableViewModel tableViewModel)
    {
        InitializeComponent();
        _homeViewModel = homeViewModel;
        _tableViewModel = tableViewModel;
        BindingContext = _tableViewModel;

        ((TableViewModel)BindingContext).CommandCompletedSuccessfully += OnCommandCompleted;
        ((TableViewModel)BindingContext).LogoutCompletedSuccessfully += OnLogoutCompleted;
        Initialize();
    }
        
    /// <summary>
    /// To load the initial data and add any other logic
    /// </summary>
    private async void Initialize()
    {
        await _tableViewModel.InitializeAsync();
    }

    /// <summary>
    /// Make sure we can't navigate away from login screen
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (!_tableViewModel._loggedIn)
        {
            Shell.SetNavBarIsVisible(this, false);
            Shell.SetTabBarIsVisible(this, false);
        }
    }

    private void OnCommandCompleted(object sender, EventArgs e)
    {
        // Show navigation bar and tab bar when command completes successfully
        Shell.SetNavBarIsVisible(this, true);
        Shell.SetTabBarIsVisible(this, true);
    }

    private void OnLogoutCompleted(object sender, EventArgs e)
    {
        // Show navigation bar and tab bar when command completes successfully
        Shell.SetNavBarIsVisible(this, false);
        Shell.SetTabBarIsVisible(this, false);
    }
}