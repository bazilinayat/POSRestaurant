using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using POSRestaurant.Service;

namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// ViewModel for Login Page
    /// </summary>
    public partial class LoginViewModel : ObservableObject
    {
        /// <summary>
        /// DI for IAuthService
        /// </summary>
        private readonly IAuthService _authService;

        /// <summary>
        /// DI for INavigationService
        /// </summary>
        private readonly INavigationService _navigationService;

        /// <summary>
        /// To accept the username from the user
        /// </summary>
        [ObservableProperty]
        private string _username;

        /// <summary>
        /// To accept the password from user
        /// </summary>
        [ObservableProperty]
        private string _password;

        /// <summary>
        /// To know if the application is busy
        /// </summary>
        [ObservableProperty]
        private bool _isBusy;

        /// <summary>
        /// To display the error message on the login screen
        /// </summary>
        [ObservableProperty]
        private string _errorMessage;

        /// <summary>
        /// To know if there was any error logging in
        /// </summary>
        [ObservableProperty]
        private bool _hasError;

        /// <summary>
        /// Constructor the LoginViewModel
        /// </summary>
        /// <param name="authService">DIed IAuthService</param>
        /// <param name="navigationService">DIed INavigationService</param>
        public LoginViewModel(IAuthService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
        }

        /// <summary>
        /// Command for logging in the user
        /// </summary>
        /// <returns>Returns Task</returns>
        [RelayCommand]
        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Username and password are required";
                HasError = true;
                return;
            }

            try
            {
                IsBusy = true;
                HasError = false;

                var result = await _authService.LoginAsync(Username, Password);

                if (result)
                {
                    // Navigate to main page after successful login
                    await _navigationService.NavigateToMainAsync();

                    // Configure tab visibility based on permissions
                    (Application.Current.MainPage as Shell)?.ConfigureTabVisibility(_authService);
                }
                else
                {
                    ErrorMessage = "Invalid username or password";
                    HasError = true;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Login error: {ex.Message}";
                HasError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
