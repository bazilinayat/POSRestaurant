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
    /// ViewModel For Manage KOT Page
    /// </summary>
    public partial class ManageKOTViewModel : ObservableObject, IRecipient<OrderChangedMessage>
    {
        /// <summary>
        /// ServiceProvider for the DIs
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

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
        /// The collection of orders with running status
        /// </summary>
        public ObservableCollection<OrderModel> RunningOrders { get; set; } = new();

        /// <summary>
        /// Constructor for ManageKOTViewModel
        /// </summary>
        /// <param name="serviceProvider">DI for IServiceProvider</param>
        /// <param name="logger">DI for LogService</param>
        /// <param name="databaseService">DI for DatabaseService</param>
        public ManageKOTViewModel(IServiceProvider serviceProvider, LogService logger,
            DatabaseService databaseService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _databaseService = databaseService;

            // Registering for listetning to the WeakReferenceMessenger for item change
            WeakReferenceMessenger.Default.Register<OrderChangedMessage>(this);
        }

        /// <summary>
        /// Initialize the ViewModel
        /// Fetch data and assign
        /// </summary>
        /// <returns>Returns a Task object</returns>
        public async ValueTask InitializeAsync()
        {
            IsLoading = true;
            try
            {
                await GetRunningOrders();
            }
            catch (Exception ex)
            {
                _logger.LogError("ManageKOTVM-InitializeAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "There was an error loading the screen", "OK");
            }
            IsLoading = false;
        }

        /// <summary>
        /// To get all the running order in the application
        /// </summary>
        /// <returns>Returns a Task</returns>
        private async Task GetRunningOrders()
        {
            try
            {
                var orders = (await _databaseService.GetRunningOrdersAsync())
                                    .Select(OrderModel.FromEntity)
                                    .ToArray();

                if (orders.Count() > 0)
                {
                    foreach (var order in orders)
                    {
                        Receive(new OrderChangedMessage(order));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ManageKOTVM-GetRunningPickups Error", ex);
                await Shell.Current.DisplayAlert("Fault", "There was an error loading the orders", "OK");
            }
        }

        /// <summary>
        /// To get the order changed message and update the screen
        /// </summary>
        /// <param name="message">OrderChangedMessage object</param>
        public async void Receive(OrderChangedMessage message)
        {
            IsLoading = true;

            try
            {
                var receivedOrder = message.Value;
                if (receivedOrder != null)
                {
                    if (receivedOrder.OrderStatus == TableOrderStatus.Paid)
                    {
                        var orderToRemove = RunningOrders.Where(o => o.Id == receivedOrder.Id).FirstOrDefault();
                        if (orderToRemove != null)
                            RunningOrders.Remove(orderToRemove);
                    }
                    else if (receivedOrder.OrderStatus == TableOrderStatus.Running)
                    {
                        RunningOrders.Add(receivedOrder);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ManageKOTVM-Receive Order Changed Error", ex);
                await Shell.Current.DisplayAlert("Fault", "There was an error when we received order change", "OK");
                throw;
            }

            IsLoading = false;
        }

        /// <summary>
        /// Command to delete the order from database
        /// </summary>
        /// <returns>Retuns a Task</returns>
        [RelayCommand]
        private async Task DeleteOrderAsync(OrderModel orderToDelete)
        {
            IsLoading = true;

            try
            {
                if (await _databaseService.DeleteCompleteOrderAsync(orderToDelete.Id))
                {
                    orderToDelete.OrderStatus = TableOrderStatus.Paid;
                    Receive(OrderChangedMessage.From(orderToDelete));
                    WeakReferenceMessenger.Default.Send(OrderChangedMessage.From(orderToDelete));
                    WeakReferenceMessenger.Default.Send(TableChangedMessage.From(new TableModel
                    {
                        Id = orderToDelete.TableId,
                        Status = TableOrderStatus.NoOrder
                    }));

                    await Shell.Current.DisplayAlert("Success", "Successfully Deleted Order", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Error DeletingOrder", "OK");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("ManageKOTVM-DeleteOrderAsync Error", ex);
                await Shell.Current.DisplayAlert("Fault", "There was an error in deleting the order", "OK");
            }

            IsLoading = false;
        }
    }
}
