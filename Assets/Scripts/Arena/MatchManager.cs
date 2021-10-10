using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mirror;
using RDPolarity.Controllers;
using RDPolarity.UI;
using UnityEngine;

namespace RDPolarity.Arena
{
    /// <summary>
    /// Manages the win/loss and round logic for the arena
    /// </summary>
    public class MatchManager : NetworkBehaviour
    {

        [SerializeField] private UIController uiController;
        [SerializeField] private bool suddenDeath;
        [SerializeField] private List<PlayerController> players = new List<PlayerController>();
        [SerializeField] private List<PlayerController> alive = new List<PlayerController>();
        [SerializeField] private int countFrom = 3;
        [SerializeField] private CinemachineTargetGroup cameraTargetGroup;
        [SerializeField] private int roundTime = 120;
        
        [SyncVar(hook = nameof(UpdateRoundTimer))] private int _roundTime;
        [SyncVar(hook = nameof(UpdateReadyTimer))] private int _readyTime;
        
        public delegate void RoundStart(bool value);
        public static event RoundStart DisablePlayers;
        
        private bool _allPlayersConnected;
        private bool _isRoundOver;
        
        public delegate void OnReadyTick(int seconds);
        public static event OnReadyTick ONReadyTick;
        
        public delegate void OnReadyFinish();
        public static event OnReadyFinish ONReadyFinish;

        private void OnEnable()
        {
            PlayerController.ONLoseEvent += PlayerLose;
            PlayerController.ONConnectEvent += AddPlayer;
            PlayerController.ONStockUpdated += StockUpdated;

        }

        private void OnDisable()
        {
            PlayerController.ONLoseEvent -= PlayerLose;
            PlayerController.ONConnectEvent -= AddPlayer;
            PlayerController.ONStockUpdated -= StockUpdated;
        }

        private void Start()
        {
            SetPlayers();
            SetInfo();
            if (isServer) _roundTime = roundTime;
        }
        
        private void UpdateRoundTimer(int oldValue, int newValue)
        {
            if (_isRoundOver) return;
            uiController.UpdateTimer(newValue.ToString()); // This needs to change to a event like the ready timer !!
        }
        
        private void UpdateReadyTimer(int oldValue, int newValue)
        {
            ONReadyTick?.Invoke(newValue);
        }

        [Server]
        private void StockUpdated(int stocks)
        {
            if (suddenDeath) OnRoundOver();
        }

        [Server]
        private void PlayerLose(PlayerController player)
        {
            alive.Remove(player.GetComponent<PlayerController>());
            KillPlayer(player.gameObject);
            if (alive?.Count <= 1) OnRoundOver();
        }

        [ClientRpc]
        private void KillPlayer(GameObject player)
        {
            alive.Remove(player.GetComponent<PlayerController>());
            player.transform.position = Vector3.up * 100;
            RemoveCameraTrack(player);
            
        }
        
        [Server]
        private void OnRoundOver()
        {
            StartCoroutine(CountdownFor(3, () => NetworkManager.singleton.ServerChangeScene("lobby")));
            RPCOnRoundOver();
            FreezeAllPlayer(true);
        }

        private void SetInfo()
        {
            for (var i = 0; i < players.Count; i++)
            {
                players[i].SetPlayerInfo(uiController.GetPlayerInfo(i));
                uiController.GetPlayerInfo(i).gameObject.SetActive(true);
            }
        }
        
        private void SetPlayers()
        {
            foreach (var g in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (g.name == "HurtBox") continue;
                var player = g.GetComponent<PlayerController>();
                if (players.Contains(player)) continue;
                players.Add(player);
                alive.Add(player);
            }
        }
        
        [Server]
        private void OnReadyCountdownFinish()
        {
            FreezeAllPlayer(false);
            RPCStartCameraTrack();
            StartCoroutine(CountdownFor(_roundTime, RPCOnSuddenDeath, s => _roundTime = s));
        }

        [ClientRpc]
        private void FreezeAllPlayer(bool isFrozen)
        {
            DisablePlayers?.Invoke(isFrozen);
        }

        [ClientRpc]
        private void RPCStartCameraTrack()
        {
            foreach (var player in players) AddCameraTrack(player.gameObject);
        }

        [ClientRpc]
        private void RPCOnSuddenDeath()
        {
            if (_isRoundOver) return;
            suddenDeath = true;
            uiController.UpdateTimer("Sudden Death");
        }
        
        [ClientRpc]
        private void RPCOnRoundOver()
        {
            _isRoundOver = true;
            if (alive.Count == 0)
            {
                uiController.UpdateTimer("Nobody" + " Wins :(");
            }
            else
            {
                uiController.UpdateTimer(alive[0].name + " Wins!");
            }
        }
        
        private void AddPlayer(PlayerController player)
        {
            var info = uiController.GetPlayerInfo(players.Count);
            SetPlayers();
            SetInfo();
            
            
            if (isServer)
            {
                FreezeAllPlayer(true);
                _allPlayersConnected = players.Count == NetworkServer.connections.Count;
                if (_allPlayersConnected)
                {
                    StartCoroutine(CountdownFor(countFrom, OnReadyCountdownFinish, s => _readyTime = s));
                }
            }
        }
        
        private static IEnumerator CountdownFor(int seconds, Action finished, Action<int> tick = null)
        {
            while (seconds >= 0)
            {
                tick?.Invoke(seconds);
                yield return new WaitForSeconds (1);
                seconds--;
            }
            finished();
        }
        
        private void AddCameraTrack(GameObject player) {
            cameraTargetGroup.AddMember(player.gameObject.transform, 1, 0);
        }
        
        private void RemoveCameraTrack(GameObject player) {
            cameraTargetGroup.RemoveMember(player.gameObject.transform);
        }
    }
}
