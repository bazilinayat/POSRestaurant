using CommunityToolkit.Maui.Views;
using POSRestaurant.Models;
using POSRestaurant.ViewModels;

namespace POSRestaurant.Controls;

/// <summary>
/// Popup to complete the order
/// </summary>
public partial class InitialPopup : Popup
{
    /// <summary>
    /// DIed OrderComplete View Model
    /// </summary>
    private readonly SettingsViewModel _settingsViewModel;

    /// <summary>
    /// Constructor for the page for completing order
    /// </summary>
    /// <param name="settingsViewModel">DIed OrderCompleteViewModel</param>
    public InitialPopup(SettingsViewModel settingsViewModel)
    {
        InitializeComponent();

        _settingsViewModel = settingsViewModel;

        BindingContext = _settingsViewModel;

        Initialize();
    }

    /// <summary>
    /// To load the initial data and add any other logic
    /// </summary>
    private async void Initialize()
    {
        await _settingsViewModel.InitializeAsync();
    }

    /// <summary>
    /// Event will be called when X label is tapped
    /// </summary>
    /// <param name="sender">Sender as Label</param>
    /// <param name="e">TappedEventArgs</param>
    private async void CloseLabel_Tapped(object sender, TappedEventArgs e) => await this.CloseAsync();

    private async void Button_Clicked_2(object sender, EventArgs e)
    {
        await _settingsViewModel.SaveRestaurantInfoCommand.ExecuteAsync(null);
        if (_settingsViewModel.InfoInitialized)
            await this.CloseAsync();
    }
}