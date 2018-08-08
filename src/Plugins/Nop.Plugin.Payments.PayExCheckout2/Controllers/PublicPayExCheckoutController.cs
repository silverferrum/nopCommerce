using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.PayExCheckout2.Models;
using Nop.Services.Customers;
using Nop.Services.Orders;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Logging;
using Nop.Plugin.Payments.PayExCheckout2.PayExClient.Models;
using Microsoft.Net.Http.Headers;
using Nop.Services.Payments;
using Nop.Core.Domain.Common;
using Nop.Services.Directory;
using Nop.Services.Common;
using Nop.Services.Plugins;
using Newtonsoft.Json;

namespace Nop.Plugin.Payments.PayExCheckout2.Controllers
{
    public class PublicPayExCheckoutController : BasePluginController
    {
        #region Fields

        //private readonly ICheckoutModelFactory _checkoutModelFactory;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IShoppingCartService _shoppingCartService;
        //private readonly ILocalizationService _localizationService;
        //private readonly IProductAttributeParser _productAttributeParser;
        //private readonly IProductService _productService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ICustomerService _customerService;
        //private readonly IGenericAttributeService _genericAttributeService;
        private readonly IAddressService _addressService;
        private readonly ICountryService _countryService;
        //private readonly IStateProvinceService _stateProvinceService;
        //private readonly IShippingService _shippingService;
        private readonly IPaymentService _paymentService;
        private readonly IPluginFinder _pluginFinder;
        private readonly IOrderService _orderService;
        private readonly IWebHelper _webHelper;
        private readonly ILogger _logger;
        //private readonly IAddressAttributeParser _addressAttributeParser;
        //private readonly IAddressAttributeService _addressAttributeService;

        private readonly OrderSettings _orderSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        //private readonly PaymentSettings _paymentSettings;
        //private readonly ShippingSettings _shippingSettings;
        //private readonly AddressSettings _addressSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly PayExCheckoutSettings _payExCheckoutSettings;
        #endregion

        #region Ctor

        public PublicPayExCheckoutController(
            //ICheckoutModelFactory checkoutModelFactory,
            IWorkContext workContext,
            IStoreContext storeContext,
            IShoppingCartService shoppingCartService,
            //ILocalizationService localizationService,
            //IProductAttributeParser productAttributeParser,
            //IProductService productService,
            IOrderProcessingService orderProcessingService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ICustomerService customerService,
            //IGenericAttributeService genericAttributeService,
            IAddressService addressService,
            ICountryService countryService,
            //IStateProvinceService stateProvinceService,
            //IShippingService shippingService,
            IPaymentService paymentService,
            IPluginFinder pluginFinder,
            IOrderService orderService,
            IWebHelper webHelper,
            ILogger logger,
            //IAddressAttributeParser addressAttributeParser,
            //IAddressAttributeService addressAttributeService,
            OrderSettings orderSettings,
            RewardPointsSettings rewardPointsSettings,
            //PaymentSettings paymentSettings,
            //ShippingSettings shippingSettings,
            //AddressSettings addressSettings,
            CustomerSettings customerSettings,
            PayExCheckoutSettings payExCheckoutSettings)
        {
            //this._checkoutModelFactory = checkoutModelFactory;
            _workContext = workContext;
            _storeContext = storeContext;
            _shoppingCartService = shoppingCartService;
            //this._localizationService = localizationService;
            //this._productAttributeParser = productAttributeParser;
            //this._productService = productService;
            _orderProcessingService = orderProcessingService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _customerService = customerService;
            //this._genericAttributeService = genericAttributeService;
            _addressService = addressService;
            _countryService = countryService;
            //this._stateProvinceService = stateProvinceService;
            //this._shippingService = shippingService;
            this._paymentService = paymentService;
            this._pluginFinder = pluginFinder;
            _orderService = orderService;
            _webHelper = webHelper;
            _logger = logger;
            //this._addressAttributeParser = addressAttributeParser;
            //this._addressAttributeService = addressAttributeService;

            _orderSettings = orderSettings;
            _rewardPointsSettings = rewardPointsSettings;
            //this._paymentSettings = paymentSettings;
            //this._shippingSettings = shippingSettings;
            //this._addressSettings = addressSettings;
            _customerSettings = customerSettings;
            _payExCheckoutSettings = payExCheckoutSettings;
        }

