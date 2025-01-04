using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using POSRestaurant.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace POSRestaurant.Controls;

/// <summary>
/// Popup to complete the order
/// </summary>
public partial class OrderCompletePopup : Popup
{
    /// <summary>
    /// DIed OrderComplete View Model
    /// </summary>
    private readonly OrderCompleteViewModel _orderCompleteViewModel;

    /// <summary>
    /// Constructor for the page for completing order
    /// </summary>
    /// <param name="orderCompleteViewModel">DIed OrderCompleteViewModel</param>
    /// <param name="tableModel">Passed TableModel value</param>
    public OrderCompletePopup(OrderCompleteViewModel orderCompleteViewModel, TableModel tableModel)
    {
        InitializeComponent();

        _orderCompleteViewModel = orderCompleteViewModel;

        _orderCompleteViewModel.TableModel = tableModel;

        BindingContext = _orderCompleteViewModel;

        Initialize();
    }

    /// <summary>
    /// To load the initial data and add any other logic
    /// </summary>
    private async void Initialize()
    {
        await _orderCompleteViewModel.InitializeAsync();
    }

    /// <summary>
    /// Event will be called when X label is tapped
    /// </summary>
    /// <param name="sender">Sender as Label</param>
    /// <param name="e">TappedEventArgs</param>
    private async void CloseLabel_Tapped(object sender, TappedEventArgs e) => await this.CloseAsync();

    /// <summary>
    /// Event called when 'Paid By Customer' field is changed
    /// </summary>
    /// <param name="sender">Sender of the event</param>
    /// <param name="e">Event args</param>
    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        _orderCompleteViewModel.CalculateReturnCommand.Execute(e.NewTextValue);
    }

    /// <summary>
    /// Event will be called when cancel button is clicked
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">EventArgs</param>
    private async void Button_Clicked(object sender, EventArgs e) => await this.CloseAsync();

    /// <summary>
    /// Event will be called when save button is clicked
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">EventArgs</param>
    private async void Button_Clicked_1(object sender, EventArgs e)
    {
        await _orderCompleteViewModel.SaveOrderPaymentCommand.ExecuteAsync(null);
        await this.CloseAsync();
    }

    /// <summary>
    /// Event called when check for payment type is changed in Part payment
    /// </summary>
    /// <param name="sender">CheckBox</param>
    /// <param name="e">EventArgs</param>
    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        _orderCompleteViewModel.CalculateReturnCommand.Execute(null);
    }
}