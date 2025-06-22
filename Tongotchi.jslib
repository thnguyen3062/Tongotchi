/*mergeInto(LibraryManager.library, 
{
    sendInvoice: function(title, description, payload, providerToken, currency, pricesJson, successCallback, errorCallback) {
        var titleStr = UTF8ToString(title);
        var descriptionStr = UTF8ToString(description);
        var payloadStr = UTF8ToString(payload);
        var providerTokenStr = UTF8ToString(providerToken);
        var currencyStr = UTF8ToString(currency);
        var pricesStr = UTF8ToString(pricesJson);
        var prices = JSON.parse(pricesStr);
        var successCallbackPtr = successCallback;
        var errorCallbackPtr = errorCallback;

        var xhr = new XMLHttpRequest();
        xhr.open("POST", "https://api.telegram.org/bot7217117668:AAE8il3eSgqPX0-2xVbYHX0yBX6K5rL3uMY/sendInvoice", true);
        xhr.setRequestHeader("Content-Type", "application/json;charset=UTF-8");

        xhr.onreadystatechange = function() {
            if (xhr.readyState === 4) {
                if (xhr.status === 200) {
                    var response = JSON.parse(xhr.responseText);
                    if (response.ok) {
                        var responseStr = JSON.stringify(response.result);
                        // Send success message to Unity
                        unityInstanceRef.SendMessage('Payment', 'OnInvoiceSent', responseStr);
                    } else {
                        // Send error message to Unity
                        unityInstanceRef.SendMessage('Payment', 'OnInvoiceError', response.description);
                    }
                } else {
                    // Send error message to Unity
                    unityInstanceRef.SendMessage('Payment', 'OnInvoiceError', xhr.statusText);
                }
            }
        };

        var data = {
            chat_id: "563356286", // Replace with the chat ID where the invoice should be sent
            title: titleStr,
            description: descriptionStr,
            payload: payloadStr,
            provider_token: providerTokenStr,
            start_parameter: "start",
            currency: currencyStr,
            prices: prices
        };

        console.log("Sending invoice data: ", JSON.stringify(data));
        xhr.send(JSON.stringify(data));
    }
})*/