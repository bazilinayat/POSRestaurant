using CommunityToolkit.Maui.Views;
using POSRestaurant.Controls;
using POSRestaurant.Utility;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace POSRestaurant
{
    /// <summary>
    /// Class for AppShell
    /// </summary>
    public partial class AppShell : Shell
    {
        /// <summary>
        /// DI SettingService
        /// </summary>
        private readonly SettingService _settingService;

        /// <summary>
        /// Constructor for AppShell
        /// </summary>
        /// <param name="settingService">DIed SettingsService</param>
        public AppShell(SettingService settingService)
        {
            InitializeComponent();
            _settingService = settingService;
        }

        /// <summary>
        /// Event for when Help icon is clicked
        /// </summary>
        /// <param name="sender">Image as Sender</param>
        /// <param name="e">TappedEventArgs</param>
        private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            var helpPopup = new HelpPopup(_settingService);
            await this.ShowPopupAsync(helpPopup);
        }
    }
}
