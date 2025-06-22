using PathologicalGames;
using UnityEngine;

namespace Game.UI.Popup
{
    public class UIPopupHandler : MonoBehaviour
    {
        [SerializeField] private Transform m_ParentPopup;
        [SerializeField] private Transform m_PopupLevelUpReward;
        [SerializeField] private Transform m_PopupLevelUpRewardFailed;
        [SerializeField] private Transform m_PopupEvovle;
        [SerializeField] private Transform m_PopupEvovleWarning;
        [SerializeField] private Transform m_PopupConfirmPurchase;
        [SerializeField] private Transform m_PopupPurchaseResult;
        [SerializeField] private Transform m_PopupNotify;
        [SerializeField] private Transform m_PopupGotchiPointsInfo;
        [SerializeField] private Transform m_PopupMinigameInfo;
        [SerializeField] private Transform m_FriendHandler;
        [SerializeField] private Transform m_TaskPopup;
        //[SerializeField] private Transform m_FadeScreen;

        public Transform ParentPopup => m_ParentPopup;

        #region levelup reward
        private PopupLevelupReward _popupLevelupReward;
        public PopupLevelupReward PopupLevelupReward
        {
            get
            {
                if (_popupLevelupReward == null)
                {
                    Transform trans = PoolManager.Pools["UI"].Spawn(m_PopupLevelUpReward, m_ParentPopup);
                    _popupLevelupReward = trans.GetComponent<PopupLevelupReward>();
                }
                return _popupLevelupReward;
            }
        }

        private PopupLevelupReward _popupLevelupRewardFailed;
        public PopupLevelupReward PopupLevelupRewardFailed
        {
            get
            {
                if(_popupLevelupRewardFailed == null)
                {
                    Transform trans = PoolManager.Pools["UI"].Spawn(m_PopupLevelUpRewardFailed, m_ParentPopup);
                    _popupLevelupRewardFailed = trans.GetComponent<PopupLevelupReward>();
                }
                return _popupLevelupRewardFailed;
            }
        }
        #endregion

        #region evolve
        private PopupEvolve _popupEvolve;
        public PopupEvolve PopupEvolve
        {
            get
            {
                if( _popupEvolve == null)
                {
                    Transform trans = PoolManager.Pools["UI"].Spawn(m_PopupEvovle, m_ParentPopup);
                    _popupEvolve = trans.GetComponent<PopupEvolve>();
                }
                return _popupEvolve;
            }
        }

        private PopupEvolve _popupEvolveFailed;
        public PopupEvolve PopupEvolveFailed
        {
            get
            {
                if(_popupEvolveFailed == null)
                {
                    Transform trans = PoolManager.Pools["UI"].Spawn(m_PopupEvovleWarning, m_ParentPopup);
                    _popupEvolveFailed = trans.GetComponent<PopupEvolve>();
                }
                return _popupEvolveFailed;
            }
        }
        #endregion

        #region purchase
        private PopupConfirmPurchase _popupConfirmPurchase;
        public PopupConfirmPurchase PopupConfirmPurchase
        {
            get
            {
                if (_popupConfirmPurchase == null)
                {
                    Transform trans = PoolManager.Pools["UI"].Spawn(m_PopupConfirmPurchase, m_ParentPopup);
                    _popupConfirmPurchase = trans.GetComponent<PopupConfirmPurchase>();
                }
                return _popupConfirmPurchase;
            }
        }

        private PopupPurchaseResult _popupPurchaseResult;
        public PopupPurchaseResult PopupPurchaseResult
        {
            get
            {
                if(_popupPurchaseResult == null)
                {
                    Transform trans = PoolManager.Pools["UI"].Spawn(m_PopupPurchaseResult, m_ParentPopup);
                    _popupPurchaseResult = trans.GetComponent<PopupPurchaseResult>();
                }
                return _popupPurchaseResult;
            }
        }
        #endregion

        #region notify
        private PopupNotify _popupNotify;
        public PopupNotify PopupNotify
        {
            get
            {
                if( _popupNotify == null)
                {
                    Transform trans = PoolManager.Pools["UI"].Spawn(m_PopupNotify, m_ParentPopup);
                    _popupNotify = trans.GetComponent<PopupNotify>();
                }
                return _popupNotify;
            }
        }
        #endregion

        #region info
        private Transform _popupGotchiPointInfo;
        public Transform PopupGotchiPointInfo
        {
            get
            {
                if (_popupGotchiPointInfo == null)
                    _popupGotchiPointInfo = PoolManager.Pools["UI"].Spawn(m_PopupGotchiPointsInfo, m_ParentPopup);
                return _popupGotchiPointInfo;
            }
        }

        private Transform _popupMinigameInfo;
        public Transform PopupMinigameInfo
        {
            get
            {
                if (_popupMinigameInfo == null)
                    _popupMinigameInfo = PoolManager.Pools["UI"].Spawn(m_PopupMinigameInfo, m_ParentPopup);
                return _popupMinigameInfo;
            }
        }
        #endregion

        #region friend
        private FriendHandler _friendHandler;
        public FriendHandler FriendHandler
        {
            get
            {
                if ( _friendHandler == null)
                {
                    Transform trans = PoolManager.Pools["UI"].Spawn(m_FriendHandler, m_ParentPopup);
                    _friendHandler = trans.GetComponent<FriendHandler>();
                }
                return _friendHandler;
            }
        }

        public void OnShowFriend()
        {
            _friendHandler.gameObject.SetActive(true);
            _friendHandler.Init();
        }
        #endregion

        #region task
        private TaskHandler _taskHandler;
        public TaskHandler TaskHandler
        {
            get
            {
                if(_taskHandler == null)
                {
                    Transform trans = PoolManager.Pools["UI"].Spawn(m_TaskPopup, m_ParentPopup);
                    _taskHandler = trans.GetComponent<TaskHandler>();
                }    
                return _taskHandler;
            }
        }

        public void OpenTask()
        {
            _taskHandler.gameObject.SetActive(true);
            _taskHandler.InitTask();
        }
        #endregion
    }
}