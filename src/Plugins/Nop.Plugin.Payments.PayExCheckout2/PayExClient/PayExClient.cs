using Newtonsoft.Json;
using Nop.Plugin.Payments.PayExCheckout2.PayExClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.PayExCheckout2.PayExClient
{
    public class PayExClient : IDisposable
    {
        private readonly HttpClient _httpClient;

        public PayExClient(string host, string merchantToken)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(host);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", merchantToken);
        }

        /// <summary>
        /// The payer will be identified with the consumers resource and will be persisted to streamline future checkout processes. 
        /// <para>Payer identification is done through the initiate-consumer-session operation. In the request body, all properties are optional. The more information that is provided, the easier the identification process becomes for the payer.</para>
        /// </summary>
        /// <param name="msisdn">The MSISDN (mobile phone number) of the payer.</param>
        /// <param name="email">The e-mail address of the payer.</param>
        /// <param name="consumerCountryCode">Consumer's country of residence. Used by the consumerUi for validation on all input fields.</param>
        /// <param name="socialSecurityNumber">The social security number of the payer.</param>
        /// <returns></returns>
        public async Task<CheckinResponseModel> CheckinInitConsumerSessionAsync(string consumerCountryCode, string msisdn = null, string email = null, string socialSecurityNumber = null)
        {
            var request = new CheckinRequestModel
            {
                Email = email,
                Msisdn = msisdn,
                ConsumerCountryCode = consumerCountryCode
            };

            if (socialSecurityNumber != null)
            {
                request.NationalIdentifier = new CheckinRequestModel.CheckinRequest_NationalIdentifier
                {
                    SocialSecurityNumber = socialSecurityNumber,
                    CountryCode = consumerCountryCode
                };
            }
            

            var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var httpResponse = await _httpClient.PostAsync("psp/consumers", content);

            if (httpResponse.IsSuccessStatusCode)
            {
                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<CheckinResponseModel>(responseContent);

                return response;
            }

            return null;
        }

        public async Task<CheckoutResponseModel> CheckoutPaymentOrdersAsync(CheckoutRequestModel model)
        {
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            
            var httpResponse = await _httpClient.PostAsync("psp/paymentorders", content);

            if (httpResponse.IsSuccessStatusCode)
            {
                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<CheckoutResponseModel>(responseContent);

                return response;
            }

            return null;
        }

        public Task<PaymentOrderResponseModel> GetPaymentOrderInfoAsync(string paymentOrderId)
        {
            return GetAsync<PaymentOrderResponseModel>(paymentOrderId + "?$expand=payeeInfo,payer,metadata");
        }

        public async Task<bool> CapturePaymentOrderAsync(string paymentOrderId, int amount, int vatAmount, int vatPercent, string description, string payeeReference = null)
        {
            var captureTransaction = TransactionRequestModel.CreateCaptureModel(amount, vatAmount, vatPercent, description, payeeReference);

            var content = new StringContent(JsonConvert.SerializeObject(captureTransaction), Encoding.UTF8, "application/json");

            var httpResponse = await _httpClient.PostAsync(paymentOrderId + "/captures", content);

            return httpResponse.IsSuccessStatusCode;
        }

        public async Task<bool> CancelPaymentOrderAsync(string paymentOrderId, string description, string payeeReference = null)
        {
            var cancelTransaction = TransactionRequestModel.CreateCancelModel(description, payeeReference);

            var content = new StringContent(JsonConvert.SerializeObject(cancelTransaction), Encoding.UTF8, "application/json");

            var httpResponse = await _httpClient.PostAsync(paymentOrderId + "/cancellations", content);

            return httpResponse.IsSuccessStatusCode;
        }

        public async Task<T> GetAsync<T>(string url)
        {
            var httpResponse = await _httpClient.GetAsync(url);

            if (!httpResponse.IsSuccessStatusCode)
            {
                return default(T);
            }

            var content = await httpResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(content);
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
