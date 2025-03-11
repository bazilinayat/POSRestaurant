using POSRestaurant.ViewModels;

namespace POSRestaurant.Pages;

/// <summary>
/// Page where application settings will can be edited
/// </summary>
public partial class SettingsPage : ContentPage
{
	/// <summary>
	/// DIed property to handle SettingsViewModel
	/// </summary>
	private readonly SettingsViewModel _settingsViewModel;

	/// <summary>
	/// Initialize SettingsPage
	/// </summary>
	public SettingsPage(SettingsViewModel settingsViewModel)
	{
		InitializeComponent();
		_settingsViewModel = settingsViewModel;
		BindingContext = _settingsViewModel;

		Initialize();
	}

	/// <summary>
	/// To load the initial data and add other logic that is needed
	/// </summary>
	public async void Initialize()
	{
		await _settingsViewModel.InitializeAsync();
	}
}