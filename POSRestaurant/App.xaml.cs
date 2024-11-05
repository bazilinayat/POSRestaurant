using POSRestaurant.Data;

namespace POSRestaurant
{
    /// <summary>
    /// Main entry point of the application
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Private instance for DatabaseService object
        /// </summary>
        private readonly DatabaseService _databaseService;

        /// <summary>
        /// Constructor for the class, just starting the application
        /// </summary>
        /// <param name="databaseService">DI for DatabaseService</param>
        public App(DatabaseService databaseService)
        {
            InitializeComponent();

            MainPage = new AppShell();
            _databaseService = databaseService;
        }

        /// <summary>
        /// Overriden method, to seed the database
        /// </summary>
        protected override async void OnStart()
        {
            base.OnStart();

            // Initialize and Seed Database
            await _databaseService.InitializeDatabaseAsync();
        }
    }
}
