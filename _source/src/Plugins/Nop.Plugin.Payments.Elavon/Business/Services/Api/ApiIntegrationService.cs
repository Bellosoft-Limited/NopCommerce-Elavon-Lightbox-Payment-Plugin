// <copyright file="ApiIntegrationService.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.Elavon.Models.DTO;
using Nop.Services.Directory;
using Nop.Services.Orders;

namespace Nop.Plugin.Payments.Elavon.Business.Services.Api;

public class ApiIntegrationService : IApiIntegrationService
{
    private readonly IWebHelper _webHelper;
    private readonly IWorkContext _workContext;
    private readonly IStoreContext _storeContext;
    private readonly ICurrencyService _currencyService;
    private readonly IShoppingCartService _shoppingCartService;
    private readonly IOrderTotalCalculationService _orderTotalCalculationService;
    private readonly CurrencySettings _currencySettings;
    private readonly ElavonPaymentSettings _elavonPaymentSettings;
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiIntegrationService> _logger;

    public ApiIntegrationService(
        IWebHelper webHelper,
        IWorkContext workContext,
        IStoreContext storeContext,
        ICurrencyService currencyService,
        IShoppingCartService shoppingCartService,
        IOrderTotalCalculationService orderTotalCalculationService,
        CurrencySettings currencySettings,
        ElavonPaymentSettings elavonPaymentSettings, 
        HttpClient httpClient,
        ILogger<ApiIntegrationService> logger)
    {
        _webHelper = webHelper;
        _workContext = workContext;
        _storeContext = storeContext;
        _currencyService = currencyService;
        _shoppingCartService = shoppingCartService;
        _orderTotalCalculationService = orderTotalCalculationService;
        _currencySettings = currencySettings;
        _elavonPaymentSettings = elavonPaymentSettings;
        _httpClient = httpClient;
        _logger = logger;

        var elavonAuthToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_elavonPaymentSettings.MerchantAlias}:{_elavonPaymentSettings.SecretKey}"));
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {elavonAuthToken}");
    }

    /// <summary>
    /// Create payment session with Elavon
    /// </summary>
    public async Task<PaymentSessionResponse> CreatePaymentSessionAsync()
    {
        var apiUrl = $"{_elavonPaymentSettings.ApiUrl.TrimEnd('/')}/payment-sessions";
        var originUrl = _webHelper.GetStoreLocation(true);
        
        var elavonOrder = await CreateElavonOrderAsync();
        var requestData = new PaymentSessionRequest(elavonOrder.Href, originUrl);

        return await PostAsync<PaymentSessionRequest, PaymentSessionResponse>(apiUrl, requestData);
    }

    public async Task<OrderResponse> GetElavonOrderBySessionIdAsync(string sessionId)
    {
        var apiUrl = $"{_elavonPaymentSettings.ApiUrl.TrimEnd('/')}/payment-sessions/{sessionId}";
        var paymentSession = await GetAsync<PaymentSessionResponse>(apiUrl);

        return await GetAsync<OrderResponse>(paymentSession.OrderHref);
    }

    /// <summary>
    /// Verify transaction with Elavon API
    /// </summary>
    public async Task<bool> VerifyTransactionApprovalAsync(string sessionId)
    {
        try
        {
            var apiUrl = $"{_elavonPaymentSettings.ApiUrl.TrimEnd('/')}/payment-sessions/{sessionId}";
            var paymentSession = await GetAsync<PaymentSessionResponse>(apiUrl);

            if (string.IsNullOrEmpty(paymentSession.TransactionHref))
                return false;

            var transaction = await GetAsync<TransactionResponse>(paymentSession.TransactionHref);

            return transaction.IsAuthorized;
        }
        catch (Exception)
        {
            return false;
        }
    }

    #region Private Methods

    private async Task<OrderResponse> CreateElavonOrderAsync()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();
        var cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);

        var cartTotal = (await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart)).shoppingCartTotal ?? 0;
        var currency = await _currencyService.GetCurrencyByIdAsync(customer.CurrencyId ?? _currencySettings.PrimaryStoreCurrencyId);

        var apiUrl = $"{_elavonPaymentSettings.ApiUrl.TrimEnd('/')}/orders";
        var requestData = new OrderRequest
        {
            Total = new OrderTotal
            {
                Amount = cartTotal,
                CurrencyCode = currency.CurrencyCode
            }
        };

        return await PostAsync<OrderRequest, OrderResponse>(apiUrl, requestData);
    }

    private async Task<TResponse> GetAsync<TResponse>(string url)
    {
        var response = await _httpClient.GetAsync(url);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("ApiIntegrationService - GetAsync : GET {Url} Failed. {Status} - {Response}", url, response.StatusCode, responseContent);
            throw new NopException();
        }

        return JsonConvert.DeserializeObject<TResponse>(responseContent);
    }

    private async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest requestData)
    {
        var json = JsonConvert.SerializeObject(requestData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("ApiIntegrationService - PostAsync : POST {Url} Failed. {Status} - {Response}", url, response.StatusCode, responseContent);
            throw new NopException();
        }

        return JsonConvert.DeserializeObject<TResponse>(responseContent);
    }

    #endregion Private Methods
}
