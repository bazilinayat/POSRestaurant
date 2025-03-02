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
    /// ViewModel to be used with RoleManagementPage
    /// </summary>
    public partial class RoleManagementViewModel : ObservableObject
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
        public List<PermissionModel> Permissions { get; set; } = new();

        /// <summary>
        /// ObservableCollection for Roles
        /// </summary>
        public ObservableCollection<UserRoleModel> Roles { get; set; } = new();

        /// <summary>
        /// Property to observe the selected role on UI
        /// </summary>
        [ObservableProperty]
        private UserRoleEditModel _roleToEdit = new();

        /// <summary>
        /// To know if the selected role can be deleted or not
        /// </summary>
        [ObservableProperty]
        private bool _canBeDeleted = false;

        /// <summary>
        /// Constructor for the RoleManagementViewModel
        /// </summary>
        /// <param name="logger">DI for LogService</param>
        /// <param name="databaseService">DI for DatabaseService</param>
        public RoleManagementViewModel(LogService logger, DatabaseService databaseService)
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

                RoleToEdit = null;

                Roles.Clear();
                var allRoles = (await _databaseService.UserOperation.GetAllRolesAsync())
                                .Select(UserRoleModel.FromEntity)
                                .ToList();

                foreach (var role in allRoles)
                {
                    Roles.Add(role);
                }

                Permissions.Clear();
                var allPermissions = (await _databaseService.UserOperation.GetAllPermissionsAsync())
                                .Select(PermissionModel.FromEntity)
                                .ToList();

                foreach (var permission in allPermissions)
                {
                    Permissions.Add(permission);
                }

                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("RoleManagementVM-InitializeAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Manage Role Screen", "OK");
            }
        }

        /// <summary>
        /// To start with adding new role
        /// </summary>
        [RelayCommand]
        private void AddNewRole()
        {
            RoleToEdit = new UserRoleEditModel
            {
                Id = 0,
                Permissions = Permissions
            };
            foreach (var role in Roles)
            {
                role.IsSelected = false;
            }
            CanBeDeleted = false;
        }

        /// <summary>
        /// Command to call when the role is selected
        /// </summary>
        /// <param name="userRoleModel">Selected Role</param>
        [RelayCommand]
        private async Task SelectRoleAsync(UserRoleModel userRoleModel)
        {
            try
            {
                var prevSelectedOrder = Roles.FirstOrDefault(o => o.IsSelected);
                if (prevSelectedOrder != null)
                {
                    prevSelectedOrder.IsSelected = false;
                    if (prevSelectedOrder.Id == userRoleModel.Id)
                    {
                        Cancel();
                        return;
                    }
                }

                userRoleModel.IsSelected = true;

                foreach (var permission in Permissions)
                {
                    if (userRoleModel.Permissions.Any(o => o.Id == permission.Id))
                        permission.IsSelected = true;
                    else
                        permission.IsSelected = false;
                }

                RoleToEdit = await _databaseService.UserOperation.GetRoleByIdAsync(userRoleModel.Id);
                CanBeDeleted = true;
            }
            catch (Exception ex)
            {
                _logger.LogError("RoleManagementVM-SelectRoleAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Loading Selected Role", "OK");
            }
        }

        /// <summary>
        /// To handle cancel button click on the control
        /// </summary>
        [RelayCommand]
        private void Cancel()
        {
            RoleToEdit = new();
            foreach (var role in Roles)
            {
                role.IsSelected = false;
            }

            var prevSelectedOrder = Roles.FirstOrDefault(o => o.IsSelected);
            if (prevSelectedOrder != null)
            {
                prevSelectedOrder.IsSelected = false;
            }
            CanBeDeleted = true;
        }

        /// <summary>
        /// To delete the role sent by the control
        /// </summary>
        /// <param name="userRole">Role to delete</param>
        /// <returns>Returns a Task object</returns>
        [RelayCommand]
        private async Task DeleteItemAsync()
        {
            try
            {
                var userRoleModel = new UserRoleModel
                {
                    Id = RoleToEdit.Id,
                    Name = RoleToEdit.Name,
                    Permissions = RoleToEdit.Permissions,
                };
                CanBeDeleted = false;

                if (await _databaseService.UserOperation.DeleteRoleAsync(RoleToEdit.Id))
                {
                    await Shell.Current.DisplayAlert("Successful", $"{RoleToEdit.Name} deleted successfully", "OK");

                    await InitializeAsync();

                    Cancel();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("RoleManagementVM-DeleteItemAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Deleting Role", "OK");
            }
        }

        /// <summary>
        /// To save or update the role sent by the control
        /// </summary>
        /// <param name="userRole">Role to save or update</param>
        /// <returns>Returns a Task object</returns>
        [RelayCommand]
        private async Task SaveRoleAsync()
        {
            try
            {
                IsLoading = true;

                var userRoleModel = new UserRoleModel
                {
                    Id = RoleToEdit.Id,
                    Name = RoleToEdit.Name,
                };

                if (await _databaseService.UserOperation.SaveRoleAsync(RoleToEdit) != null)
                {
                    await Shell.Current.DisplayAlert("Successful", "Role saved successfully", "OK");

                    await InitializeAsync();

                    // Push for change in staff info
                    WeakReferenceMessenger.Default.Send(UserRoleChangedMessage.From(userRoleModel));

                    Cancel();
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Error in Saving Role", "OK");
                }

                CanBeDeleted = false;

                IsLoading = false;
            }
            catch (Exception ex)
            {
                _logger.LogError("RoleManagementVM-SaveRoleAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Saving Role", "OK");
            }
        }
    }
}
