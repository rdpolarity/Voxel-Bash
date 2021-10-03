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
        public delegate void RoundStart();
        public static event RoundStart roundStartEvent;

        [SerializeField] private UIController uiController;
        [SerializeField] private bool suddenDeath;
        [SyncVar(hook = nameof(UpdateTimer))] private float roundTime;

        [SerializeField] private List<PlayerController> players = new List<PlayerController>();
        [SerializeField] private List<PlayerController> alive = new List<PlayerController>();
        private bool roundOver;
        private bool allPlayersConnected;

        [SerializeField] private int countFrom = 3;
        [Serializable] public class OnFinish : UnityEvent { }
        public OnFinish onFinish = new OnFinish();
        [Serializable] public class OnTick : UnityEvent<int> { }
        public OnTick onTick = new OnTick();

        

        // Start is called before the first frame update
        void Start()
        {
            SetPlayers();
            alive = players;
            roundTime = 10;
            PlayerController.onLoseEvent += PlayerLose;
            PlayerController.onConnectEvent += AddPlayer;
        }

        private void Update()
        {
            if (players.Count == NetworkServer.connections.Count && !allPlayersConnected)
            {
                allPlayersConnected = true;
                StartCoroutine(TickFor(countFrom));
            }
            if (allPlayersConnected)
            {
                roundTime -= Time.deltaTime;
                if (roundTime < 0)
                {
                    suddenDeath = true;
                }
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

        private void SetPlayers()
        {
            foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (g.name.Contains("Player"))
                {
                    if (!players.Contains(g.GetComponent<PlayerController>()))
                    {
                        AddPlayer(g.GetComponent<PlayerController>());
                    }

                }
            }
        }

        private IEnumerator TickFor(int seconds)
        {
            while (seconds >= 0)
            {
                onTick.Invoke(seconds);
                yield return new WaitForSeconds(1);
                seconds--;
            }
            onFinish.Invoke();
        }

        //This is a bandaid fix for when players join late
        private IEnumerator DelayedSetPlayer()
        {
            yield return new WaitForSeconds(3f);
            SetPlayers();
        }
        private void AddPlayer(PlayerController player)
        {
            PlayerInfo info = uiController.GetPlayerInfo(players.Count);
            Debug.Log(players.Count);
            players.Add(player);
            
            info.gameObject.SetActive(true);
            player.SetPlayerInfo(info);
        }
    }
}
