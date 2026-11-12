using FluentAssertions;
using Nop.Plugin.Payments.Elavon.Business;
using Nop.Plugin.Payments.Elavon.Controllers;
using NUnit.Framework;

namespace Nop.Plugin.Payments.Elavon.Tests;

/// <summary>
/// The Moran's plugin (MoransCommerceGit, DepositDefaults) and the LogicCloud plugin (Constants.AMOUNT_CHARGED_CUSTOM_VALUE) cannot
/// reference this plugin, so they use these names as plain text: changing one here breaks deposits on the Moran's store until they are changed to match
/// </summary>
[TestFixture]
public class MoransContractTests
{
    [Test]
    public void MoransContract_ShouldKeepNamesTheMoransPluginUses()
    {
        Globals.SystemName.Should().Be("Payments.Elavon");
        Globals.AmountToChargeAttribute.Should().Be("Payments.AmountToCharge");
        Globals.AmountChargedCustomValue.Should().Be("Amount charged");

        //MVC routes these as "PaymentElavonPublic" and "CreatePaymentSession": the Controller and Async suffixes are dropped
        nameof(PaymentElavonPublicController).Should().Be("PaymentElavonPublicController");
        nameof(PaymentElavonPublicController.CreatePaymentSessionAsync).Should().Be("CreatePaymentSessionAsync");
    }
}
