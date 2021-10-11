using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using PlayFab;
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
        String username;
    }

    /// <summary>
    /// Network manager for voxel bash's addition network logic
    /// </summary>
    public class VoxelBashNetworkManager : NetworkManager
    {
        public static VoxelBashNetworkManager Instance { get; private set; }

        public PlayerEvent OnPlayerAdded = new PlayerEvent();
        public PlayerEvent OnPlayerRemoved = new PlayerEvent();
        
        public int MaxConnections = 100;
        public int Port = 7777;
        
        public List<UnityNetworkConnection> Connections
        {
            get { return _connections; }
            private set { _connections = value; }
        }
        
        private List<UnityNetworkConnection> _connections = new List<UnityNetworkConnection>();

        public class PlayerEvent : UnityEvent<string>
        {
        }

        public override void Awake()
        {
            base.Awake();
            
            Instance = this;
            NetworkServer.RegisterHandler<ReceiveAuthenticateMessage>(OnReceiveAuthenticate);
            //_netManager.transport.port = Port;
        }
        
        public void StartListen()
        {
            NetworkServer.Listen(MaxConnections);
        }
        
        public override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            NetworkServer.Shutdown();
        }

        private void OnReceiveAuthenticate(NetworkConnection nconn, ReceiveAuthenticateMessage message)
        {
            var conn = _connections.Find(c => c.ConnectionId == nconn.connectionId);
            if (conn != null)
            {
                conn.PlayFabId = message.PlayFabId;
                conn.IsAuthenticated = true;
                OnPlayerAdded.Invoke(message.PlayFabId);
            }
        }
        
        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);

            Debug.LogWarning("Client Connected");
            var uconn = _connections.Find(c => c.ConnectionId == conn.connectionId);
            if (uconn == null)
            {
                _connections.Add(new UnityNetworkConnection()
                {
                    Connection = conn,
                    ConnectionId = conn.connectionId,
                    LobbyId = PlayFabMultiplayerAgentAPI.SessionConfig.SessionId
                });
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
                if (Application.platform != RuntimePlatform.WebGLPlayer)
                {
                    StartHost();
                    if (SceneManager.GetActiveScene().name == "MainMenu") ServerChangeScene("Lobby");
                }
            }
        }

        public void Join()
        {
            StartClient();
            if (SceneManager.GetActiveScene().name == "MainMenu") ServerChangeScene("Lobby");
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);

            var uconn = _connections.Find(c => c.ConnectionId == conn.connectionId);
            if (uconn != null)
            {
                if (!string.IsNullOrEmpty(uconn.PlayFabId))
                {
                    OnPlayerRemoved.Invoke(uconn.PlayFabId);
                }

                _connections.Remove(uconn);
            }
        }
        
        [Serializable]
        public class UnityNetworkConnection
        {
            public bool IsAuthenticated;
            public string PlayFabId;
            public string LobbyId;
            public int ConnectionId;
            public NetworkConnection Connection;
        }
        
        public class CustomGameServerMessageTypes
        {
            public const short ReceiveAuthenticate = 900;
            public const short ShutdownMessage = 901;
            public const short MaintenanceMessage = 902;
        }
        
        public struct ReceiveAuthenticateMessage : NetworkMessage
        {
            public string PlayFabId;
        }

        public struct ShutdownMessage : NetworkMessage
        {
        }
        
        [Serializable]
        public struct MaintenanceMessage : NetworkMessage
        {
            public DateTime ScheduledMaintenanceUTC;
        }
        
        public static class MaintenanceMessageFunctions
        {
            public static MaintenanceMessage Deserialize(NetworkReader reader)
            {
                MaintenanceMessage msg = new MaintenanceMessage();

                var json = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);
                msg.ScheduledMaintenanceUTC = json.DeserializeObject<DateTime>(reader.ReadString());

                return msg;
            }

            public static void Serialize(NetworkWriter writer, MaintenanceMessage value)
            {
                var json = PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);
                var str = json.SerializeObject(value.ScheduledMaintenanceUTC);
                writer.Write(str);
            }
        }
        
        public override void OnServerError(NetworkConnection conn, int exception)
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
            NetworkClient.connection.Send(new SpawnPlayerMessage());
        }

        [Server]
        private void OnCreateClient(NetworkConnection conn, SpawnPlayerMessage msg)
        {
            Transform startPos = GetStartPosition();
            GameObject player = startPos != null
                ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
                : Instantiate(playerPrefab);
            player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
            NetworkServer.AddPlayerForConnection(conn, player);
        }
    }
}