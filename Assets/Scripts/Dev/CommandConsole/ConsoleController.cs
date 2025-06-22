namespace DevTools
{
    using Game;
    using Game.Websocket;
    using Game.Websocket.Commands.Game;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class ConsoleController : MonoBehaviour
    {
        [SerializeField] private Button m_ConsoleBtn;
        [SerializeField] private UIConsoleView m_ConsoleView;

        private List<object> commandList = new List<object>();

        private void Awake()
        {
            m_ConsoleBtn.onClick.AddListener(m_ConsoleView.ToggleView);
            m_ConsoleView.OnSendCommand += ReceiveCommand;
        }

        private void Start()
        {
            LoadCommands();
        }

        private void OnDestroy()
        {
            m_ConsoleBtn.onClick.RemoveListener(m_ConsoleView.ToggleView);
        }

        private void LoadCommands()
        {
            ConsoleCommand<int> command_AddDiamond = new ConsoleCommand<int>("ADD_DIAMOND", "Add 10 more diamonds", "ADD_DIAMOND", (int param) => {
                PlayerData.Instance.AddCurrency(CurrencyType.Diamond, param);
            });

            ConsoleCommand<int> command_IncreaseTicket = new ConsoleCommand<int>("INCREASE_TICKET", "Add n-th more tickets", "INCREASE_TICKET", (int param) => {
                PlayerData.Instance.AddTicket(param, (TicketChangeResponse response) => {
                    LoggerUtil.Logging($"Response.IncreaseTicket: {response.ToString()}", TextColor.Red);
                });
            });

            ConsoleCommand<int> command_DecreaseTicket = new ConsoleCommand<int>("DECREASE_TICKET", "Decrease n-th tickets", "DECREASE_TICKET", (int param) =>
            {
                PlayerData.Instance.DecreaseTicket(param, (TicketChangeResponse response) => {
                    LoggerUtil.Logging($"Response.DecreaseTicket: {response.ToString()}", TextColor.Red);
                });
            });

            ConsoleCommand command_GetTickets = new ConsoleCommand("GET_TICKETS", "Query the current tickets", "GET_TICKETS", () => {
                PlayerData.Instance.GetRemainTickets((GetCurrencyResponse response) => {
                    LoggerUtil.Logging($"Response.GetTicket: {response.success}", TextColor.Red);
                });
            });

            ConsoleCommand command_CheckPetStats = new ConsoleCommand("PET_STATUS", "Query the selected pet's statuses", "PET_STATUS", () => {
                //GameManager.instance.PetController.StatusManager.CheckPetStatus();
            });

            ConsoleCommand command_DailyReward = new ConsoleCommand("DAILY_REWARD", "Call the daily reward request", "DAILY_REWARD", () => {
                WebSocketRequestHelper.RequestDailyReward(PlayerData.Instance.userInfo.telegramCode, (DailyRewardResponse response) => {
                    LoggerUtil.Logging("DAILY REWARD", $"{response.ToString()}");
                });
            });

            ConsoleCommand<int> command_Use1Food = new ConsoleCommand<int>("USE_1_FOOD", "", "USE_1_FOOD", (int value) => {
                WebSocketRequestHelper.FeedPetOnce(PlayerData.Instance.PetData.petId, value, (PetStatusesResponse response, PetLevelExpResponse exp) => {

                });
            });

            ConsoleCommand command_TestStorage = new ConsoleCommand("STORAGE_TEST", "", "STORAGE_TEST", () => {
                WebSocketRequestHelper.TestStorage(PlayerData.Instance.userInfo.telegramCode, () => {

                });
            });

            ConsoleCommand command_LoadItems = new ConsoleCommand("LOAD_ITEMS", "", "LOAD_ITEMS", () => {
                WebSocketRequestHelper.RequestLoadInventory((Dictionary<int, InventoryItem> data) => {
                    LoggerUtil.Logging("RESPONSE.LOAD_INVENTORY", data[0].data.id.ToString());
                });
            });

            ConsoleCommand<int> command_BuyItem = new ConsoleCommand<int>("BUY_ITEM", "", "BUY_ITEM", (int value) => {
                WebSocketRequestHelper.RequestBuyItem(value, 1);
            });

            ConsoleCommand command_PetSick = new ConsoleCommand("PET_SICK", "", "PET_SICK", () => {
                WebSocketRequestHelper.RequestPetSick(PlayerData.Instance.PetData.petId, (PetStateResponse state) => { });
            });

            ConsoleCommand command_PetDie = new ConsoleCommand("PET_DIE", "", "PET_DIE", () => {
                GameManager.Instance.PetController.RequestDie();
            });

            ConsoleCommand command_ClaimFriend = new ConsoleCommand("CLAIM_FRIEND_REWARD", "", "", () => {
                WebSocketRequestHelper.ClaimScoreOnce((ClaimScoreResponse response) => {
                    LoggerUtil.Logging("CLAIM_FRIEND_REWARD", $"Exp={response.referenceExp}, Level={response.referenceLv}");
                });
            });

            ConsoleCommand command_Poop = new ConsoleCommand("POOP", "", "", () => {
                WebSocketRequestHelper.RequestPetPoop(PlayerData.Instance.PetData.petId, (PetPoopResponse response) => {
                    LoggerUtil.Logging("Poop_Response", $"Success={response.success}\nMessage={response.message}\nSpawnedPoop(s)={response.poopsCleaned}");
                });
            });

            ConsoleCommand command_CleanPoop = new ConsoleCommand("CLEAN_P", "", "", () => {
                WebSocketRequestHelper.RequestCleanPoop(PlayerData.Instance.PetData.petId, (PetPoopResponse response) => {
                    LoggerUtil.Logging("Clean_Poop_Response", $"Success={response.success}\nMessage={response.message}\nCleanedPoop(s)={response.poopsCleaned}");
                });
            });

            ConsoleCommand command_QueryBoost = new ConsoleCommand("Query_Boost", "", "", () => {
                WebSocketRequestHelper.RequestQueryBoost(PlayerData.Instance.PetData.petId, (GetBoostsResponse response) => {
                    LoggerUtil.Logging("Response.QueryBoosts");
                });
            });

            ConsoleCommand command_Storage = new ConsoleCommand("QUERY_STORAGE", "", "", () => {
                WebSocketRequestHelper.LoadFromCloudOnce((string json) => { });
            });

            ConsoleCommand command_LoadPet = new ConsoleCommand("QUERY_PET", "", "", () => {
                WebSocketRequestHelper.LoadPetOnce(PlayerData.Instance.PetData.petId, (GamePetData data) => { });
            });

            ConsoleCommand command_CurePet = new ConsoleCommand("CURE_PET", "", "", () => {
                GameManager.Instance.UseItemForPet(24, ItemCategory.Medicine, false, () => { });
            });

            ConsoleCommand command_Revive = new ConsoleCommand("REVIVE", "", "", () => {
                GameManager.Instance.RevivePet((RevivePetResponse res) => { });
                //WebSocketRequestHelper.RequestRevivePet(PlayerData.Instance.PetData.petId, 2);
            });

            ConsoleCommand<int> command_StartFusion = new ConsoleCommand<int>("START_FUSION", "", "", (int param) => {
                WebSocketRequestHelper.RequestStartFusion(0, 1, param, (FusionStartResult result) => { 

                });
            });

            ConsoleCommand<string> command_ClaimFusion = new ConsoleCommand<string>("CLAIM_FUSION", "", "", (string fusionIdParam) => {
                if (!string.IsNullOrEmpty(fusionIdParam))
                {
                    PlayerData.Instance.ClaimFusion(fusionIdParam.Trim());
                }
                else
                {
                    Debug.LogError("Fusion id is empty!");
                }
            });

            ConsoleCommand command_ChangePet = new ConsoleCommand("CHANGE_PET", "", "", () => { 
                WebSocketRequestHelper.RequestChangePetId(0, () => {
                    WebSocketRequestHelper.LoadFromCloudOnce((string json) => { });
                });
            });

#if UNITY_EDITOR
            ConsoleCommand<int> command_CheatLevel = new ConsoleCommand<int>("CHEAT_LEVEL", "", "", (int level) => { 
                PlayerData.Instance.CheatPetLevel(level);
            });
#endif

            commandList.Add(command_TestStorage);
            commandList.Add(command_AddDiamond);
            commandList.Add(command_IncreaseTicket);
            commandList.Add(command_DecreaseTicket);
            commandList.Add(command_GetTickets);
            commandList.Add(command_CheckPetStats);
            commandList.Add(command_DailyReward);
            commandList.Add(command_Use1Food);
            commandList.Add(command_LoadItems);
            commandList.Add(command_BuyItem);
            commandList.Add(command_PetSick);
            commandList.Add(command_PetDie);
            commandList.Add(command_ClaimFriend);
            commandList.Add(command_Poop);
            commandList.Add(command_CleanPoop);
            commandList.Add(command_QueryBoost);
            commandList.Add(command_LoadPet);
            commandList.Add(command_CurePet);
            commandList.Add(command_Revive);
            commandList.Add(command_Storage);
            commandList.Add(command_StartFusion);
            commandList.Add(command_ClaimFusion);
            commandList.Add(command_ChangePet);

#if UNITY_EDITOR
            commandList.Add(command_CheatLevel);
#endif
        }

        private void ReceiveCommand(string commandInput)
        {
            string[] args = commandInput.Split(' ');

            for (int i = 0; i < commandList.Count; i++)
            {
                BaseConsoleCommand baseCommand = commandList[i] as BaseConsoleCommand;

                if (commandInput.Contains(baseCommand.CommandID))
                {
                    if (commandList[i] as ConsoleCommand != null)
                    {
                        (commandList[i] as ConsoleCommand).Invoke();
                    }
                    else if (baseCommand as ConsoleCommand<int> != null)
                    {
                        (commandList[i] as ConsoleCommand<int>).Invoke(int.Parse(args[1]));
                    }
                    else if (baseCommand as ConsoleCommand<string> != null)
                    {
                        (commandList[i] as ConsoleCommand<string>).Invoke(args[1]);
                    }
                }
            }
        }
    }
}