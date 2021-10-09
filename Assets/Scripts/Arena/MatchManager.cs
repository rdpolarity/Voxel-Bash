using System;
using System.Collections;
using System.Collections.Generic;
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
        
        [SyncVar(hook = nameof(UpdateRoundTimer))] private int _roundTime;
        [SyncVar(hook = nameof(UpdateReadyTimer))] private int _readyTime;
        
        public delegate void RoundStart(bool value);
        public static event RoundStart DisablePlayers;
        
        private bool _allPlayersConnected;
        
        public delegate void OnReadyTick(int seconds);
        public static event OnReadyTick ONReadyTick;

        private void OnEnable()
        {
            PlayerController.ONLoseEvent += PlayerLose;
            PlayerController.ONConnectEvent += AddPlayer;
        }

        private void OnDisable()
        {
            PlayerController.ONLoseEvent -= PlayerLose;
            PlayerController.ONConnectEvent -= AddPlayer;
        }

        private void Start()
        {
            SetPlayers();
            SetInfo();
            if (isServer) _roundTime = 10;
        }
        
        private void UpdateRoundTimer(int oldValue, int newValue)
        {
            uiController.UpdateTimer(newValue.ToString()); // This needs to change to a event like the ready timer !!
        }
        
        private void UpdateReadyTimer(int oldValue, int newValue)
        {
            ONReadyTick?.Invoke(newValue);
        }

        private void PlayerLose(PlayerController player)
        {
            alive.Remove(player);
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
            DisablePlayers?.Invoke(false);
            StartCoroutine(CountdownFor(_roundTime, RPCOnSuddenDeath, s => _roundTime = s));

        }
        
        [ClientRpc]
        private void RPCOnSuddenDeath()
        {
            suddenDeath = true;
            uiController.UpdateTimer("Sudden Death");
        }
        
        [ClientRpc]
        private void RPCOnRoundOver()
        {
            DisablePlayers?.Invoke(true);
            uiController.UpdateTimer(alive[0].name + " Wins!");
        }
        
        [Server]
        private void AddPlayer(PlayerController player)
        {
            var info = uiController.GetPlayerInfo(players.Count);
            SetPlayers();
            SetInfo();
            
            DisablePlayers?.Invoke(true);
            
            _allPlayersConnected = players.Count == NetworkServer.connections.Count;
            
            if (_allPlayersConnected)
            {
                StartCoroutine(CountdownFor(countFrom, OnReadyCountdownFinish, s => _readyTime = s));
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
    }
}
