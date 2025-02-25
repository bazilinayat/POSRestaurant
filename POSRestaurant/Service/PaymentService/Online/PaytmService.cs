using Paytm;
using POSRestaurant.PaymentService.Models.Paytm;
using POSRestaurant.PaymentService.Models.Paytm.CreateQR;
using POSRestaurant.PaymentService.Models.Paytm.TansactionStatus;
using POSRestaurant.Service.LoggerService;
using POSRestaurant.Service.SettingService;
using System.Net;
using System.Text.Json;

namespace POSRestaurant.PaymentService.Online
{
    /// <summary>
    /// Service class to handle all the paytm related payment transactions
    /// </summary>
    public class PaytmService
    {
        /// <summary>
        /// To DI the log service
        /// </summary>
        private readonly LogService _logService;

        /// <summary>
        /// DI SettingService
        /// </summary>
        private readonly SettingService _settingService;

        /// <summary>
        /// To initialize the paytm service
        /// And add DI components too
        /// </summary>
        /// <param name="logService">DIed LogService</param>
        /// <param name="settingService">DIed SettingsService</param>
        public PaytmService(LogService logService, SettingService settingService)
        {
            _logService = logService;
            _settingService = settingService;
        }

        /// <summary>
        /// Method to generate the signature we need to pass in header of api requests
        /// </summary>
        /// <param name="body">body of the request</param>
        /// <param name="merchantKey">merchant key of paytm account</param>
        /// <returns>Returns signature in case of success, else null</returns>
        private string GenerateSignature(string body, string merchantKey)
        {
            try
            {
                return Checksum.generateSignature(body, merchantKey);
            }
            catch (Exception ex)
            {
                _logService.LogError(ex.Message, ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Call the api to create the dynamic qr code
        /// </summary>
        /// <param name="orderId">OrderId to be referenced by paytm</param>
        /// <param name="orderAmount">Amount of the order for generating qr</param>
        /// <retun>null if failed, else CreateResopnse object</retun>
        public CreateResponse GenerateDynamicQR(string orderId, decimal orderAmount)
        {
            CreateResponse createResponse = null;
            try
            {
                var createBody = new CreateRequestBody
                {
                    MID = _settingService.Settings.PaytmInfo.MID,
                    OrderId = orderId,
                    Amount = orderAmount,
                    BusinessType = _settingService.Settings.PaytmInfo.BusinessType,
                    PosId = _settingService.Settings.PaytmInfo.POSID
                };
                /*
                * Generate checksum by parameters we have in body
                * Find your Merchant Key in your Paytm Dashboard at https://dashboard.paytm.com/next/apikeys 
                */
                string paytmChecksum = Checksum.generateSignature(JsonSerializer.Serialize(createBody), _settingService.Settings.PaytmInfo.MerchantKey);

                var createHead = new PaytmAPIHead
                {
                    ClientId = _settingService.Settings.PaytmInfo.MerchantKey,
                    Version = _settingService.Settings.PaytmInfo.Version,
                    Signature = paytmChecksum,
                };

                var requestBody = new CreateRequest
                {
                    Head = createHead,
                    Body = createBody,
                };

                string post_data = JsonSerializer.Serialize(requestBody);

                //For  Staging
                string url = _settingService.Settings.PaytmInfo.CreateQRURL;

                //For  Production  url
                //string  url  =  "https://securegw.paytm.in/paymentservices/qr/create";

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                webRequest.ContentLength = post_data.Length;

                using (StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    requestWriter.Write(post_data);
                }

                string responseData = string.Empty;

                using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {
                    responseData = responseReader.ReadToEnd();

                    createResponse = JsonSerializer.Deserialize<CreateResponse>(responseData);
                }
            }
            catch (Exception ex)
            {
                _logService.LogError(ex.Message, ex);
            }
            return createResponse;
        }

        /// <summary>
        /// Call the transaction status api
        /// </summary>
        /// <param name="orderId">Order id referenced by paytm, same one passed in create qr</param>
        /// <returns>null if failed, else TSReponse object</returns>
        public TSResponse CheckTransactionStatus(string orderId)
        {
            TSResponse tsResponse = null;

            try
            {
                var tsbody = new TSRequestBody
                {
                    MID = _settingService.Settings.PaytmInfo.MID,
                    OrderId = orderId,
                };

                /*
                * Generate checksum by parameters we have in body
                * Find your Merchant Key in your Paytm Dashboard at https://dashboard.paytm.com/next/apikeys 
                */
                string paytmChecksum = Checksum.generateSignature(JsonSerializer.Serialize(tsbody), _settingService.Settings.PaytmInfo.MerchantKey);

                var tsHead = new PaytmAPIHead
                {
                    Signature = paytmChecksum,
                };

                var requestBody = new TSRequest
                {
                    Head = tsHead,
                    Body = tsbody,
                };

                string post_data = JsonSerializer.Serialize(requestBody);

                //For  Staging
                string url = _settingService.Settings.PaytmInfo.TransactionStatusURL;

                //For  Production 
                //string  url  =  "https://securegw.paytm.in/v3/order/status";

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);

                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";
                webRequest.ContentLength = post_data.Length;

                using (StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    requestWriter.Write(post_data);
                }

                string responseData = string.Empty;

                using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {
                    responseData = responseReader.ReadToEnd();

                    tsResponse = JsonSerializer.Deserialize<TSResponse>(responseData);
                }
            }
            catch (Exception ex)
            {
                _logService.LogError(ex.Message, ex);
            }

            return tsResponse;
        }
    }
}
