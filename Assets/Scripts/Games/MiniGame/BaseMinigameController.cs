using Game.Websocket.Commands.Storage;
using Game.Websocket;
using UnityEngine;

namespace Game
{
    public class BaseMinigameController : MonoBehaviour
    {
        private UIMinigameHome minigameHome;
        private PopUpGameOver gameOverUI;
        protected int remainTurn;
        protected int maxTurn;

        public bool GameStarted { get; protected set; }
        protected UIMinigameHome MinigameHome => minigameHome;
        protected PopUpGameOver UIGameOver => gameOverUI;

        public void InitializeMinigame()
        {
            UIManager.ShowUIView<UILoadingView>();
            if (!MinigameHome) minigameHome = UIManager.GetUIView<UIMinigameHome>();
            if (!UIGameOver) gameOverUI = UIManager.GetUIView<PopUpGameOver>();
            MinigameHome.Show();
            UIGameOver.Hide();
            GameStarted = false;
            WebSocketRequestHelper.RequestGetMinigameTicket(PlayerData.Instance.userInfo.telegramCode, (GetMinigameTicketResponse response) =>
            {
                UIManager.HideUIView<UILoadingView>();

                minigameHome.SetOnPlayMinigameCallback(StartGame).SetPlayLabel($"Play {response.remainingPlays}<sprite=0>");
                remainTurn = response.remainingPlays;
                OnInitializedComplete();
            });
        }

        public void StartGame()
        {
            UIGameOver.Hide();
            UIManager.ShowUIView<UILoadingView>();
            WebSocketRequestHelper.RequestPlayMinigame(PlayerData.Instance.userInfo.telegramCode, (PlayMinigameResponse response) =>
            {
                UIManager.HideUIView<UILoadingView>();

                if (response.success)
                {
                    remainTurn = response.remainingPlays;
                    maxTurn = response.maxPurchasesPerDay;
                    MinigameHome.Hide();
                    
                    OnGameStarted(true);
                }
                else
                {
                    OnGameStarted(false);
                    UIManager.Instance.ShowView<PopupNotify>().SetOnConfirmBuyMinigame((success, remain, max) =>
                    {
                        if (success)
                        {
                            MinigameHome.SetPlayLabel($"Play {remain}<sprite=0>");
                            UIGameOver.SetRemainTicket(remainTurn, maxTurn);
                        }
                    }).Init(
                        "OUT OF MINIGAME TICKETS",
                        "You've out of minigame tickets!\nYour tickets will re-fill tomorrow!\nOr buy 5 tickets with only 1<sprite=2>",
                        1, false
                    );
                }
            });
        }

        protected virtual void OnInitializedComplete() { }
        protected virtual void OnGameStarted(bool success) { }
    }
}