using POSRestaurant.Data;

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
        public App(DatabaseService databaseService)
        {
            InitializeComponent();

            // Set AppTheme permanently to light
            // Application.Current.UserAppTheme = AppTheme.Light;

            MainPage = new AppShell();

            // Initialize and Seed Database
            Task.Run(async() => await databaseService.InitializeDatabaseAsync()).GetAwaiter().GetResult();            
        }
    }
}
