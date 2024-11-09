using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using POSRestaurant.Data;
using POSRestaurant.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task<bool> PlaceOrderAsync(CartItemModel[] cartItems, bool isPaidOnline)
        {
            var orderItems = cartItems.Select(o => new OrderItem
            {
                Icom = o.Icon,
                ItemId = o.ItemId,
                Name = o.Name,
                Price = o.Price,
                Quantity = o.Quantity
            }).ToArray();

            var orderModel = new OrderModel
            {
                OrderDate = DateTime.Now,
                PaymentMode = isPaidOnline ? PaymentModes.Online : PaymentModes.Cash,
                TotalItemCount = cartItems.Length,
                TotalPrice = cartItems.Sum(x => x.Price),
                Items = orderItems
            };

            var errorMessage = await _databaseService.PlaceOrderAsync(orderModel);

            if (errorMessage != null)
            {
                await Shell.Current.DisplayAlert("Error", errorMessage.ToString(), "Ok");
                return false;
            }

            Orders.Add(orderModel);
            await Shell.Current.DisplayAlert("Success", "Order Placeed Successfully", "Ok");
            return true;
        }

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
                OrderDate = o.OrderDate,
                TotalItemCount = o.TotalItemCount,
                TotalPrice = o.TotalPrice,
                PaymentMode = o.PaymentMode
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
            OrderItems = await _databaseService.GetOrderItemsAsync(orderModel.Id);
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

            OrderItems = [];
        }
    }
}
