# NopCommerce Elavon Lightbox Payment Plugin

Custom nopCommerce payment plugin for **Elavon's Hosted Payment Lightbox** integration.

This repository contains a custom payment plugin for [nopCommerce](https://www.nopcommerce.com/) that integrates [Elavon's Hosted Payments](https://developer.elavon.com/products/en-uk/elavon-payment-gateway/v1/hosted-payments-overview) page using a secure lightbox modal.

The plugin source code is located at:

```
_source/src/Plugins/Nop.Plugin.Payments.Elavon/
```

> **Note:** The full nopCommerce source code is included in this repository to make local development, testing, and debugging of the plugin easier.

## Features

* Seamless integration with Elavon's Hosted Payments Page via a secure lightbox modal.
* Secure payment processing: payment details are handled directly by Elavon.
* Admin configuration page for managing Elavon credentials and settings.
* Supports nopCommerce **4.80+** (targeting **.NET 9.0**).

## Installation

### Quick Install (From Release)

1. Download the plugin archive from the [Releases page](https://github.com/Bellosoft-Limited/NopCommerce-Elavon-Lightbox-Payment-Plugin/releases) (e.g., `Nop.Plugin.Payments.Elavon-vx.xx.xx.zip`).
2. In the nopCommerce admin area, go to **Configuration > Local plugins**.
3. Click **Upload plugin or theme** and select the downloaded ZIP file.
4. Click **Reload list of plugins**.
5. Find **Elavon** and click **Install**.

**Alternatively:**

1. Download and extract the plugin ZIP file from the Releases page.
2. Copy the extracted folder to:

```
Presentation/Nop.Web/Plugins/
```

3. Restart your application.
4. In the admin panel, go to **Configuration > Local plugins** and install **Elavon**.

> **Important:**
> The plugin package from Releases is for **installation only**.
> To **modify or develop** the plugin, you must clone the repository and work within the `_source/src/Plugins/` directory (see the "Build from Source" section below).

## Build from Source

1. **Clone the repository:**

```console
git clone https://github.com/Bellosoft-Limited/NopCommerce-Elavon-Lightbox-Payment-Plugin.git
```

2. **Open the solution and build:**
Open the solution in **Visual Studio** or build from the terminal:

```console
dotnet build
```

The build process compiles the plugin and copies the output to:

```
Presentation/Nop.Web/Plugins/Payments.Elavon/
```

3. **Install in nopCommerce:**

* Run the `Nop.Web` project.
* Log in to the **Admin Area > Configuration > Local plugins**.
* Find **Elavon** and click **Install**.

### Development Notes

* **Develop in Source:**
Make all code changes under:

```
_source/src/Plugins/Nop.Plugin.Payments.Elavon/
```

* **Do not edit** the compiled files in:

```
Presentation/Nop.Web/Plugins/Payments.Elavon/
```

> This directory is build output and will be **overwritten** during compilation. The included nopCommerce source is provided for **local debugging and integration testing**.

## Configuration

1. In the nopCommerce admin area, go to **Configuration > Payment methods**.
2. Click **Configure** next to **Elavon**.
3. Enter your Elavon credentials and adjust any desired settings.
4. Click **Save** to apply the changes.

## Usage

Once installed and configured, the plugin becomes available during checkout.

### Customer Checkout Experience

* During checkout, if multiple payment methods are available, customers can select **Elavon**. 
  * If it's the only available payment method, they are automatically directed to the next step.
* The customer is taken to an order summary page showing their cart details and a payment button.
* When the customer clicks to pay, the Elavon lightbox modal opens.
* The customer completes the payment securely within the Elavon-hosted modal.

Once the payment is successfully authorized, the plugin:

* Creates the corresponding order in nopCommerce.
* Displays a "Transaction Approved" message within the modal flow.
* The customer can then click Continue to proceed to the standard nopCommerce Order Confirmation page.

### Security & Compliance

* All sensitive payment data is handled exclusively by **Elavon**'s PCI-compliant systems.
* Your nopCommerce site never processes or stores card information, ensuring [PCI DSS compliance](https://www.pcisecuritystandards.org/standards/).

### Order Handling

* Successful transactions automatically update the nopCommerce order status to Paid when the Elavon API reports the transaction as SETTLED.
* Orders with a PaymentStatus of Authorized are periodically synchronized via a background task.

## Compatibility

* nopCommerce: 4.80+
* .NET: 9.0+

## License

Licensed under the [GNU Affero General Public License v3.0](LICENSE).