using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using POSRestaurant.ChangedMessages;
using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using POSRestaurant.Service.LoggerService;
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
        /// DIed LogService
        /// </summary>
        private readonly LogService _logger;

        /// <summary>
        /// To add details to the UI
        /// </summary>
        public TableModel TableModel { get; set; }

        /// <summary>
        /// To add details to the UI
        /// </summary>
        public OrderModel OrderModel { get; set; }

        /// <summary>
        /// To know if the order is on table or pickup
        /// </summary>
        [ObservableProperty]
        private bool _isDineIn;

        /// <summary>
        /// To track the payment mode on UI
        /// </summary>
        private PaymentModes PaymentMode;

        /// <summary>
        /// To manage the selected order type on main page
        /// </summary>
        private int _selectedPaymentMode;

        /// <summary>
        /// To know if the payment is done in parts
        /// </summary>
        [ObservableProperty]
        private bool _isPartPayment;

        /// <summary>
        /// To know if the payment is done in parts
        /// </summary>
        [ObservableProperty]
        private bool _isNotPartPayment;

        /// <summary>
        /// In case of part payment, is cash selected
        /// </summary>
        [ObservableProperty]
        private bool _isCashForPart;

        /// <summary>
        /// In case of part payment, is card selected
        /// </summary>
        [ObservableProperty]
        private bool _isCardForPart;

        /// <summary>
        /// In case of part payment, is online selected
        /// </summary>
        [ObservableProperty]
        private bool _isOnlineForPart;

        /// <summary>
        /// In case of part payment, how much is paid in cash
        /// </summary>
        [ObservableProperty]
        private decimal _paidByCustomerInCash;

        /// <summary>
        /// In case of part payment, how much is paid in card
        /// </summary>
        [ObservableProperty]
        private decimal _paidByCustomerInCard;
        
        /// <summary>
        /// In case of part payment, how much is paid in online
        /// </summary>
        [ObservableProperty]
        private decimal _paidByCustomerInOnline;

        /// <summary>
        /// To manage the selected order type on main page
        /// Should be handled by code as well
        /// </summary>
        public int SelectedPaymentMode
        {
            get => _selectedPaymentMode;
            set
            {
                try
                {
                    if (_selectedPaymentMode != value)
                    {
                        _selectedPaymentMode = value;
                        if (_selectedPaymentMode != 0)
                            PaymentMode = (PaymentModes)_selectedPaymentMode;
                        if (PaymentMode == PaymentModes.Part)
                        {
                            IsPartPayment = true;
                            IsNotPartPayment = false;
                            IsCashForPart = IsCardForPart = IsOnlineForPart = false;
                            PaidByCustomerInCash = PaidByCustomerInCard = PaidByCustomerInOnline = 0;
                        }
                        else if (PaymentMode == PaymentModes.Online || PaymentMode == PaymentModes.Card || PaymentMode == PaymentModes.Cash)
                        {
                            IsPartPayment = false;
                            IsNotPartPayment = true;
                        }
                        CalculateReturn();
                        OnPaymenModeChanged();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("OrderCompleteVM-SelectedPaymentMode Set Error", ex);
                }
            }
        }

        /// <summary>
        /// Contructor for the view model
        /// </summary>
        /// <param name="databaseService">DIed DatabaseService</param>
        public OrderCompleteViewModel(LogService logger, DatabaseService databaseService)
        {
            _logger = logger;
            _databaseService = databaseService;
        }

        /// <summary>
        /// To initialize the starting of the control
        /// </summary>
        /// <returns></returns>
        public async ValueTask InitializeAsync()
        {
            SelectedPaymentMode = 1;

            IsPartPayment = false;
            IsNotPartPayment = true;
            IsCashForPart = IsCardForPart = IsOnlineForPart = false;
            PaidByCustomerInCash = PaidByCustomerInCard = PaidByCustomerInOnline = 0;

            if (TableModel != null)
                IsDineIn = true;
            else
                IsDineIn = false;
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
            if (IsPartPayment)
            {
                if (!IsCashForPart)
                    PaidByCustomerInCash = 0;
                if (!IsCardForPart)
                    PaidByCustomerInCard = 0;
                if (!IsOnlineForPart)
                    PaidByCustomerInOnline = 0;

                var totalPaid = PaidByCustomerInCash + PaidByCustomerInCard + PaidByCustomerInOnline;
            }
        }

        /// <summary>
        /// Command to save the order payment details
        /// </summary>
        /// <returns>retuns a taks object</returns>
        [RelayCommand]
        private async Task SaveOrderPaymentAsync()
        {
            try
            {
                var orderPayment = new OrderPayment
                {
                    OrderId = TableModel != null ? TableModel.RunningOrderId : OrderModel.Id,
                    SettlementDate = DateTime.Now,
                    PaymentMode = PaymentMode,
                    OrderType = OrderTypes.DineIn,
                    Total = TableModel != null ? TableModel.OrderTotal : OrderModel.GrandTotal,
                    IsCardForPart = IsCardForPart,
                    IsCashForPart = IsCashForPart,
                    IsOnlineForPart = IsOnlineForPart,
                    PartPaidInCard = PaidByCustomerInCard,
                    PartPaidInCash = PaidByCustomerInCash,
                    PartPaidInOnline = PaidByCustomerInOnline,
                };

                var errorMessage = await _databaseService.OrderPaymentOperations.SaveOrderPaymentAsync(orderPayment);

                if (errorMessage != null)
                {
                    await Shell.Current.DisplayAlert("Order Payment Error", errorMessage, "Ok");
                    return;
                }

                Order order = null;
                if (TableModel != null)
                    order = await _databaseService.GetOrderById(TableModel.RunningOrderId);
                else
                    order = await _databaseService.GetOrderById(OrderModel.Id);

                if (order != null)
                {
                    order.OrderStatus = TableOrderStatus.Paid;
                    order.PaymentMode = PaymentMode;
                    await _databaseService.UpdateOrder(order);
                }
                else
                {
                    _logger.LogError("OrderCompleteVM-SaveOrderPaymentAsync Order is Empty");
                    await Shell.Current.DisplayAlert("Fault", "Order is Empty", "OK");
                    return;
                }

                if (TableModel != null)
                {
                    TableModel.Status = TableOrderStatus.NoOrder;
                    TableModel.Waiter = null;
                    TableModel.OrderTotal = 0;
                    TableModel.RunningOrderId = 0;
                    TableModel.NumberOfPeople = 0;

                    WeakReferenceMessenger.Default.Send(TableStateChangedMessage.From(TableModel));
                    WeakReferenceMessenger.Default.Send(TableChangedMessage.From(TableModel));
                }
                

                var orderModel = OrderModel.FromEntity(order);
                WeakReferenceMessenger.Default.Send(OrderChangedMessage.From(orderModel));
            }
            catch (Exception ex)
            {
                _logger.LogError("OrderCompleteVM-SaveOrderPaymentAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "Error in Saving Payment Details", "OK");
            }
        }
    }
}
