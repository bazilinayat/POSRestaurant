using POSRestaurant.DBO;
using POSRestaurant.Models;

namespace POSRestaurant.Service
{
    /// <summary>
    /// Authentication service for user authentication
    /// </summary>
    public class AuthService : IAuthService
    {
        /// <summary>
        /// DI for DatabaseService
        /// </summary>
        private readonly DatabaseService _databaseService;

        /// <summary>
        /// Instance for current user
        /// </summary>
        private CurrentUser _currentUser;

        /// <summary>
        /// Constructor for AuthService
        /// </summary>
        /// <param name="databaseService">DIed DatabaseService</param>
        public AuthService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            LoadUserFromStorage();
        }

        /// <summary>
        /// Method to handle the login for user
        /// </summary>
        /// <param name="username">username passed</param>
        /// <param name="password">password passed</param>
        /// <returns>Returns bool</returns>
        public async Task<bool> LoginAsync(string username, string password)
        {
            // In a real application, you should hash the password and compare with stored hash
            var users = await _databaseService.UserOperation.GetAllUsersAsync();
            var user = users.Where(o => o.Username == username && o.Password == password).FirstOrDefault();

            if (user == null)
                return false;

            // Get user roles and permissions
            var roles = await _databaseService.UserOperation.GetUserRolesAsync(user.Id);
            var permissions = await _databaseService.UserOperation.GetUserPermissionsAsync(user.Id);

            _currentUser = new CurrentUser
            {
                Id = user.Id,
                Username = user.Username,
                Roles = roles,
                Permissions = permissions
            };

            // Save user info to secure storage
            await SaveUserToStorage();

            return true;
        }

        /// <summary>
        /// Method to logout the logged in user
        /// </summary>
        /// <returns>Returns Task</returns>
        public async Task LogoutAsync()
        {
            _currentUser = null;
            await SecureStorage.Default.SetAsync("is_authenticated", "false");
            SecureStorage.Default.Remove("current_user");
        }

        /// <summary>
        /// To return if the user is authenticated
        /// </summary>
        /// <returns>Returns bool</returns>
        public bool IsAuthenticated()
        {
            return _currentUser != null;
        }

        /// <summary>
        /// To get the current logged in user
        /// </summary>
        /// <returns>Returns CurrentUser object</returns>
        public CurrentUser GetCurrentUser()
        {
            return _currentUser;
        }

        /// <summary>
        /// To check if the logged in user has given permission
        /// </summary>
        /// <param name="permissionName">Permission name to check</param>
        /// <returns>Return bool</returns>
        public bool HasPermission(string permissionName)
        {
            if (_currentUser == null)
                return false;

            return _currentUser.Permissions.Contains(permissionName);
        }

        /// <summary>
        /// To save the user to secure storage
        /// </summary>
        /// <returns>Returns Task</returns>
        private async Task SaveUserToStorage()
        {
            if (_currentUser == null)
                return;

            await SecureStorage.Default.SetAsync("is_authenticated", "true");

            // Serialize current user to JSON and save
            var json = System.Text.Json.JsonSerializer.Serialize(_currentUser);
            await SecureStorage.Default.SetAsync("current_user", json);
        }

        /// <summary>
        /// To load the user from secure storage and assign to current user
        /// </summary>
        private void LoadUserFromStorage()
        {
            try
            {
                var isAuthenticatedTask = SecureStorage.Default.GetAsync("is_authenticated");
                isAuthenticatedTask.Wait();
                var isAuthenticated = isAuthenticatedTask.Result;

                if (isAuthenticated != "true")
                    return;

                var jsonTask = SecureStorage.Default.GetAsync("current_user");
                jsonTask.Wait();
                var json = jsonTask.Result;

                if (string.IsNullOrEmpty(json))
                    return;

                _currentUser = System.Text.Json.JsonSerializer.Deserialize<CurrentUser>(json);
            }
            catch
            {
                _currentUser = null;
            }
        }
    }


    public class PermissionUIService
    {
        private readonly IAuthService _authService;

        public PermissionUIService(IAuthService authService)
        {
            _authService = authService;
        }

        public bool IsVisible(string permissionName)
        {
            return _authService.HasPermission(permissionName);
        }

        public void ConfigureTabVisibility()
        {
            // Hide tabs based on permissions
            var shell = Shell.Current;

            // Check permissions for each tab
            if (!_authService.HasPermission("ViewUsers"))
            {
                // Find and hide the Users tab
                HideTab(shell, "users");
            }

            if (!_authService.HasPermission("ViewReports"))
            {
                // Find and hide the Reports tab
                HideTab(shell, "reports");
            }

            if (!_authService.HasPermission("ViewSettings"))
            {
                // Find and hide the Settings tab
                HideTab(shell, "settings");
            }
        }

        private void HideTab(Shell shell, string route)
        {
            var tabBar = shell.Items.FirstOrDefault() as TabBar;
            if (tabBar != null)
            {
                var tab = tabBar.Items.FirstOrDefault(t => t.Route.Contains(route));
                if (tab != null)
                {
                    tab.IsVisible = false;
                }
            }
        }
    }
}
