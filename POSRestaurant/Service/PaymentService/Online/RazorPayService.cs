using POSRestaurant.Data;
using POSRestaurant.DBO;
using POSRestaurant.PaymentService.Models.Paytm.CreateQR;
using POSRestaurant.Service.LoggerService;
using POSRestaurant.Service.PaymentService.Models.RazorPay;
using POSRestaurant.Service.SettingService;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace POSRestaurant.PaymentService.Online
{
    /// <summary>
    /// Service class to handle all the paytm related payment transactions
    /// </summary>
    public class RazorPayService
    {
        /// <summary>
        /// To DI the log service
        /// </summary>
        private readonly LogService _logger;

        /// <summary>
        /// DI SettingService
        /// </summary>
        private readonly Setting _settingService;

        /// <summary>
        /// DI DatabaseService
        /// </summary>
        private readonly DatabaseService _databaseService;

        /// <summary>
        /// To initialize the paytm service
        /// And add DI components too
        /// </summary>
        /// <param name="logger">DIed LogService</param>
        /// <param name="settingService">DIed SettingsService</param>
        /// <param name="databaseService">DIed DatabaseService</param>
        public RazorPayService(LogService logger, Setting settingService, DatabaseService databaseService)
        {
            _logger = logger;
            _settingService = settingService;
            _databaseService = databaseService;
        }

        /// <summary>
        /// Call the api to create the dynamic qr code
        /// </summary>
        /// <param name="orderId">OrderId to be referenced by paytm</param>
        /// <param name="orderAmount">Amount of the order for generating qr</param>
        /// <retun>Returns a Task</retun>
        public async Task<(string?, string)> GenerateDynamicQR(long orderId, decimal orderAmount)
        {
            string qrCode = null;
            string qrCodeId = string.Empty;
            try
            {
                var restaurantInfo = await _databaseService.SettingsOperation.GetRestaurantInfo();

                using HttpClient client = new HttpClient();

                // Set basic authentication
                var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                    $"{_settingService.Settings.RazorPayInfo.KEYID}:{_settingService.Settings.RazorPayInfo.KEYSECRET}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

                var createQr = new CreateQR
                {
                    Name = restaurantInfo.Name,
                    PaymentAmount = (double)orderAmount * 100,
                    Description = $"Order #{orderId}",
                    Notes = new Note
                    {
                        Purpose = "Billing Purpose"
                    }
                };

                var jsonContent = new StringContent(JsonSerializer.Serialize(createQr), Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(_settingService.Settings.RazorPayInfo.CreateQRURL, jsonContent);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var successResponse = JsonSerializer.Deserialize<QRSuccessResponse>(responseContent);
                    qrCode = successResponse.ImageUrl;
                    qrCodeId = successResponse.Id;
                    var orderReference = new OrderOnlineReference
                    {
                        OrderId = orderId,
                        ReferenceId = successResponse.Id
                    };
                    await _databaseService.OrderPaymentOperations.SaveOrderOnlineReferenceAsync(orderReference);
                }
                else
                {
                    _logger.LogError($"RazorPayService-GenerateDynamicQR - Failure Response for {orderId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("RazorPayService-GenerateDynamicQR Error", ex);
            }
            return (qrCode, qrCodeId);
        }

        public async Task<OnlinePaymentStatus> SeeQRpaymentStatus(string qrId)
        {
            OnlinePaymentStatus qrCodeStatus = OnlinePaymentStatus.NoStatus;
            try
            {
                var restaurantInfo = await _databaseService.SettingsOperation.GetRestaurantInfo();

                using HttpClient client = new HttpClient();

                // Set basic authentication
                var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                    $"{_settingService.Settings.RazorPayInfo.KEYID}:{_settingService.Settings.RazorPayInfo.KEYSECRET}"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

                HttpResponseMessage response = await client.GetAsync(string.Format(_settingService.Settings.RazorPayInfo.FetchPaymentURL, qrId));
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var successResponse = JsonSerializer.Deserialize<QRPaymentsSuccessResponse>(responseContent);

                    if (successResponse.Count > 0)
                    {
                        if (successResponse.Items[0].Captured)
                        {
                            qrCodeStatus = OnlinePaymentStatus.Completed;
                        }
                    }
                }
                else
                {
                    _logger.LogError($"RazorPayService-SeeQRpaymentStatus - Failure Response for {qrId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("RazorPayService-SeeQRpaymentStatus Error", ex);
            }
            return qrCodeStatus;
        }
    }
}
