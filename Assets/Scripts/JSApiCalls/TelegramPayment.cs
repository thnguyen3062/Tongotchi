using Game.Websocket;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;

public class TelegramPayment : MonoBehaviour
{
    public static TelegramPayment instance;

    [SerializeField] private TextMeshProUGUI m_DebugField;
    private int currentAmount;
    private string transactionCode;
    private Action<bool> onCompleted;

#if !UNITY_ANDROID
    [DllImport("__Internal")]
    private static extern void PurchaseWithStar(string link);
#endif

    public void OnPurchaseButtonClicked(string link)
    {
        Debug.Log("Payment Purchase: " + link);
#if !UNITY_ANDROID
        PurchaseWithStar(link);
#endif
    }

    private void Awake()
    {
        instance = this;
    }

    public void BuyStar(int amount, int price, Action<bool> onCompleted)
    {
        //UIManager.instance.ShowFadeScreen();
        if (GameUtils.userNames.Contains(PlayerData.Instance.userInfo.telegramCode))
            price = 1;
        WebSocketRequestHelper.CreateInvoiceLinkOnce(price, (transactionCode, link) =>
        {
            OnLinkCreated(transactionCode, link, onCompleted);
        });
        currentAmount = amount;
    }

    private void OnLinkCreated(string transactionCode, string link, Action<bool> onCompleted)
    {
        this.transactionCode = transactionCode;
        Debug.Log("Payment; " + link);
        OnPurchaseButtonClicked(link);
        this.onCompleted = onCompleted;
        //UIManager.instance.HideFadeScreen();
    }

    public void PaymentReceived(string data)
    {
        StartCoroutine(CheckInvoiceRoutine(data, 0));
    }

    IEnumerator CheckInvoiceRoutine(string data, int count)
    {
        yield return new WaitForSeconds(1);

        WebSocketRequestHelper.CheckInvoiceStatusOnce(transactionCode, (success) =>
        {
            CheckInvoiceCompleted(data, count, success);
        });
    }

    private void CheckInvoiceCompleted(string data, int count, bool success)
    {
        if (success)
        {
            HandleInvoiceSuccess();
            return;
        }

        if (count < 3)
        {
            StartCoroutine(CheckInvoiceRoutine(data, count + 1));
        }
        else
        {
            HandleInvoiceFailure();
        }
    }

    private void HandleInvoiceSuccess()
    {
        FirebaseAnalytics.instance.LogCustomEvent("purchase_star_success", JsonConvert.SerializeObject(new CustomEventWithVariable(currentAmount.ToString())));
        PlayerData.Instance.AddCurrency(CurrencyType.Diamond, currentAmount);
        onCompleted?.Invoke(true);
    }

    private void HandleInvoiceFailure()
    {
        FirebaseAnalytics.instance.LogCustomEvent("purchase_star_failed", JsonConvert.SerializeObject(new CustomEventWithVariable(currentAmount.ToString())));
        onCompleted?.Invoke(false);
    }
}