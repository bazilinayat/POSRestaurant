using POSRestaurant.Pages;
using POSRestaurant.ViewModels;
using POSRestaurant.Service.SettingService;

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
        private readonly Setting _settingService;

        /// <summary>
        /// DI ServiceProvider to get objects
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// DIed property to handle the ShellViewModel
        /// </summary>
        private readonly ShellViewModel _shellViewModel;

        /// <summary>
        /// Constructor for AppShell
        /// </summary>
        /// <param name="serviceProvider">DIed IServiceProvider</param>
        /// <param name="shellViewModel">DIed ShellViewModel</param>
        /// <param name="settingService">DIed SettingsService</param>
        public AppShell(IServiceProvider serviceProvider, ShellViewModel shellViewModel, Setting settingService)
        {
            InitializeComponent();

            _settingService = settingService;
            _serviceProvider = serviceProvider;

            _shellViewModel = shellViewModel;
            BindingContext = shellViewModel;

            Routing.RegisterRoute("Setting", typeof(ManageMenuItemPage));
        }
    }
}
