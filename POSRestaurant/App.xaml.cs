using POSRestaurant.DBO;
using POSRestaurant.ViewModels;
using POSRestaurant.Service.SettingService;
using POSRestaurant.Service;

namespace POSRestaurant
{
    /// <summary>
    /// Main entry point of the application
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// ServiceProvider for the DIs
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// DI for AuthService
        /// </summary>
        private readonly IAuthService _authService;

        /// <summary>
        /// DI for NavigationService
        /// </summary>
        private readonly INavigationService _navigationService;

        /// <summary>
        /// Constructor for the class, just starting the application
        /// </summary>
        /// <param name="databaseService">DI for DatabaseService</param>
        public App(IServiceProvider serviceProvider, DatabaseService databaseService, Setting settingService, IAuthService authService, INavigationService navigationService)
        {
            try
            {
                InitializeComponent();

                _authService = authService;
                _navigationService = navigationService;

                // Set AppTheme permanently to light
                Application.Current.UserAppTheme = AppTheme.Light;

                var shellViewModel = serviceProvider.GetRequiredService<ShellViewModel>();

                MainPage = new AppShell(serviceProvider, shellViewModel, settingService);

                // Initialize and Seed Database
                Task.Run(async () => await databaseService.InitializeDatabaseAsync()).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// To navigate to login screen on startup of application
        /// </summary>
        protected override async void OnStart()
        {
            base.OnStart();

            //// Check if user is authenticated
            //if (_authService.IsAuthenticated())
            //{
            //    await _navigationService.NavigateToMainAsync();

            //    // Configure tab visibility based on permissions
            //    (MainPage as Shell)?.ConfigureTabVisibility(_authService);
            //}
            //else
            //{
            //    await _navigationService.NavigateToLoginAsync();
            //}
        }

        /// <summary>
        /// Overriding the create window method for changes as we need.
        /// </summary>
        /// <param name="activationState"></param>
        /// <returns></returns>
        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = base.CreateWindow(activationState);

            window.Height = window.MinimumHeight = 760;
            window.Width = window.MinimumWidth = 1200;

            return window;
        }
    }
}
