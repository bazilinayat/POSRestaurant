using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using POSRestaurant.ChangedMessages;
using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using System.Collections.ObjectModel;

namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// ViewModel to be used with StaffManagementPage
    /// </summary>
    public partial class StaffManagementViewModel : ObservableObject
    {
        /// <summary>
        /// DIed variable for DatabaseService
        /// </summary>
        private readonly DatabaseService _databaseService;

        /// <summary>
        /// To indicate that the ViewModel data is loading
        /// </summary>
        [ObservableProperty]
        private bool _isLoading;

        /// <summary>
        /// List of roles for the drop down
        /// </summary>
        public List<RoleModel> Roles { get; set; }

        /// <summary>
        /// ObservableCollection for orders
        /// </summary>
        public ObservableCollection<StaffModel> StaffMembers { get; set; } = new();

        /// <summary>
        /// Property to observe the selected staff on UI
        /// </summary>
        [ObservableProperty]
        private StaffEditModel _staffMember = new();

        /// <summary>
        /// Constructor for the StaffManagementViewModel
        /// </summary>
        /// <param name="databaseService">DI for DatabaseService</param>
        public StaffManagementViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            Roles = Enum.GetValues(typeof(StaffRole))
               .Cast<StaffRole>()
               .Select(t => new RoleModel { Id = (int)t, RoleName = t.ToString(), IsSelected = false }).ToList();
        }

        /// <summary>
        /// Initialize the ViewModel
        /// Fetch data and assign
        /// </summary>
        /// <returns>Returns a Task object</returns>
        public async ValueTask InitializeAsync()
        {
            IsLoading = true;

            StaffMembers.Clear();
            var allStaff = await _databaseService.StaffOperaiotns.GetAllStaff();
            var staffMembers = allStaff.Select(o => new StaffModel
            {
                Id = o.Id,
                Name = o.Name,
                PhoneNumber = o.PhoneNumber,
                Role = o.Role,
                IsSelected = false
            });

            foreach (var staff in staffMembers)
            {
                StaffMembers.Add(staff);
            }
            foreach (var role in Roles)
            {
                role.IsSelected = false;
            }

            IsLoading = false;
        }

        /// <summary>
        /// Command to call when the Staff is selected
        /// </summary>
        /// <param name="staffModel">Selected Staff</param>
        [RelayCommand]
        private async Task SelectStaffAsync(StaffModel staffModel)
        {
            var prevSelectedOrder = StaffMembers.FirstOrDefault(o => o.IsSelected);
            if (prevSelectedOrder != null)
            {
                prevSelectedOrder.IsSelected = false;
                if (prevSelectedOrder.Id == staffModel.Id)
                {
                    Cancel();
                    return;
                }
            }

            staffModel.IsSelected = true;

            foreach(var role in Roles)
            {
                if (role.Id == (int)staffModel.Role)
                    role.IsSelected = true;
            }

            StaffMember = new StaffEditModel
            {
                Id = staffModel.Id,
                Name = staffModel.Name,
                PhoneNumber = staffModel.PhoneNumber,
                Role = staffModel.Role
            };
        }

        /// <summary>
        /// To save or update the staff sent by the control
        /// </summary>
        /// <param name="staff">Staff to save or update</param>
        /// <returns>Returns a Task object</returns>
        [RelayCommand]
        private async Task SaveStaffAsync(StaffEditModel staff)
        {
            IsLoading = true;

            var staffModel = new StaffModel
            {
                Id = staff.Id,
                Name = staff.Name,
                PhoneNumber = staff.PhoneNumber,
                Role = staff.Role,
            };

            var errorMessage = await _databaseService.StaffOperaiotns.SaveStaffAsync(staffModel);

            if (errorMessage != null)
            {
                await Shell.Current.DisplayAlert("Error", errorMessage, "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Successful", "Staff saved successfully", "OK");

                await InitializeAsync();

                // Push for change in staff info
                WeakReferenceMessenger.Default.Send(StaffChangedMessage.From(staffModel));

                Cancel();
            }

            IsLoading = false;
        }

        /// <summary>
        /// To delete the staff sent by the control
        /// </summary>
        /// <param name="staff">Staff to delete</param>
        /// <returns>Returns a Task object</returns>
        [RelayCommand]
        private async Task DeleteItemAsync(StaffEditModel staff)
        {
            var staffModel = new StaffModel
            {
                Id = staff.Id,
                Name = staff.Name,
                PhoneNumber = staff.PhoneNumber,
                Role = staff.Role,
            };

            if (await _databaseService.StaffOperaiotns.DeleteStaffAsync(staffModel) > 0)
            {
                await Shell.Current.DisplayAlert("Successful", $"{staff.Name} deleted successfully", "OK");

                await InitializeAsync();

                Cancel();
            }
        }

        /// <summary>
        /// To handle cancel button click on the control
        /// </summary>
        [RelayCommand]
        private void Cancel()
        {
            StaffMember = new();
            foreach (var role in Roles)
            {
                role.IsSelected = false;
            }
        }

        /// <summary>
        /// To start with adding new staff
        /// </summary>
        [RelayCommand]
        private void AddNewStaff()
        {
            StaffMember = new StaffEditModel
            {
                Id = 0
            };
            foreach(var role in Roles)
            {
                role.IsSelected = false;
            }
        }
    }
}
