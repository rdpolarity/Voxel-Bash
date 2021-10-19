using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.FizzySteam;
using RDPolarity.Controllers;
using Steamworks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace RDPolarity.Multiplayer
{
    /// <summary>
    /// Message data for spawning players on connection
    /// </summary>
    public struct SpawnPlayerMessage : NetworkMessage
    {
        public string username;
    }

    /// <summary>
    /// Network manager for voxel bash's addition network logic
    /// </summary>
    public class VoxelBashNetworkManager : NetworkManager
    {
        [SerializeField] private bool enableSteam;
        private Callback<LobbyCreated_t> _lobbyCreated;
        private Callback<GameLobbyJoinRequested_t> _joinRequested;
        private Callback<LobbyEnter_t> _lobbyJoin;

        private const string HostAddressKey = "hostAddressKey";

        public override void Awake()
        {
            if (enableSteam)
            {
                transport = GetComponent<FizzySteamworks>();
            }
            else
            {
                transport = GetComponent<TelepathyTransport>();
            }
            
            base.Awake();
        }

        public override void Start()
        {
            base.Start();
            if (enableSteam)
            {
                if (!SteamManager.Initialized) { return; }
                _lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
                _joinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnRequestedJoin);
                _lobbyJoin = Callback<LobbyEnter_t>.Create(OnLobbyJoined);
            }
        }

        public void Leave()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                StopHost();
            }
            else if (NetworkClient.isConnected)
            {
                StopClient();
            }
            else if (NetworkServer.active)
            {
                StopServer();
            }

            SceneManager.LoadScene("MainMenu");
        }

        public void Host()
        {
            if (!NetworkClient.active)
            {
                if (enableSteam)
                {
                    SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, maxConnections);
                }
                else
                {
                    StartHost();
                    if (SceneManager.GetActiveScene().name == "MainMenu") ServerChangeScene("Lobby");
                }
            }
        }

        public void Join()
        {
            if (enableSteam)
            {
                SteamFriends.ActivateGameOverlay("friends");
            }
            else
            {
                StartClient();
                if (SceneManager.GetActiveScene().name == "MainMenu") ServerChangeScene("Lobby");
            }
        }

        public override void OnServerError(NetworkConnection conn, Exception exception)
        {
            base.OnServerError(conn, exception);
            SceneManager.LoadScene("ConnectionLost");
        }

        public override void OnClientError(Exception exception)
        {
            base.OnClientError(exception);
            SceneManager.LoadScene("ConnectionLost");
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            SceneManager.LoadScene("ConnectionLost");
        }

        public override void OnStopClient()
        {
            if (!NetworkClient.isHostClient)
            {
                SceneManager.LoadScene("ConnectionLost");
            }

            base.OnStopClient();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            NetworkServer.RegisterHandler<SpawnPlayerMessage>(OnCreateClient);
        }


        public override void OnStopServer()
        {
            base.OnStopServer();
            SceneManager.LoadScene("ConnectionLost");
            NetworkServer.UnregisterHandler<SpawnPlayerMessage>();
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);
            NetworkClient.connection.Send(new SpawnPlayerMessage());
            Debug.Log("New player has connected!");
        }

        public override void OnClientSceneChanged(NetworkConnection conn)
        {
            base.OnClientSceneChanged(conn);
            NetworkClient.connection.Send(new SpawnPlayerMessage()
            {
                username = enableSteam ? SteamFriends.GetPersonaName() : "DebugPlayer"
            });
        }

        [Server]
        private void OnCreateClient(NetworkConnection conn, SpawnPlayerMessage msg)
        {
            Transform startPos = GetStartPosition();
            GameObject player = startPos != null
                ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
                : Instantiate(playerPrefab);
            player.name = msg.username + " " + conn.connectionId;
            NetworkServer.AddPlayerForConnection(conn, player);
        }

        /// STEAM LOGIC
        
        /// <summary>
        /// Triggers when a player has successfully joined the steam lobby
        /// </summary>
        /// <param name="lobbyDetails"></param>
        private void OnLobbyJoined(LobbyEnter_t lobbyDetails) {
            // Don't do anything if you're hosting the lobby
            if (NetworkServer.active) { return; }
            
            if (SceneManager.GetActiveScene().name == "MainMenu") ServerChangeScene("Lobby");
            
            // If you're not the host then connect to the steam lobby relay
            string hostAddress = SteamMatchmaking.GetLobbyData(
                new CSteamID(lobbyDetails.m_ulSteamIDLobby),
                HostAddressKey
            );

            networkAddress = hostAddress;
            StartClient();
        }

        /// <summary>
        /// Triggers when a user requests to join server
        /// </summary>
        private void OnRequestedJoin(GameLobbyJoinRequested_t joinRequestDetails) {
            SteamMatchmaking.JoinLobby(joinRequestDetails.m_steamIDLobby);
        }

        /// <summary>
        /// Triggers when a lobby is created
        /// </summary>
        /// <param name="lobbyDetails">Data received back on the connection details</param>
        private void OnLobbyCreated(LobbyCreated_t lobbyDetails) {
            if (lobbyDetails.m_eResult != EResult.k_EResultOK) {
                return;
            }

            StartHost();
            if (SceneManager.GetActiveScene().name == "MainMenu") ServerChangeScene("Lobby");
            SteamMatchmaking.SetLobbyData(new CSteamID(lobbyDetails.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        }
    }
}