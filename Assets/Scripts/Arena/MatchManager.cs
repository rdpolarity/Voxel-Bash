using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using RDPolarity.UI;
using RDPolarity.Controllers;


namespace RDPolarity
{
    public class MatchManager : NetworkBehaviour
    {
        public delegate void RoundStart(bool value);
        public static event RoundStart disablePlayers;

        [SerializeField] private UIController uiController;
        [SerializeField] private bool suddenDeath;
        [SyncVar(hook = nameof(UpdateTimer))] private float roundTime;

        [SerializeField] private List<PlayerController> players = new List<PlayerController>();
        [SerializeField] private List<PlayerController> alive = new List<PlayerController>();
        private bool roundOver;
        private bool allPlayersConnected;
        private bool countDownFinished;

        [SerializeField] private int countFrom = 3;
        [Serializable] public class OnFinish : UnityEvent { }
        public OnFinish onFinish = new OnFinish();
        [Serializable] public class OnTick : UnityEvent<int> { }
        public OnTick onTick = new OnTick();

        

        // Start is called before the first frame update
        void Start()
        {
            SetPlayers();
            SetInfo();
            roundTime = 10;
            PlayerController.onLoseEvent += PlayerLose;
            PlayerController.onConnectEvent += AddPlayer;
        }

        private void Update()
        {
            
            if (players.Count == NetworkServer.connections.Count && !allPlayersConnected)
            {
                Debug.Log("Starting Countdown");
                allPlayersConnected = true;
                StartCoroutine(TickFor(countFrom));
            }
            else if (allPlayersConnected && countDownFinished)
            {
                
                roundTime -= Time.deltaTime;
                if (roundTime < 0)
                {
                    suddenDeath = true;
                }
            }
            else
            {
                disablePlayers.Invoke(true);
            }

        }

        // Update is called once per frame
        private void UpdateTimer(float oldValue, float newValue)
        {
            string text = "";
            if(roundOver)
            {
                text = alive[0].name + " wins!";
            }
            else if (suddenDeath)
            {
                text = "Sudden Death";
            }
            else
            {
                text = ((int)roundTime).ToString();
            }
            uiController.UpdateTimer(text);
        }

        private void PlayerLose(PlayerController p)
        {
            alive.Remove(p);
        }

        private IEnumerator TickFor(int seconds)
        {
            while (seconds >= 0)
            {
                onTick.Invoke(seconds);
                yield return new WaitForSeconds(1);
                seconds--;
            }
            countDownFinished = true;
            disablePlayers.Invoke(false);
            onFinish.Invoke();
        }

        private void SetInfo()
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].SetPlayerInfo(uiController.GetPlayerInfo(i));
                uiController.GetPlayerInfo(i).gameObject.SetActive(true);
            }
        }

        private void SetPlayers()
        {
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (g.name != "HurtBox")
                {
                    PlayerController player = g.GetComponent<PlayerController>();
                    if (!players.Contains(player))
                    {
                        players.Add(player);
                        alive.Add(player);
                    }
                }
            }
        }

        private void AddPlayer(PlayerController player)
        {
            PlayerInfo info = uiController.GetPlayerInfo(players.Count);
            SetPlayers();
            SetInfo();
        }
    }
}
