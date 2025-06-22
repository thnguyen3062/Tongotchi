using System.Collections;
using Game;
using Game.Websocket.Model;
using Game.Websocket;
using UnityEngine;
using TMPro;

public class UIFindMatchPanel : BaseView
{
    [SerializeField] private TextMeshProUGUI FindingTime;

    public void StartFindMatch(int petId)
    {
        StartCoroutine(CountDownRoutine(petId));
    }

    private IEnumerator CountDownRoutine(int petId)
    {
        float maxTime = 8f;
        float elapsedTime = 0;
        bool matchFound = false;

        PvpCombat pvpCombat = new PvpCombat();

        WebSocketRequestHelper.GoCombatOnce(petId, (combat) =>
        {
            matchFound = true;
            pvpCombat = combat;
        });

        while (elapsedTime < maxTime) // Stop counting when elapsedTime reaches maxTime
        {
            // Update elapsed time
            elapsedTime += 1f;

            // Calculate minutes and seconds
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);

            FindingTime.text = "Finding Opponent...\n" + string.Format("{0:00}:{1:00}", minutes, seconds);

            if (matchFound)
                maxTime = 2f;

            yield return new WaitForSeconds(1f); // Wait for 1 second
        }

        if (matchFound)
        {
            // code logic combat here
            ShowUIView<UIBeforeMatchPanel>().SetBeforeMatch(pvpCombat);
            //SetBeforeMatch(pvpCombat);
        }
        else
        {
            Toast.Show("No opponents found!");
        }
        Hide();
    }
}
