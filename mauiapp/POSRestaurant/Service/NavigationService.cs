namespace POSRestaurant.Service
{

    public interface INavigationService
    {
        Task NavigateToLoginAsync();
        Task NavigateToMainAsync();
    }

    /// <summary>
    /// Navigation service for application
    /// Specifically for login
    /// </summary>
    public class NavigationService : INavigationService
    {
        /// <summary>
        /// To navigate to login screen
        /// </summary>
        /// <returns>Returns Task</returns>
        public async Task NavigateToLoginAsync()
        {
            await Shell.Current.GoToAsync("login");
        }

        /// <summary>
        /// To navigate to main screen
        /// </summary>
        /// <returns>Returns Task</returns>
        public async Task NavigateToMainAsync()
        {
            await Shell.Current.GoToAsync("main");
        }
    }
}
