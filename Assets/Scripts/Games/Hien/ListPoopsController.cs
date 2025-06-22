using Game.Websocket;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListPoopsController : MonoBehaviour
{
    public Action OnPoop;
    public Action OnCleanPoop;

    [SerializeField] private List<Transform> m_Poops = new List<Transform>();
    [SerializeField] private GameObject m_CleanAnimGO;

    public void InitPoops(int poopCount)
    {
        foreach (Transform trans in m_Poops)
            trans.gameObject.SetActive(false);
        for (int i = 0; i < poopCount; i++)
        {
            PetDidAnOopsie();
        }
    }

    /// <summary>
    /// Clean all poops.
    /// </summary>
    public void CleanPoops()
    {
        WebSocketRequestHelper.RequestCleanPoop(PlayerData.Instance.PetData.petId, (PetPoopResponse response) => {
            if (response.success && response.poopsCleaned > 0) {
                int totalTicketCanEarn = response.poopsCleaned * 60;

                foreach (Transform trans in m_Poops)
                {
                    if (trans.gameObject.activeSelf)
                    {
                        //totalTicketCanEarn += 60;
                        trans.gameObject.SetActive(false);
                    }
                }

                PlayCleanPoopAnim();
                OnCleanPoop?.Invoke();
                PlayerData.Instance.AddCurrency(CurrencyType.Ticket, totalTicketCanEarn);
                PlayerData.Instance.PetData.SetPoopCount(0);
            }
        });
    }
    
    /// <summary>
    /// Spawn poop.
    /// </summary>
    public void PetDidAnOopsie()
    {
        bool checkPoop = false;
        while (!checkPoop)
        {
            int ind = RandomPoop();
            if (!m_Poops[ind].gameObject.activeInHierarchy)
            {
                m_Poops[ind].gameObject.SetActive(true);
                checkPoop = true;
                OnPoop?.Invoke();
            }
            else
            {
                checkPoop = false;
            }
        }
    }

    public void PlayCleanPoopAnim()
    {
        m_CleanAnimGO.gameObject.SetActive(true);
    }


    private int RandomPoop()
    {
        return UnityEngine.Random.Range(0, 8);
    }
}
