// <copyright file="Globals.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

namespace Nop.Plugin.Payments.Elavon.Business;

public class Globals
{
    public static string SystemName => "Payments.Elavon";

    #region Session Keys

    public static string PaymentRequestSessionKey => "OrderPaymentInfo";

    public static string PaymentSessionId => "SessionId";

    #endregion Session Keys

    #region Deposits

    /// <summary>
    /// Customer attribute, saved per store by the Moran's plugin before the payment session is created, holding the amount to
    /// charge instead of the order total (a deposit); no attribute means the order total
    /// </summary>
    public static string AmountToChargeAttribute => "Payments.AmountToCharge";

    /// <summary>
    /// Order custom value holding the amount Elavon was asked to charge, written invariantly (e.g. "50.00")
    /// </summary>
    public static string AmountChargedCustomValue => "Amount charged";

    #endregion Deposits
}
