// <copyright file="Globals.cs" company="Bellosoft Limited">
// Copyright (c) Bellosoft Limited. All rights reserved.
// </copyright>

namespace Nop.Plugin.Payments.Elavon.Business;

public class Globals
{
    #region Session Keys

    public static string PaymentRequestSessionKey => "OrderPaymentInfo";

    public static string PaymentSessionId => "SessionId";
    
    #endregion Session Keys
}
