using CommunityToolkit.Maui.Views;
using POSRestaurant.Service.SettingService;

namespace POSRestaurant.Controls;

/// <summary>
/// Popup class, for help popup
/// </summary>
public partial class HelpPopup : Popup
{
    /// <summary>
    /// DI SettingService
    /// </summary>
    private readonly Setting _settingService;

    /// <summary>
    /// Constructor for the help popup
    /// </summary>
    /// <param name="settingService">DIed SettingsService</param>
    public HelpPopup(Setting settingService)
	{
		InitializeComponent();
        _settingService = settingService;
        emailLabel.Text = _settingService.Settings.Email;
        phoneLabel.Text = _settingService.Settings.Phone;
	}


    /// <summary>
    /// Event will be called when X label is tapped
    /// </summary>
    /// <param name="sender">Sender as Label</param>
    /// <param name="e">TappedEventArgs</param>
    private async void CloseLabel_Tapped(object sender, TappedEventArgs e) => await this.CloseAsync();

    /// <summary>
    /// Event will be called when Footer is tapped
    /// </summary>
    /// <param name="sender">Sender as Grid</param>
    /// <param name="e">TappedEventArgs</param>
    private async void Footer_Tapped(object sender, TappedEventArgs e)
    {
		await Launcher.Default.OpenAsync(_settingService.Settings.WebsiteURL);
    }

    /// <summary>
    /// Event to be called when 'Copy to Clipboard' of email is clicked
    /// </summary>
    /// <param name="sender">Object as sender</param>
    /// <param name="e">TappedEventArgs</param>
    private async void CopyEmail_Tapped(object sender, TappedEventArgs e)
    {
        await Clipboard.SetTextAsync(_settingService.Settings.Email);
        emailCopyClipboard.Text = "Copied";
        await Task.Delay(2000);
        emailCopyClipboard.Text = "Copy to Clipboard";
    }

    /// <summary>
    /// Event to be called when 'Copy to Clipboard' of phone is clicked
    /// </summary>
    /// <param name="sender">Object as sender</param>
    /// <param name="e">TappedEventArgs</param>
    private async void CopyPhone_Tapped(object sender, TappedEventArgs e)
    {
        await Clipboard.SetTextAsync(_settingService.Settings.Phone);
        phoneCopyClipboard.Text = "Copied";
        await Task.Delay(2000);
        phoneCopyClipboard.Text = "Copy to Clipboard";
    }
}