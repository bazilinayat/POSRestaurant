using POSRestaurant.Data;
using POSRestaurant.Utility;

namespace POSRestaurant
{
    /// <summary>
    /// Main entry point of the application
    /// </summary>
    public partial class App : Application
    {

        /// <summary>
        /// Constructor for the class, just starting the application
        /// </summary>
        /// <param name="databaseService">DI for DatabaseService</param>
        public App(DatabaseService databaseService, SettingService settingService)
        {
            InitializeComponent();

            // Set AppTheme permanently to light
            Application.Current.UserAppTheme = AppTheme.Light;

            MainPage = new AppShell(settingService);

            // Initialize and Seed Database
            Task.Run(async() => await databaseService.InitializeDatabaseAsync()).GetAwaiter().GetResult();            
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
