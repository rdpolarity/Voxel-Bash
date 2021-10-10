using System;
using System.Collections.Generic;
using Mirror;
using RDPolarity.Arena;
using RDPolarity.Multiplayer;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

namespace RDPolarity.UI
{
    /// <summary>
    /// Master class for controlling user interface
    /// </summary>
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameObject escapeMenuGroup;
        [SerializeField] private GameObject lobbyButton;
        [SerializeField] private TMP_Text countdownText;
        [SerializeField] private TMP_Text timer;
        [SerializeField] private List<PlayerInfo> playerInfos;

        private void OnEnable()
        {
            MatchManager.ONReadyTick += OnCountdownTick;
        }

        private void OnDisable()
        {
            MatchManager.ONReadyTick -= OnCountdownTick;
        }

        private void Awake()
        {
            if (lobbyButton != null)
            {
                if (SceneManager.GetActiveScene().name != "Lobby")
                {
                    if (NetworkServer.active && NetworkClient.isHostClient)
                    {
                        lobbyButton.SetActive(true);
                    }
                }
            }
            Inputs inputActions = new Inputs();
            inputActions.Player.Enable();
            inputActions.Player.EscapeMenu.performed += ToggleEscapeMenu;
        }

        public PlayerInfo GetPlayerInfo(int index)
        {
            return playerInfos[index];
        }

        public void OnCountdownTick(int seconds)
        {
            if (countdownText != null)
            {
                if (seconds == 0) countdownText.gameObject.SetActive(false);
                countdownText.text = seconds.ToString();
            }
        }
        
        public void ToggleEscapeMenu(InputAction.CallbackContext context)
        {
            if (escapeMenuGroup != null)
            {
                escapeMenuGroup.SetActive(!escapeMenuGroup.activeSelf);
            }
        }

        public void LeaveGameOnClick()
        {
            var nm = NetworkManager.singleton as VoxelBashNetworkManager;
            nm.Leave();
        }

        public void JoinGameOnClick()
        {
            var nm = NetworkManager.singleton as VoxelBashNetworkManager;
            nm.Join();
        }

        public void HostGameOnClick()
        {
            var nm = NetworkManager.singleton as VoxelBashNetworkManager;
            nm.Host();
        }

        public void UpdateTimer(string time)
        {
            timer.text = time;
        }

        public void GotoLobby()
        {
            if (NetworkServer.active && NetworkClient.isHostClient) {
                NetworkManager.singleton.ServerChangeScene("Lobby");
            }
        }

        /// <summary>
        /// Will go to the main menu offline
        /// </summary>
        public void GotoOfflineMainMenu()
        {
            NetworkManager.singleton.ServerChangeScene("MainMenu");
        }

        public void Exit()
        {
            Application.Quit();
        }
    }
}
