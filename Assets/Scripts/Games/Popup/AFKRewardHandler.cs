using Game.Websocket;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AFKRewardHandler : MonoBehaviour
{
    [SerializeField] private GameObject m_RewardCount;
    [SerializeField] private TextMeshProUGUI m_Timer;
    [SerializeField] private TextMeshProUGUI m_Count;
    [SerializeField] private Button m_ClaimBtn;
    [SerializeField] private Button m_StartFramBtn;

    private int currentTickets;
    private float farmingTimeElapsed;
    private bool query;

    private void OnEnable()
    {
        m_ClaimBtn.onClick.AddListener(OnClaim);
        m_StartFramBtn.onClick.AddListener(StartFarm);

        // When the UI is opened, fetch the remain time to farm and farmed tickets.
        InitPopup();
    }

    private void OnDisable()
    {
        m_ClaimBtn.onClick.RemoveListener(OnClaim);
        m_StartFramBtn.onClick.RemoveListener(StartFarm);
    }

    private void Update()
    {
        if (PlayerData.Instance.data.isFarming && !query)
        {
            UpdateTimerText();
        }
    }

    private void InitPopup()
    {
        m_Timer.text = "08 : 00 : 00";
        if (PlayerData.Instance.data.isFarming)
        {
            query = true;
            WebSocketRequestHelper.RequestAFKFarm(PlayerData.Instance.userInfo.telegramCode, (AFKRewardResponse response) => {
                switch (response.type)
                {
                    case "farming":
                        PlayerData.Instance.data.isFarming = !response.success;
                        //PlayerData.Instance.data.isFarming = isFarming;
                        farmingTimeElapsed = (response.farmTime < 0 ? -1 : 1) * response.farmTime * 60 * 60;
                        break;
                    case "claim":
                        farmingTimeElapsed = 0;
                        PlayerData.Instance.data.isFarming = false;
                        break;
                }
                query = false;
                SetButtonState();
                UpdateTickets();
            });
        }
        else
        {
            SetButtonState();
        }

        /*
    DateTime currentTime;
    WebSocketRequestHelper.GetTimeOnce((time) =>
    {
        currentTime = GameUtils.ParseTime(time);
        DateTime targetTime = GameUtils.ParseTime(PlayerData.Instance.data.targetFarmTime);
        TimeSpan timeSpan = targetTime - currentTime;
        farmingTimeElapsed = (float)timeSpan.TotalSeconds;
        isFarming = true;
        SetButtonState();
        UpdateTickets();
    });
        */
    }

    private void SetButtonState()
    {
        m_StartFramBtn.gameObject.SetActive(!PlayerData.Instance.data.isFarming);
    }

    private int CalculateTickets()
    {
        int currentTickets = GameUtils.MAX_AFK_REWARD - (int)(farmingTimeElapsed / (GameUtils.MAX_AFK_HOURS * 3600) * GameUtils.MAX_AFK_REWARD);
        return currentTickets;
    }

    private void UpdateTimerText()
    {
        if (farmingTimeElapsed <= 0)
        {
            LoggerUtil.Logging("End farm", "");
            if (!m_ClaimBtn.gameObject.activeSelf)
                m_ClaimBtn.gameObject.SetActive(true);
            if (m_RewardCount.activeSelf)
                m_RewardCount.SetActive(false);

            m_Timer.text = "00 : 00 : 00";
            return;
        }

        farmingTimeElapsed -= Time.deltaTime;

        if (m_ClaimBtn.gameObject.activeSelf)
            m_ClaimBtn.gameObject.SetActive(false);
        if (!m_RewardCount.activeSelf)
            m_RewardCount.SetActive(true);

        int hours = (int)farmingTimeElapsed / 3600;
        int minutes = ((int)farmingTimeElapsed % 3600) / 60;
        int seconds = (int)farmingTimeElapsed % 60;

        m_Timer.text = string.Format("{0:D2} : {1:D2} : {2:D2}", hours, minutes, seconds);

        UpdateTickets();
    }

    private void UpdateTickets()
    {
        currentTickets = CalculateTickets();
        m_Count.text = $"{currentTickets}/{GameUtils.MAX_AFK_REWARD}";
    }

    #region on click btn
    private void OnClaim()
    {/*
        if (!string.IsNullOrEmpty(PlayerData.Instance.data.reminderCode))
            WebSocketRequestHelper.CancelReminder();

        WebSocketRequestHelper.SendReminder("Your ticket farming task is waiting. Log in now to get started!", 3600);

        isFarming = false;
        PlayerData.Instance.data.isFarming = isFarming;

        UpdateTickets();
        SetButtonState();

        PlayerData.Instance.AddCurrency(CurrencyType.Ticket, currentTickets);
        currentTickets = 0;

        m_Timer.text = "08 : 00 : 00";
        */

        WebSocketRequestHelper.RequestAFKFarm(PlayerData.Instance.userInfo.telegramCode, (AFKRewardResponse response) => { 
            if (response.type.Equals("claim") && response.success)
            {
                if (!string.IsNullOrEmpty(PlayerData.Instance.data.reminderCode))
                    WebSocketRequestHelper.CancelReminder();

                WebSocketRequestHelper.SendReminder("Your ticket farming task is waiting. Log in now to get started!", 3600);

                PlayerData.Instance.data.isFarming = false;

                UpdateTickets();
                SetButtonState();

                PlayerData.Instance.AddCurrency(CurrencyType.Ticket, currentTickets);
                currentTickets = 0;

                m_Timer.text = "08 : 00 : 00";
            }
        });
    }

    private void StartFarm()
    {
        WebSocketRequestHelper.RequestAFKFarm(PlayerData.Instance.userInfo.telegramCode, (AFKRewardResponse response) => {
            if (response.type.Equals("farming") && response.success)
            {
                if (!string.IsNullOrEmpty(PlayerData.Instance.data.reminderCode))
                    WebSocketRequestHelper.CancelReminder();
                farmingTimeElapsed = 8 * 60 * 60; // 8 hours.
                PlayerData.Instance.data.isFarming = true;
                SetButtonState();
                WebSocketRequestHelper.SendReminder("Farming done!!! Ready to collect?", 3600 * GameUtils.MAX_AFK_HOURS);
            }
        });

        /*
        DateTime startFarmingTime;
        WebSocketRequestHelper.GetTimeOnce((time) =>
        {
            startFarmingTime = GameUtils.ParseTime(time);
            DateTime targetTime = startFarmingTime.AddHours(8);
            PlayerData.Instance.data.targetFarmTime = GameUtils.GetSaveDateString(targetTime);
            TimeSpan timeSpan = targetTime - startFarmingTime;
            farmingTimeElapsed = (float)timeSpan.TotalSeconds;

            isFarming = true;
            PlayerData.Instance.data.isFarming = isFarming;

            if (!string.IsNullOrEmpty(PlayerData.Instance.data.reminderCode)) WebSocketRequestHelper.CancelReminder();

            SetButtonState();

            WebSocketRequestHelper.SendReminder("Farming done!!! Ready to collect?", 3600 * GameUtils.MAX_AFK_HOURS);
            PlayerData.Instance.SaveData();
        });*/
    }
    #endregion
}
