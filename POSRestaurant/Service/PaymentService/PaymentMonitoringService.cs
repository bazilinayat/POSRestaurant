using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Uwp.Notifications;
using POSRestaurant.ChangedMessages;
using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.Models;
using POSRestaurant.PaymentService.Online;
using POSRestaurant.Service.LoggerService;
using System.Collections.Concurrent;

namespace POSRestaurant.Service.PaymentService
{
    /// <summary>
    /// To monitor 
    /// </summary>
    public class PaymentMonitoringService
    {
        /// <summary>
        /// To have a list of qr codes to check for payment status
        /// </summary>
        public ConcurrentDictionary<string, PaymentNecessaryDetail> QrCodes;

        /// <summary>
        /// DIed variable for DatabaseService
        /// </summary>
        private readonly DatabaseService _databaseService;

        /// <summary>
        /// DIed LogService
        /// </summary>
        private readonly LogService _logger;

        /// <summary>
        /// DIed Setting
        /// </summary>
        private readonly RazorPayService _razorPayService;

        /// <summary>
        /// Constructor for the PaymentMonitoringService
        /// </summary>
        /// <param name="databaseService">DI for DatabaseService</param>
        /// <param name="logger">DI for LogService</param>
        /// <param name="razorPayService">DI for RazorPayService</param>
        public PaymentMonitoringService(DatabaseService databaseService, LogService logger,
            RazorPayService razorPayService)
        {
            _logger = logger;
            _databaseService = databaseService;
            _razorPayService = razorPayService;

            QrCodes = new ConcurrentDictionary<string, PaymentNecessaryDetail>();
        }

        /// <summary>
        /// The method to keep running continuously to monitor the qr code statuses
        /// </summary>
        /// <returns>Returns a task</returns>
        public void CheckPaymentStatus()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    int _min = 1000;
                    int _max = 9999;
                    Random _rdm = new Random();
                    List<string> qrCodesToRemove = new List<string>();

                    foreach (var qrCode in QrCodes)
                    {
                        var statusResult = await _razorPayService.SeeQRpaymentStatus(qrCode.Value.QrCodeId);

                        if (statusResult == OnlinePaymentStatus.Completed)
                        {
                            var paymentDetail = await _databaseService.OrderPaymentOperations.GetOrderPaymentById(qrCode.Value.Orderid);

                            var orderPayment = new OrderPayment
                            {
                                OrderId = qrCode.Value.Orderid,
                                SettlementDate = DateTime.Now,
                                PaymentMode = PaymentModes.Online,
                                OrderType = OrderTypes.DineIn,
                                Total = qrCode.Value.OrderTotal,
                                IsCardForPart = false,
                                IsCashForPart = false,
                                IsOnlineForPart = false,
                                PartPaidInCard = 0,
                                PartPaidInCash = 0,
                                PartPaidInOnline = 0,
                            };

                            var errorMessage = await _databaseService.OrderPaymentOperations.SaveOrderPaymentAsync(orderPayment);

                            var onlineReference = await _databaseService.OrderPaymentOperations.GetOrderOnlineReferenceAsync(qrCode.Value.Orderid);
                            onlineReference.OrderPayemntId = orderPayment.Id;
                            await _databaseService.OrderPaymentOperations.SaveOrderOnlineReferenceAsync(onlineReference);

                            var order = await _databaseService.GetOrderById(qrCode.Value.Orderid);
                            if (order != null)
                            {
                                order.OrderStatus = TableOrderStatus.Paid;
                                order.PaymentMode = PaymentModes.Online;
                                await _databaseService.UpdateOrder(order);
                            }

                            if (qrCode.Value.TableModelToUpdate != null)
                            {
                                qrCode.Value.TableModelToUpdate.Status = TableOrderStatus.NoOrder;
                                qrCode.Value.TableModelToUpdate.Waiter = null;
                                qrCode.Value.TableModelToUpdate.OrderTotal = 0;
                                qrCode.Value.TableModelToUpdate.RunningOrderId = 0;
                                qrCode.Value.TableModelToUpdate.NumberOfPeople = 0;

                                WeakReferenceMessenger.Default.Send(TableStateChangedMessage.From(qrCode.Value.TableModelToUpdate));
                                WeakReferenceMessenger.Default.Send(TableChangedMessage.From(qrCode.Value.TableModelToUpdate));
                            }


                            var orderModel = OrderModel.FromEntity(order);
                            WeakReferenceMessenger.Default.Send(OrderChangedMessage.From(orderModel));

                            qrCodesToRemove.Add(qrCode.Key);

                            string title = qrCode.Value.TableModelToUpdate != null ?
                            $"Table #{qrCode.Value.TableModelToUpdate.TableNo} Payment Done of ₹{qrCode.Value.OrderTotal}" :
                            $"Pickup #{orderModel.OrderNumber} Payment Done of ₹{qrCode.Value.OrderTotal}";

                            new ToastContentBuilder()
                            .AddArgument("action", "viewConversation")
                            .AddArgument("conversationId", _rdm.Next(_min, _max))
                            .AddText(title)
                            .Show();
                        }
                    }

                    foreach (var toRemove in qrCodesToRemove)
                        QrCodes.TryRemove(toRemove, out PaymentNecessaryDetail value);

                    Task.Delay(500);
                }
            });
            
        }
    }
}
