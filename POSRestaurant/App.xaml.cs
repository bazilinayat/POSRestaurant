﻿using POSRestaurant.DBO;
using POSRestaurant.ViewModels;
using POSRestaurant.Service.SettingService;
using POSRestaurant.Service;
using POSRestaurant.Service.PaymentService;

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
        public App(IServiceProvider serviceProvider, DatabaseService databaseService, 
            Setting settingService, IAuthService authService, 
            INavigationService navigationService, PaymentMonitoringService paymentMonitoringService)
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

                // Start monitoring for payments
                Task.Run(async () => await paymentMonitoringService.CheckPaymentStatus());
            }
            catch (Exception ex)
            {
                throw;
            }
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
