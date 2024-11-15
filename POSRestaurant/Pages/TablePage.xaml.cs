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
        Initialize();

        NavigationPage.SetHasBackButton(this, false);
        var bb = new BackButtonBehavior();
        bb.IsEnabled = false;
        bb.IsVisible = false;
        Shell.SetBackButtonBehavior(this, bb);
    }
        
    /// <summary>
    /// To load the initial data and add any other logic
    /// </summary>
    private async void Initialize()
    {
        await _tableViewModel.InitializeAsync();
    }
}