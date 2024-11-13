using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using POSRestaurant.Data;
using POSRestaurant.Models;
using System.Collections.ObjectModel;

namespace POSRestaurant.ViewModels
{
    /// <summary>
    /// ViewModel for Order operations
    /// </summary>
    public partial class OrdersViewModel : ObservableObject
    {
        /// <summary>
        /// DIed variable for DatabaseService
        /// </summary>
        private readonly DatabaseService _databaseService;

        /// <summary>
        /// ObservableCollection for orders
        /// </summary>
        public ObservableCollection<OrderModel> Orders { get; set; } = new();

        /// <summary>
        /// To indicate that the ViewModel data is loading
        /// </summary>
        [ObservableProperty]
        private bool _isLoading;

        /// <summary>
        /// To store the order items of selected order
        /// </summary>
        [ObservableProperty]
        private OrderItem[] _orderItems = [];

        /// <summary>
        /// To store the order items of selected order
        /// </summary>
        [ObservableProperty]
        private KOTModel[] _orderKOTs = [];

        private int TableId = 1;

        /// <summary>
        /// To check if ViewModel is already initialized
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// Constructor for the HomeViewModel
        /// </summary>
        /// <param name="databaseService">DI for DatabaseService</param>
        public OrdersViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        /// <summary>
        /// Command to place an order
        /// </summary>
        /// <param name="cartItems">List of items in the cart</param>
        /// <param name="isPaidOnline">True if paid online, False if paid cash</param>
        /// <returns>Returns true if successful, false otherwise</returns>
        //public async Task<bool> PlaceOrderAsync(CartItemModel[] cartItems, bool isPaidOnline)
        //{
        //    var orderItems = cartItems.Select(o => new OrderItem
        //    {
        //        Icom = o.Icon,
        //        ItemId = o.ItemId,
        //        Name = o.Name,
        //        Price = o.Price,
        //        Quantity = o.Quantity
        //    }).ToArray();

        //    var orderModel = new OrderModel
        //    {
        //        OrderDate = DateTime.Now,
        //        PaymentMode = isPaidOnline ? PaymentModes.Online : PaymentModes.Cash,
        //        TotalItemCount = cartItems.Length,
        //        TotalPrice = cartItems.Sum(x => x.Price),
        //        KOTs = orderItems
        //    };

        //    var errorMessage = await _databaseService.PlaceOrderAsync(orderModel);

        //    if (errorMessage != null)
        //    {
        //        await Shell.Current.DisplayAlert("Error", errorMessage.ToString(), "Ok");
        //        return false;
        //    }

        //    Orders.Add(orderModel);
        //    await Shell.Current.DisplayAlert("Success", "Order Placeed Successfully", "Ok");
        //    return true;
        //}

        /// <summary>
        /// Initialize the ViewModel
        /// Fetch data and assign
        /// </summary>
        /// <returns>Returns a Task object</returns>
        public async ValueTask InitializeAsync()
        {
            if (_isInitialized) return;

            _isInitialized = true;
            IsLoading = true;

            var dbOrders = await _databaseService.GetOrdersAsync();
            var orders = dbOrders.Select(o => new OrderModel
            {
                Id = o.Id,
                TableId = o.TableId,
                OrderDate = o.OrderDate,
                TotalItemCount = o.TotalItemCount,
                TotalPrice = o.TotalPrice,
                PaymentMode = o.PaymentMode,
                OrderStatus = o.OrderStatus,
            });

            foreach (var order in orders)
            {
                Orders.Add(order);
            }
            IsLoading = false;
        }

        /// <summary>
        /// Command to get the items of selected order
        /// </summary>
        /// <param name="orderModel">Selected Order</param>
        /// <returns>Returns Task Object</returns>
        [RelayCommand]
        private async Task SelectOrderAsync(OrderModel? orderModel)
        {
            if (orderModel == null || orderModel.Id == 0)
            {
                OrderItems = [];
                return;
            }

            var prevSelectedOrder = Orders.FirstOrDefault(o => o.IsSelected);
            if (prevSelectedOrder != null)
            {
                prevSelectedOrder.IsSelected = false;
                if (prevSelectedOrder.Id == orderModel.Id)
                {
                    OrderItems = [];
                    return;
                }
            }

            IsLoading = true;
            orderModel.IsSelected = true;
            //OrderItems = await _databaseService.GetOrderItemsAsync(orderModel.Id);

            OrderKOTs = (await _databaseService.GetOrderKotsAsync(orderModel.Id))
                            .Select(KOTModel.FromEntity)
                            .ToArray();

            foreach (var kot in OrderKOTs)
            {
                kot.Items = await _databaseService.GetKotItemsAsync(kot.Id);
            }

            IsLoading = false;
        }

        /// <summary>
        /// Command to clear all the cart items
        /// </summary>
        [RelayCommand]
        private void ClearCart()
        {
            if (OrderItems.Length == 0)
                return;

            foreach (var order in Orders)
            {
                order.IsSelected = false;
            }

            OrderItems = [];
        }

        #region KOT Flow

        /// <summary>
        /// Command to place an order
        /// </summary>
        /// <param name="cartItems">List of items in the cart</param>
        /// <returns>Returns true if successful, false otherwise</returns>
        public async Task<bool> PlaceKOTAsync(CartItemModel[] cartItems, TableModel tableModel)
        {
            var kotItems = cartItems.Select(o => new KOTItem
            {
                Icom = o.Icon,
                ItemId = o.ItemId,
                Name = o.Name,
                Price = o.Price,
                Quantity = o.Quantity
            }).ToArray();

            var kotModel = new KOTModel
            {
                KOTDateTime = DateTime.Now,
                TotalItemCount = cartItems.Length,
                TotalPrice = cartItems.Sum(x => x.Price),
                Items = kotItems
            };

            List<KOTModel> kots = new List<KOTModel>();
            kots.Add(kotModel);

            var latestOrderId = await _databaseService.GetLatestOrderId();

            string? errorMessage;

            if (latestOrderId != 0)
            {
                // existing order, add kot
                errorMessage = await _databaseService.InsertOrderKOTAsync(kots.ToArray(), latestOrderId);

                if (errorMessage != null)
                {
                    await Shell.Current.DisplayAlert("Error", errorMessage.ToString(), "Ok");
                    return false;
                }
            }
            else
            {
                // new order, place order
                var orderModel = new OrderModel
                {
                    TableId = TableId,
                    OrderDate = DateTime.Now,
                    TotalItemCount = kots.Sum(x => x.TotalItemCount),
                    TotalPrice = kots.Sum(x => x.TotalPrice),
                    KOTs = kots.ToArray(),
                    OrderStatus = TableOrderStatus.Running
                };

                errorMessage = await _databaseService.PlaceOrderAsync(orderModel);

                if (errorMessage != null)
                {
                    await Shell.Current.DisplayAlert("Error", errorMessage.ToString(), "Ok");
                    return false;
                }

                Orders.Add(orderModel);
            }

            await Shell.Current.DisplayAlert("Success", "Order Placeed Successfully", "Ok");
            return true;
        }

        #endregion
    }
}
