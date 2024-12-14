using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Controls;
using POSRestaurant.ChangedMessages;
using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace POSRestaurant.ViewModels
{
    public partial class OrderCompleteViewModel : ObservableObject
    {
        /// <summary>
        /// To handle the calls to database for the order
        /// </summary>
        private readonly DatabaseService _databaseService;

        /// <summary>
        /// To add details to the UI
        /// </summary>
        public TableModel TableModel { get; set; }

        /// <summary>
        /// Tip given by customer
        /// To be taken from UI
        /// </summary>
        [ObservableProperty]
        private decimal _tip;

        /// <summary>
        /// Amount to return to customer
        /// To be taken from UI
        /// </summary>
        [ObservableProperty]
        private decimal _return;

        /// <summary>
        /// Amount received from customer
        /// To be taken from UI
        /// </summary>
        [ObservableProperty]
        private decimal _paidByCustomer;

        /// <summary>
        /// To track the payment mode on UI
        /// </summary>
        private PaymentModes PaymentMode;

        /// <summary>
        /// To manage the selected order type on main page
        /// </summary>
        private int _selectedPaymentMode;

        /// <summary>
        /// To manage the selected order type on main page
        /// Should be handled by code as well
        /// </summary>
        public int SelectedPaymentMode
        {
            get => _selectedPaymentMode;
            set
            {
                if (_selectedPaymentMode != value)
                {
                    _selectedPaymentMode = value;
                    PaymentMode = (PaymentModes)_selectedPaymentMode;
                    OnPaymenModeChanged();
                }
            }
        }

        /// <summary>
        /// Contructor for the view model
        /// </summary>
        /// <param name="databaseService">DIed DatabaseService</param>
        public OrderCompleteViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            SelectedPaymentMode = 1;
        }

        /// <summary>
        /// To handle the property changed event for the radio button switch
        /// </summary>
        public event PropertyChangedEventHandler PaymentModePropertyChanged;

        /// <summary>
        /// Called when OrderType is changed
        /// </summary>
        /// <param name="orderType">Selected OrderType name</param>
        protected virtual void OnPaymenModeChanged([CallerMemberName] string orderType = null)
        {
            PaymentModePropertyChanged?.Invoke(this, new PropertyChangedEventArgs(orderType));
        }

        /// <summary>
        /// Command to be called when search box changes
        /// </summary>
        [RelayCommand]
        private void CalculateReturn()
        {
            Return = PaidByCustomer - TableModel.OrderTotal;
        }

        /// <summary>
        /// Command to save the order payment details
        /// </summary>
        /// <returns>retuns a taks object</returns>
        [RelayCommand]
        private async Task SaveOrderPaymentAsync()
        {
            var orderPayment = new OrderPayment
            {
                OrderId = TableModel.RunningOrderId,
                PaidByCustomer = PaidByCustomer,
                PaymentMode = PaymentMode,
                Tip = Tip,
                Total = TableModel.OrderTotal
            };

            var errorMessage = await _databaseService.OrderPaymentOperations.SaveOrderPaymentAsync(orderPayment);

            if (errorMessage != null)
            {
                await Shell.Current.DisplayAlert("Order Payment Error", errorMessage, "Ok");
                return;
            }

            TableModel.Status = TableOrderStatus.NoOrder;
            TableModel.Waiter = null;
            TableModel.OrderTotal = 0;
            TableModel.RunningOrderId = 0;
            TableModel.NumberOfPeople = 0;

            WeakReferenceMessenger.Default.Send(TableChangedMessage.From(TableModel));
        }
    }
}