        #endregion

        public IActionResult Redirect()
        {
            var currentPaymentMethod = _paymentService.LoadPaymentMethodBySystemName(PayExCheckoutPlugin.PaymentSystemName);
            var isEnabled = _paymentService.IsPaymentMethodActive(currentPaymentMethod);

            if (!isEnabled)
            {
                return Redirect(Url.RouteUrl("CheckoutOnePage"));
            }

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            return RedirectToRoute("PayExCheckout2.Checkin");
        }

        public async Task<IActionResult> Checkin()
        {            
            //validation
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
           
            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (!_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("Checkout");

            if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();


            CheckinModel model = null;

            using (var payexClient = new PayExClient.PayExClient(_payExCheckoutSettings.ApiEndPoint, _payExCheckoutSettings.MerchantToken))
            {
                var response = await payexClient.CheckinInitConsumerSessionAsync(
                                                    consumerCountryCode: "SE",
                                                    msisdn: null,
                                                    email: _workContext.CurrentCustomer.Email,
                                                    socialSecurityNumber: null);

                if (response != null)
                {
                    var operation = response.GetViewConsumerIdentificationOperation();

                    if (operation != null)
                    {
                        model = new CheckinModel()
                        {
                            ScriptHref = operation.Href,
                            Language = _payExCheckoutSettings.Language
                        };
                    }
                }

                return View("~/Plugins/Payments.PayExCheckout2/Views/PublicPayExCheckout/Checkin.cshtml", model);
            }
        }
        public async Task<IActionResult> Checkout(string consumerProfileRef)
        {            //validation
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            if (!cart.Any())
                return RedirectToRoute("ShoppingCart");

            if (!_orderSettings.OnePageCheckoutEnabled)
                return RedirectToRoute("Checkout");

            if (_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed)
                return Challenge();

            _orderTotalCalculationService.GetShoppingCartSubTotal(cart, true, out _, out _, out _, out decimal shoppingCartSubTotal);
            var taxtTotal = _orderTotalCalculationService.GetTaxTotal(cart);

            CheckoutModel model = null;

            using (var payexClient = new PayExClient.PayExClient(_payExCheckoutSettings.ApiEndPoint, _payExCheckoutSettings.MerchantToken))
            {
                var checkouModel = new CheckoutRequestModel(
                                        Guid.NewGuid(),
                                        (int)(shoppingCartSubTotal * 100),
                                        (int)(taxtTotal * 100),
                                        _workContext.WorkingCurrency.CurrencyCode,
                                        "ORDER",
                                        Request.Headers.ContainsKey(HeaderNames.UserAgent) ? Request.Headers[HeaderNames.UserAgent].ToString() : "NO_USER_AGENT",                                        
                                        _payExCheckoutSettings.Language,
                                        _payExCheckoutSettings.PayeeId,
                                        DateTime.UtcNow.ToString("yyMMddHHmmss"),
                                        consumerProfileRef);

                checkouModel.PaymentOrder.PayeeInfo.PayeeName = _payExCheckoutSettings.PayeeName;

                checkouModel.PaymentOrder.Urls.HostUrls = new List<string> { $"{Request.Scheme}://{Request.Host}" };
                checkouModel.PaymentOrder.Urls.CompleteUrl = _payExCheckoutSettings.Urls_completeUrl;
                checkouModel.PaymentOrder.Urls.CancelUrl = _payExCheckoutSettings.Urls_cancelUrl;
                checkouModel.PaymentOrder.Urls.CallbackUrl = _payExCheckoutSettings.Urls_callbackUrl;
                checkouModel.PaymentOrder.Urls.TermsOfServiceUrl = _payExCheckoutSettings.Urls_termsOfServiceUrl;

                if (_payExCheckoutSettings.CreditCard_SettingsEnabled)
                {
                    checkouModel.PaymentOrder.Items.Add(new CheckoutRequestModel.CheckoutRequest_PaymentOrder.PaymentOrder_Item
                    {
                        CreditCard = new CheckoutRequestModel.CheckoutRequest_PaymentOrder.PaymentOrder_Item.Item_CreditCard
                        {
                            No3DSecure = _payExCheckoutSettings.CreditCard_no3DSecure,
                            RejectAuthenticationStatusA = _payExCheckoutSettings.CreditCard_rejectAuthenticationStatusA,
                            RejectAuthenticationStatusU = _payExCheckoutSettings.CreditCard_rejectAuthenticationStatusU,
                            RejectCardNot3DSecureEnrolled = _payExCheckoutSettings.CreditCard_rejectCardNot3DSecureEnrolled,
                            RejectDebitCards = _payExCheckoutSettings.CreditCard_rejectDebitCards,
                            RejectCreditCards = _payExCheckoutSettings.CreditCard_rejectCreditCards,
                            RejectConsumerCards = _payExCheckoutSettings.CreditCard_rejectConsumerCards,
                            RejectCorporateCards = _payExCheckoutSettings.CreditCard_rejectCorporateCards
                        }
                    });
                }

                var response = await payexClient.CheckoutPaymentOrdersAsync(checkouModel);

                if (response != null)
                {
                    var operation = response.GetViewPaymentOrderOperation();

                    if (operation != null)
                    {
                        model = new CheckoutModel()
                        {
                            ScriptHref = operation.Href,
                            Language = _payExCheckoutSettings.Language,
                            PaymentOrderId = response.PaymentOrder.Id
                        };
                    }
                }

                return View("~/Plugins/Payments.PayExCheckout2/Views/PublicPayExCheckout/Checkout.cshtml", model);
            }
        }

        public async Task<IActionResult> OnPaymentCompleted(string paymentOrderId)
        {
            using (var payexClient = new PayExClient.PayExClient(_payExCheckoutSettings.ApiEndPoint, 
                                                                _payExCheckoutSettings.MerchantToken))
            {
                var response = await payexClient.GetPaymentOrderInfoAsync(paymentOrderId);

                if (response != null && response.CanCapture() && response.CanCancel())
                {
                    var currentCustomer = _workContext.CurrentCustomer;

                    #region Shipping and Billing Addresses                    

                    var payer = response.PaymentOrder.Payer;

                    if (payer == null)
                    {
                        _logger.Warning($"[PayEx Checkout] Payment order has no payer information [{paymentOrderId}]");
                        return NotFound();
                    }

                    if (payer.ShippingAddress == null)
                    {
                        _logger.Warning($"[PayEx Checkout] Payment order has no payer shipping address information [{paymentOrderId}]");
                        return NotFound();
                    }

                    var payerName = payer.ShippingAddress.Addressee.Split(new char[] { ' ' }, 2, StringSplitOptions.None);

                    var country = _countryService.GetCountryByTwoLetterIsoCode(payer.ShippingAddress.CountryCode);

                    Address shippingAddress = new Address()
                    {
                        FirstName = payerName[0],
                        LastName = payerName.Length > 1 ? payerName[1] : null,

                        Email = payer.Email,
                        PhoneNumber = payer.Msisdn,
                        Address1 = payer.ShippingAddress.StreetAddress,
                        Address2 = payer.ShippingAddress.CoAddress,
                        City = payer.ShippingAddress.City,
                        ZipPostalCode = payer.ShippingAddress.ZipCode,
                        CountryId = country?.Id
                    };

                     

                    var existingAddress =_addressService.FindAddress(currentCustomer.Addresses.ToList(),
                                                                    shippingAddress.FirstName, shippingAddress.LastName, 
                                                                    shippingAddress.PhoneNumber, shippingAddress.Email, 
                                                                    shippingAddress.FaxNumber, shippingAddress.Company, 
                                                                    shippingAddress.Address1, shippingAddress.Address2, 
                                                                    shippingAddress.City, shippingAddress.County,
                                                                    shippingAddress.StateProvinceId, shippingAddress.ZipPostalCode, 
                                                                    shippingAddress.CountryId, shippingAddress.CustomAttributes);

                    if (existingAddress == null)
                    {
                        shippingAddress.CreatedOnUtc = DateTime.UtcNow;
                        currentCustomer.Addresses.Add(shippingAddress);

                        existingAddress = shippingAddress;
                    }

                    currentCustomer.BillingAddress = existingAddress;
                    currentCustomer.ShippingAddress = existingAddress;
                    _customerService.UpdateCustomer(currentCustomer);
                    #endregion

                    var processPaymentRequest = new ProcessPaymentRequest
                    {
                        StoreId = _storeContext.CurrentStore.Id,
                        CustomerId = currentCustomer.Id,
                        PaymentMethodSystemName = PayExCheckoutPlugin.PaymentSystemName,
                        OrderGuid = new Guid(response.PaymentOrder.Metadata.OrderGuid)
                    };

                    processPaymentRequest.CustomValues.Add(PayExCheckoutPlugin.PaymentOrderIdCustomValueKey, paymentOrderId);

                    var placeOrderResult = _orderProcessingService.PlaceOrder(processPaymentRequest);

                    if (placeOrderResult.Success)
                    {
                        _orderProcessingService.Capture(placeOrderResult.PlacedOrder);

                        return Redirect(_payExCheckoutSettings.Urls_completeUrl + "#orderId:" + placeOrderResult.PlacedOrder.Id);
                    }
                }
            }

            return Redirect(_payExCheckoutSettings.Urls_cancelUrl + "#failed");
        }

        public async Task<IActionResult> Callback()
        {
            CallbackModel callbackModel = null;

            using (var fs = new System.IO.StreamReader(Request.Body))
            {
                string body = fs.ReadToEnd();

                callbackModel = JsonConvert.DeserializeObject<CallbackModel>(body);

                if (callbackModel?.Payment == null)
                {
                    _logger.Warning($"[PayEx Checkout] Callback model is null. Body: {body}");

                    return BadRequest();
                }
            }

            using (var payexClient = new PayExClient.PayExClient(_payExCheckoutSettings.ApiEndPoint,
                                                     _payExCheckoutSettings.MerchantToken))
            {
                var response = await payexClient.GetPaymentOrderInfoAsync(callbackModel.PaymentOrder.id);

                var orderGuid = response.PaymentOrder?.Metadata?.OrderGuid;

                if (orderGuid == null)
                {
                    _logger.Warning($"[PayEx Checkout] Order guid is absent in metadata of [{callbackModel.PaymentOrder.id}]");

                    return BadRequest();
                }

                var order = _orderService.GetOrderByGuid(new Guid(orderGuid));

                if (order == null)
                {
                    _logger.Warning($"[PayEx Checkout] Order with such guid {orderGuid} is absent. [{callbackModel.PaymentOrder.id}]");

                    return BadRequest();
                }

                var transaction = await payexClient.GetAsync<TransactionResponseModel>(callbackModel.Transaction.id);

                if (transaction == null)
                {
                    _logger.Warning($"[PayEx Checkout] Transaction not found. [{callbackModel.Transaction.id}]");

                    return BadRequest();
                }

                var transactionOperation = transaction.authorization ?? transaction.capture ?? transaction.cancellation ?? transaction.reversal;

                if (transactionOperation?.transaction == null)
                {
                    _logger.Warning($"[PayEx Checkout] Transaction operation not found. [{callbackModel.Transaction.id}]");

                    return BadRequest();
                }

                if (transactionOperation.transaction.state == "Completed")
                {
                    switch (transactionOperation.transaction.type)
                    {
                        case "Authorization":
                            if (_orderProcessingService.CanMarkOrderAsAuthorized(order))
                            {
                                _orderProcessingService.MarkAsAuthorized(order);
                            }
                            break;
                        case "Capture":
                            if (_orderProcessingService.CanMarkOrderAsPaid(order))
                            {
                                _orderProcessingService.MarkOrderAsPaid(order);
                            }
                            break;
                        case "Cancellation":
                            if (_orderProcessingService.CanVoidOffline(order))
                            {
                                _orderProcessingService.VoidOffline(order);
                            }
                            break;

                        case "Reversal":
                            decimal refundAmount = (transactionOperation.transaction.amount / 100m);

                            if (_orderProcessingService.CanPartiallyRefundOffline(order, refundAmount))
                            {
                                _orderProcessingService.PartiallyRefundOffline(order, refundAmount);
                            }
                            break;
                        default:
                            _logger.Warning($"[PayEx Checkout] Not imlpemented transaction type {transactionOperation.transaction.type}. [{callbackModel.Transaction.id}]");
                            return BadRequest();
                    }
                }

                order.OrderNotes.Add(new OrderNote
                {
                    Note = $"[PayEx Checkout] Transaction {transactionOperation.transaction.type} #{transactionOperation.transaction.number} is {transactionOperation.transaction.state} ({transactionOperation.transaction.description})",
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });

                _orderService.UpdateOrder(order);
            }

            return Content("Callback has been logged.");
        }
    }
}
