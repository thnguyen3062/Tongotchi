using Core.Utils;
using Game;
using Game.Websocket.Model;
using PathologicalGames;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupConfirmPurchase : BaseView
{
    [SerializeField] private TextMeshProUGUI m_Desc;
    // Start is called before the first frame update
    private ICallback.CallFunc8<int, bool, int, int> onConfirmBuyItem;
    private ICallback.CallFunc2<bool> onConfirmBuySlot;
    private ICallback.CallFunc8<bool, CurrencyType, ExchangeInfo, int> onConfirmExchange;
    private PurchaseType purchaseType;
    private ItemData data;
    private PVPShopItemData dataItemPvp;
    ExchangeInfo exchangeInfo;
    private bool canBuy = false;
    private int count;
    public PopupConfirmPurchase SetOnConfirmPurchaseItemCallback(ICallback.CallFunc8<int, bool, int, int> func) { onConfirmBuyItem = func; return this; }
    public PopupConfirmPurchase SetOnConfirmPurchaseSlotCallback(ICallback.CallFunc2<bool> func) { onConfirmBuySlot = func; return this; }
    public PopupConfirmPurchase SetOnConfirmExchangeCallback(ICallback.CallFunc8<bool, CurrencyType, ExchangeInfo, int> func) { onConfirmExchange = func; return this; }

    public void InitData(PurchaseType type, int itemID, int count = 1)
    {
        transform.localPosition = Vector3.zero;
        purchaseType = type;
        this.count = count;
        data = PlayerData.Instance.GetItemData(itemID);
        string nameItem = data.itemName;
        string currency = "";
        if (data.currencyType == CurrencyType.Ticket)
            currency = "<sprite=0>";
        else if (data.currencyType == CurrencyType.Diamond)
            currency = "<sprite=2>";
        else
            currency = "<sprite=1>";
        m_Desc.text = "Do you want to buy " + nameItem + " for " + currency + (data.price * count) + "?";
    }

    public void InitDataPurchaseSlotPet(PurchaseType type, CurrencyType currencyType, int price)
    {
        transform.localPosition = Vector3.zero;
        purchaseType = type;
        data = new ItemData() { id = -1, currencyType = currencyType, price = price };
        string currency = "";
        if (data.currencyType == CurrencyType.Ticket)
            currency = "<sprite=0>";
        else if (data.currencyType == CurrencyType.Diamond)
            currency = "<sprite=2>";
        else
            currency = "<sprite=1>";
        m_Desc.text = "Do you want to buy Pet Slot for " + currency + data.price.ToString() + "?";
    }

    public void InitDataExchange(PurchaseType type, ExchangeType exchangeType, float quantityLost, float quantityGot)
    {
        transform.localPosition = Vector3.zero;
        purchaseType = type;
        string item1 = "";
        string item2 = "";
        if (exchangeType == ExchangeType.Ticket)
        {
            exchangeInfo = new ExchangeInfo() { currencyExchange = CurrencyType.Diamond, quantityLost = quantityLost, currencyReceive = CurrencyType.Ticket, quantityGot = quantityGot };
            item1 = "<sprite=2>";
            item2 = "<sprite=0>";
            m_Desc.text = "Do you want to convert " + item1 + quantityLost + " to " + item2 + quantityGot + "?";
        }
        else
        {
            exchangeInfo = new ExchangeInfo() { currencyExchange = CurrencyType.Token, quantityLost = quantityLost, currencyReceive = CurrencyType.Diamond, quantityGot = quantityGot };
            item1 = "<sprite=1>";
            item2 = "<sprite=2>";
            m_Desc.text = "Do you want to buy " + item2 + quantityGot + " with " + item1 + quantityLost + "?";
        }
    }

    public void OnConfirm()
    {
        SoundManager.Instance.PlayVFX("11. Buy Item");
        switch (purchaseType)
        {
            case PurchaseType.Item:

                if (data.id == GameUtils.EXP_POTION_BOOST && GameManager.Instance.PetController.PetNeedToEvolve)
                {
                    ShowUIView<PopupNotify>().Init("Pet is in evolve progress", "Please evolve your pet before buying EXP potion", 0, false, false);
                    return;
                }

                GetCurrency(data.currencyType, (GetCurrencyResponse response) => {
                    if (response.success)
                    {
                        canBuy = response.GetCurrencyValue(data.currencyType) >= (data.price * count) ? true : false;
                        if (canBuy)
                        {
                            SoundManager.Instance.PlayVFX("12. Use Buff");

                            if (data.id == GameUtils.ROBOT_BOOST || data.id == GameUtils.TICKET_POTION_BOOST || data.id == GameUtils.EXP_POTION_BOOST)
                            {
                                bool petIsDead = PlayerData.Instance.PetData.isDead;
                                if (!petIsDead)
                                {
                                    PlayerData.Instance.AddBoost(data.id, () => {
                                        if (NeedToReloadBoosts(data.id))
                                        {
                                            GameManager.Instance.BoostHandler.ReloadBoosts();
                                        }
                                        onConfirmBuyItem?.Invoke(data.id, canBuy, count, 1);
                                        if (data.id == GameUtils.EXP_POTION_BOOST) GameManager.Instance.PetController.CheckPetStatus(false);
                                    });//, 3 * 24 * 3600);
                                }
                                else
                                {
                                    ShowUIView<PopupNotify>().Init("PET IS DEAD", $"Cannot buy boost(s) when current pet is dead", 0, false, false);
                                }
                            }
                            else
                            {
                                PlayerData.Instance.RequestBuyItem(data.id, count, (BuyItemResponse response) => {
                                    Debug.Log($"Buy item {data.id} successfully!");
                                    onConfirmBuyItem?.Invoke(data.id, canBuy, count, response.newQuantity);
                                });
                            }
                        }
                        else
                        {
                            Debug.Log($"Cannot buy item {data.id}");
                            onConfirmBuyItem?.Invoke(data.id, canBuy, count, 0);
                        }
                        Hide();
                    }
                });
                //canBuy = PlayerData.Instance.data.currencyDict[data.currencyType] >= (data.price * count) ? true : false;
                break;
            case PurchaseType.Slot:
                GetCurrency(data.currencyType, (GetCurrencyResponse response) => {
                    canBuy = response.GetCurrencyValue(data.currencyType) >= data.price ? true : false;
                    if (canBuy)
                    {
                        PlayerData.Instance.AddNewPetSlot((success, newPet) =>
                        {
                            if (!success)
                                return;
                            onConfirmBuySlot?.Invoke(canBuy);
                            PlayerData.Instance.AddCurrency(data.currencyType, -(int)data.price);
                            GetUIView<UISelectPetPopup>().AddNewUIPetSlot(newPet);
                            //UISelectPetPopup.instance.OnAddNewPetSlot();
                        });
                    }
                    else
                    {
                        onConfirmBuySlot?.Invoke(canBuy);
                    }
                    Hide();
                });
                
                break;
            case PurchaseType.Exchange:
                if (exchangeInfo.currencyExchange != CurrencyType.Token)
                {
                    GetCurrency(exchangeInfo.currencyExchange, (GetCurrencyResponse response) => {
                        float changeAmount = response.GetCurrencyValue(exchangeInfo.currencyExchange) - exchangeInfo.quantityLost;//PlayerData.Instance.data.currencyDict[exchangeInfo.currencyExchange] - exchangeInfo.quantityLost;
                        canBuy = response.GetCurrencyValue(exchangeInfo.currencyExchange) >= exchangeInfo.quantityLost;
                        if (canBuy)
                        {
                            PlayerData.Instance.AddCurrency(exchangeInfo.currencyReceive, (int)exchangeInfo.quantityGot);
                            PlayerData.Instance.AddCurrency(exchangeInfo.currencyExchange, -(int)exchangeInfo.quantityLost);
                        }
                        else
                        {
                            Debug.Log("Cannot buy");
                        }
                        onConfirmExchange?.Invoke(canBuy, exchangeInfo.currencyExchange, exchangeInfo, Mathf.RoundToInt(changeAmount));
                        Hide();
                    });
                }
                else
                {
                    onConfirmExchange?.Invoke(canBuy, exchangeInfo.currencyExchange, exchangeInfo, Mathf.RoundToInt(0));
                }
                break;
        }
        //PoolManager.Pools["Popup"].Despawn(this.transform);
    }

    private bool NeedToReloadBoosts(int itemId)
    {
        return itemId == GameUtils.TICKET_POTION_BOOST || itemId == GameUtils.ROBOT_BOOST;
    }

    public void OnCancel()
    {
        SoundManager.Instance.PlayVFX("20. Cancel");
        Hide();
        //PoolManager.Pools["Popup"].Despawn(this.transform);
    }

    private void GetCurrency(CurrencyType currency, Action<GetCurrencyResponse> onCompleted)
    {
        PlayerData.Instance.GetCurrency(currency, onCompleted);
    }
}

