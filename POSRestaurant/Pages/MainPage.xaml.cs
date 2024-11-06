using POSRestaurant.Data;
using POSRestaurant.ViewModels;

namespace POSRestaurant.Pages;

/// <summary>
/// MainPage of the Application
/// </summary>
public partial class MainPage : ContentPage
{
    /// <summary>
    /// DIed property to handle the HomeViewModel
    /// </summary>
    private readonly HomeViewModel _homeViewModel;

    /// <summary>
    /// Initialize MainPage
    /// </summary>
    /// <param name="homeViewModel">DI for HomeViewModel</param>
    public MainPage(HomeViewModel homeViewModel)
    {
        InitializeComponent();
        _homeViewModel = homeViewModel;
        BindingContext = _homeViewModel;
        Initialize();
    }

    /// <summary>
    /// To load the initial data and add any other logic
    /// </summary>
    private async void Initialize()
    {
        await _homeViewModel.InitializeAsync();
    }
}
