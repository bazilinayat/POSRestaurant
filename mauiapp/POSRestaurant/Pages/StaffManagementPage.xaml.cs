using POSRestaurant.Data;
using POSRestaurant.ViewModels;
using System;

namespace POSRestaurant.Pages;

/// <summary>
/// Page to mange the staff in the org
/// </summary>
public partial class StaffManagementPage : ContentPage
{
	/// <summary>
	/// View Model for this page
	/// To Be DIed
	/// </summary>
	private readonly StaffManagementViewModel _staffViewModel;

    /// <summary>
    /// Constructor to intialize things
    /// </summary>
    /// <param name="staffViewModel">DIed StaffManagementViewModel</param>
    public StaffManagementPage(StaffManagementViewModel staffViewModel)
	{
		InitializeComponent();
		_staffViewModel = staffViewModel;
		BindingContext = _staffViewModel;

        Initialize();
    }

    /// <summary>
    /// To load the initial data and add any other logic
    /// </summary>
    private async void Initialize()
    {
        await _staffViewModel.InitializeAsync();
    }

    /// <summary>
    /// Event called when we save a staff
    /// </summary>
    /// <param name="staff">Staff to save or update</param>
    private async void SaveStaffControl_OnSaveStaff(Models.StaffEditModel staff)
    {
        await _staffViewModel.SaveStaffCommand.ExecuteAsync(staff);
    }

    /// <summary>
    /// Event called when we delete a staff
    /// </summary>
    /// <param name="staff">Staff to delete</param>
    private async void SaveStaffControl_OnDeleteItem(Models.StaffEditModel staff)
    {
        await _staffViewModel.DeleteItemCommand.ExecuteAsync(staff);
    }

    /// <summary>
    /// Event called when cancel button is clicked
    /// </summary>
    private void SaveStaffControl_OnCancel()
    {
        _staffViewModel.CancelCommand.Execute(null);
    }
}