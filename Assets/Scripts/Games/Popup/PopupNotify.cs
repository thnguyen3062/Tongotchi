using Core.Utils;
using Game;
using Game.Websocket;
using Game.Websocket.Commands.Pvp;
using Game.Websocket.Commands.Storage;
using Game.Websocket.Model;
using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupNotify : BaseView
{
    [SerializeField] private TextMeshProUGUI m_TitleText;
    [SerializeField] private TextMeshProUGUI m_ContentText;
    [SerializeField] private Button m_ShopBtn;
    [SerializeField] private Button m_BuyMoreBtn;
    [SerializeField] private Button m_CloseBtn;
    [SerializeField] private Image m_InfoImage;
    [SerializeField] private TextMeshProUGUI m_TmpBuyMore;
    private ICallback.CallFunc4<bool, int, int> onConfirmBuyMinigame;
    private ICallback.CallFunc2<bool> onConfirmBuyPvpItem;
    private int type = 0;
    private int missingAmount;
    public PopupNotify SetOnConfirmBuyMinigame(ICallback.CallFunc4<bool, int, int> func) { onConfirmBuyMinigame = func; return this; }
    public PopupNotify SetOnConfirmBuyItemPvp(ICallback.CallFunc2<bool> func) { onConfirmBuyPvpItem = func; return this; }

    protected virtual void OnEnable()
    {
        m_ShopBtn.onClick.AddListener(OnShop);
        m_BuyMoreBtn.onClick.AddListener(OnBuyMore);
        m_CloseBtn.onClick.AddListener(Close);
    }

    protected virtual void OnDisable()
    {
        m_ShopBtn.onClick.RemoveListener(OnShop);
        m_BuyMoreBtn.onClick.RemoveListener(OnBuyMore);
        m_CloseBtn.onClick.RemoveListener(Close);
    }

    public void Init(string title, string content, int missingAmount, bool goToShop, bool canBuyMore = true, int type = 0)
    {
        // type de chia case. tam thoi mini game = 0, pvp item =1
        this.type = type;
        this.missingAmount = missingAmount;
        m_ShopBtn.gameObject.SetActive(goToShop);
        m_TitleText.text = title;
        m_ContentText.text = content;
        m_BuyMoreBtn.gameObject.SetActive(canBuyMore);
        m_InfoImage.raycastTarget = canBuyMore;
        transform.localPosition = Vector3.zero;
    }

    private void OnShop()
    {
        ShowUIView<ShopManager>().ShowTab(ShopButtonTab.Currency);
        Close();
    }

    private void OnBuyMore()
    {
        if (type == 0)
        {
            PlayerData.Instance.GetCurrency(CurrencyType.Diamond, (GetCurrencyResponse response) => {
                if (response.success)
                {
                    if (response.diamond >= GameUtils.MINIGAME_TICKET_PRICE)
                    {
                        OnPurchaseCompleted();
                    }
                    else
                    {
                        UIManager.Instance.OpenBuyStar(GameUtils.MINIGAME_TICKET_PRICE, UIManager.Instance.diamondValueDict[GameUtils.MINIGAME_TICKET_PRICE], (success) =>
                        {
                            if (success)
                                OnPurchaseCompleted();
                        });
                    }
                }
            });
        }
        else if (type == 1)
        {
            PlayerData.Instance.GetCurrency(CurrencyType.Ticket ,(GetCurrencyResponse response) => { 
                if (response != null && response.success)
                {
                    if (response.tickets >= missingAmount)
                    {
                        OnPurchaseCompleted();
                    }
                    else
                    {
                        UIManager.Instance.OpenBuyStar(GameUtils.ITEM_PVP_PRICE, UIManager.Instance.diamondValueDict[GameUtils.ITEM_PVP_PRICE], (success) =>
                        {
                            if (success)
                                OnPurchaseCompleted();
                        });
                    }
                }
            });
        }
        else
        {
            PlayerData.Instance.GetCurrency(CurrencyType.Diamond, (GetCurrencyResponse response) => {
                if (response.diamond >= GameUtils.ATTACK_COUNT_PRICE)
                {
                    OnPurchaseCompleted();
                }
                else
                {
                    UIManager.Instance.OpenBuyStar(GameUtils.ATTACK_COUNT_PRICE, UIManager.Instance.diamondValueDict[GameUtils.ATTACK_COUNT_PRICE], (success) =>
                    {
                        if (success)
                            OnPurchaseCompleted();
                    });
                }
            });

        }

        Close();
    }

    private void OnPurchaseCompleted()
    {
        if (type == 0)
        {
            WebSocketRequestHelper.RequestResetMinigameTicket(PlayerData.Instance.userInfo.telegramCode, (PurchaseResponse response) => {
                onConfirmBuyMinigame?.Invoke(true, response.RemainingPlays, response.MaxPurchasesPerDay);
            });
        }
        else if (type == 1)
        {
            PlayerData.Instance.AddCurrency(CurrencyType.Ticket, missingAmount, (TicketChangeResponse response) => { 
                if (response.success)
                {
                    onConfirmBuyPvpItem?.Invoke(true);
                }
            });
        }
        else
        {
            WebSocketRequestHelper.ResetAttackCountOnce((PVPProfileResponse response) =>
            {
                if (response.today_reset_attack)
                {
                    PlayerData.Instance.SetAttackCount(response.today_attack_count);
                    LoggerUtil.Logging("PURCHASE_COMPLETED_PVP", "Result=Success");
                    PlayerData.Instance.AddCurrency(CurrencyType.Diamond, -GameUtils.ITEM_PVP_PRICE);
                }
                else
                {
                    LoggerUtil.Logging("PURCHASE_COMPLETED_PVP", "Result=Failed");
                }
            });
        }
    }

    private void Close()
    {
        Hide();
    }
}
