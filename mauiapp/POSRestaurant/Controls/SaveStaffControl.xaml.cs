using CommunityToolkit.Mvvm.Input;
using POSRestaurant.Data;
using POSRestaurant.Models;

namespace POSRestaurant.Controls;

/// <summary>
/// Control for editing the staff entries
/// </summary>
public partial class SaveStaffControl : ContentView
{
	/// <summary>
	/// constructor for the editing control
	/// </summary>
	public SaveStaffControl()
	{
		InitializeComponent();
	}

    /// <summary>
    /// BindableProperty for Item to be used on UI
    /// </summary>
    public static readonly BindableProperty StaffToSaveProperty =
        BindableProperty.Create(nameof(StaffToSave), typeof(StaffEditModel), typeof(SaveStaffControl), new StaffEditModel());

    /// <summary>
    /// For the specific item this control is responsible for
    /// </summary>
    public StaffEditModel StaffToSave
    {
        get => (StaffEditModel)GetValue(StaffToSaveProperty);
        set => SetValue(StaffToSaveProperty, value);
    }

    /// <summary>
    /// BindableProperty for Item to be used on UI
    /// </summary>
    public static readonly BindableProperty RolesProperty =
        BindableProperty.Create(nameof(Roles), typeof(List<RoleModel>), typeof(SaveStaffControl), new List<RoleModel>());

    /// <summary>
    /// For the specific item this control is responsible for
    /// </summary>
    public List<RoleModel> Roles
    {
        get => (List<RoleModel>)GetValue(RolesProperty);
        set => SetValue(RolesProperty, value);
    }


    /// <summary>
    /// To change the header
    /// In case it is to be added or updated
    /// </summary>
    public string SaveStaffHeader
    {
        get => CanBeDeleted == 0 ? "Add Staff Details" : "Update Staff Details";
    }

    /// <summary>
    /// BindableProperty for staff to be used on UI
    /// </summary>
    public static readonly BindableProperty CanBeDeletedProperty =
        BindableProperty.Create(nameof(CanBeDeleted), typeof(int), typeof(SaveMenuItemFormControl), -1);

    /// <summary>
    /// A public property to pass when we use the control
    /// </summary>
    public int CanBeDeleted
    {
        get
        {
            var val = (int)GetValue(CanBeDeletedProperty);
            return val > 0 ? 1 : 0;
        }
        set
        {
            SetValue(CanBeDeletedProperty, value);
        }
    }

    /// <summary>
    /// Event to work with save button click on the control
    /// </summary>
    public event Action<StaffEditModel> OnSaveStaff;

    /// <summary>
    /// Command called when Save button is clicked
    /// </summary>
    /// <returns>Returns a task object</returns>
    [RelayCommand]
    private async Task SaveMenuItem()
    {
        // Validation

        if (string.IsNullOrWhiteSpace(StaffToSave.Name))
        {
            await ErrorAlertAsync("Enter a name for the staff");
            return;
        }

        if (string.IsNullOrEmpty(StaffToSave.PhoneNumber))
        {
            await ErrorAlertAsync("Enter a phone number for the staff");
            return;
        }

        OnSaveStaff?.Invoke(StaffToSave);

        static async Task ErrorAlertAsync(string message) =>
            await Shell.Current.DisplayAlert("Validation Error", message, "OK");
    }

    /// <summary>
    /// Event to work with delete button click on the control
    /// </summary>
    public event Action<StaffEditModel> OnDeleteItem;

    /// <summary>
    /// Command called when delete button is clicked
    /// </summary>
    /// <returns>Returns a task object</returns>
    [RelayCommand]
    private async Task DeleteMenuItem()
    {
        if (await Shell.Current.DisplayAlert("Delete Item?", $"Do you really want to delete {StaffToSave.Name}?", "Yes", "No"))
            OnDeleteItem?.Invoke(StaffToSave);
    }

    /// <summary>
    /// Event to work with cancel button click on the control
    /// </summary>
    public event Action? OnCancel;

    /// <summary>
    /// Command to handle the cancel button click
    /// </summary>
    [RelayCommand]
    private void Cancel() => OnCancel?.Invoke();

    /// <summary>
    /// Command to toggle roles
    /// </summary>
    /// <param name="roleModel">RoleModel toggled</param>
    [RelayCommand]
    private void ToggleRoleSelection(RoleModel roleModel)
    {
        if (roleModel.IsSelected)
            return;

        var prevSelectedRole = Roles.FirstOrDefault(o => o.IsSelected);
        if (prevSelectedRole != null)
        {
            prevSelectedRole.IsSelected = false;
        }

        roleModel.IsSelected = true;
        StaffToSave.Role = (StaffRole)roleModel.Id;
    }
}