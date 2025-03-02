using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using POSRestaurant.ChangedMessages;
using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using POSRestaurant.Service.LoggerService;
using System.Collections.ObjectModel;

namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// ViewModel to be used with UserManagementPage
    /// </summary>
    public partial class UserManagementViewModel : ObservableObject
    {
        /// <summary>
        /// DIed variable for DatabaseService
        /// </summary>
        private readonly DatabaseService _databaseService;

        /// <summary>
        /// DIed LogService
        /// </summary>
        private readonly LogService _logger;

        /// <summary>
        /// To indicate that the ViewModel data is loading
        /// </summary>
        [ObservableProperty]
        private bool _isLoading;

        /// <summary>
        /// List of roles for the drop down
        /// </summary>
        public ObservableCollection<UserRoleModel> Roles { get; set; } = new();

        /// <summary>
        /// ObservableCollection for Roles
        /// </summary>
        public ObservableCollection<UserModel> Users { get; set; } = new();

        /// <summary>
        /// Property to observe the selected role on UI
        /// </summary>
        [ObservableProperty]
        private UserEditModel _userToEdit = new();

        /// <summary>
        /// To know if the selected role can be deleted or not
        /// </summary>
        [ObservableProperty]
        private bool _canBeDeleted = false;

        /// <summary>
        /// Constructor for the UserManagementViewModel
        /// </summary>
        /// <param name="logger">DI for LogService</param>
        /// <param name="databaseService">DI for DatabaseService</param>
        public UserManagementViewModel(LogService logger, DatabaseService databaseService)
        {
            _logger = logger;
            _databaseService = databaseService;
        }

        /// <summary>
        /// Initialize the ViewModel
        /// Fetch data and assign
        /// </summary>
        /// <returns>Returns a Task object</returns>
        public async ValueTask InitializeAsync()
        {
            try
            {
                IsLoading = true;

                UserToEdit = null;

                Users.Clear();
                var allUsers = (await _databaseService.UserOperation.GetAllUsersAsync())
                                .Select(UserModel.FromEntity)
                                .ToList();

                foreach (var user in allUsers)
                {
                    Users.Add(user);
                }

                Roles.Clear();
                var allRoles = (await _databaseService.UserOperation.GetAllRolesAsync())
                                .Select(UserRoleModel.FromEntity)
                                .ToList();

                foreach (var role in allRoles)
                {
                    Roles.Add(role);
                }

                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserManagementVM-InitializeAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Manage Role Screen", "OK");
            }
        }

        /// <summary>
        /// To start with adding new role
        /// </summary>
        [RelayCommand]
        private void AddNewUser()
        {
            UserToEdit = new UserEditModel
            {
                Id = 0
            };
            foreach (var user in Users)
            {
                user.IsSelected = false;
            }
            CanBeDeleted = false;
        }

        public void UpdateRoleSelection(UserRoleModel role)
        {
            // Update all roles
            foreach (var r in Roles)
            {
                r.IsSelected = r.Id == role.Id;
            }

            // Update the user's assigned role ID
            if (UserToEdit != null)
                UserToEdit.AssignedRoleId = role.Id;
        }

        /// <summary>
        /// Command to call when the role is selected
        /// </summary>
        /// <param name="userModel">Selected User</param>
        [RelayCommand]
        private async Task SelectUserAsync(UserModel userModel)
        {
            try
            {
                var prevSelectedOrder = Roles.FirstOrDefault(o => o.IsSelected);
                if (prevSelectedOrder != null)
                {
                    prevSelectedOrder.IsSelected = false;
                    if (prevSelectedOrder.Id == userModel.Id)
                    {
                        Cancel();
                        return;
                    }
                }

                userModel.IsSelected = true;

                // Get all available roles
                var roles = await _databaseService.UserOperation.GetAllRolesAsync();

                // Get the user's assigned role
                var userRole = await _databaseService.UserOperation.GetUserRoleAsync(userModel.Id);

                Roles.Clear();
                foreach (var role in roles)
                {
                    Roles.Add(new UserRoleModel
                    {
                        Id = role.Id,
                        Name = role.Name,
                        IsSelected = userRole != null && userRole.RoleId == role.Id
                    });
                }

                var user = await _databaseService.UserOperation.GetUserByIdAsync(userModel.Id);

                UserToEdit = new UserEditModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    Password = user.Password
                };

                CanBeDeleted = true;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserManagementVM-SelectUserAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Selected Role", "OK");
            }
        }

        /// <summary>
        /// To handle cancel button click on the control
        /// </summary>
        [RelayCommand]
        private void Cancel()
        {
            UserToEdit = new();
            foreach (var user in Users)
            {
                user.IsSelected = false;
            }

            var prevSelectedUser = Roles.FirstOrDefault(o => o.IsSelected);
            if (prevSelectedUser != null)
            {
                prevSelectedUser.IsSelected = false;
            }
            CanBeDeleted = true;
        }

        /// <summary>
        /// To delete the user sent by the control
        /// </summary>
        /// <returns>Returns a Task object</returns>
        [RelayCommand]
        private async Task DeleteItemAsync()
        {
            try
            {
                CanBeDeleted = false;

                if (await _databaseService.UserOperation.DeleteUserAsync(UserToEdit.Id))
                {
                    await Shell.Current.DisplayAlert("Successful", $"{UserToEdit.Username} deleted successfully", "OK");

                    await InitializeAsync();

                    Cancel();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("UserManagementVM-DeleteItemAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Deleting User", "OK");
            }
        }

        /// <summary>
        /// To save or update the user sent by the control
        /// </summary>
        /// <returns>Returns a Task object</returns>
        [RelayCommand]
        private async Task SaveUserAsync()
        {
            try
            {
                IsLoading = true;

                var userModel = new UserModel
                {
                    Id = UserToEdit.Id,
                    Username = UserToEdit.Username,
                    Password = UserToEdit.Password
                };

                if (await _databaseService.UserOperation.SaveUserAsync(UserToEdit) != null)
                {
                    await Shell.Current.DisplayAlert("Successful", "User saved successfully", "OK");

                    await InitializeAsync();

                    Cancel();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Error in Saving User", "OK");
                }

                CanBeDeleted = false;

                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("UserManagementVM-SaveUserAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Saving User", "OK");
            }
        }
    }
}
