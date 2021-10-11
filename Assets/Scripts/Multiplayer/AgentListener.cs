using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.MultiplayerAgent.Model;
using UnityEngine;

namespace RDPolarity.Multiplayer
{
    public class AgentListener : MonoBehaviour {
        private List<ConnectedPlayer> _connectedPlayers;
        public bool Debugging = true;
        // Use this for initialization
        void Start () {
            _connectedPlayers = new List<ConnectedPlayer>();
            PlayFabMultiplayerAgentAPI.Start();
            PlayFabMultiplayerAgentAPI.IsDebugging = Debugging;
            PlayFabMultiplayerAgentAPI.OnMaintenanceCallback += OnMaintenance;
            PlayFabMultiplayerAgentAPI.OnShutDownCallback += OnShutdown;
            PlayFabMultiplayerAgentAPI.OnServerActiveCallback += OnServerActive;
            PlayFabMultiplayerAgentAPI.OnAgentErrorCallback += OnAgentError;

            VoxelBashNetworkManager.Instance.OnPlayerAdded.AddListener(OnPlayerAdded);
            VoxelBashNetworkManager.Instance.OnPlayerRemoved.AddListener(OnPlayerRemoved);

            StartCoroutine(ReadyForPlayers());
        }

        IEnumerator ReadyForPlayers()
        {
            yield return new WaitForSeconds(.5f);
            PlayFabMultiplayerAgentAPI.ReadyForPlayers();
        }
    
        private void OnServerActive()
        {
            VoxelBashNetworkManager.Instance.StartListen();
            Debug.Log("Server Started From Agent Activation");
        }

        private void OnPlayerRemoved(string playfabId)
        {
            ConnectedPlayer player = _connectedPlayers.Find(x => x.PlayerId.Equals(playfabId, StringComparison.OrdinalIgnoreCase));
            _connectedPlayers.Remove(player);
            PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(_connectedPlayers);
        }

        private void OnPlayerAdded(string playfabId)
        {
            _connectedPlayers.Add(new ConnectedPlayer(playfabId));
            PlayFabMultiplayerAgentAPI.UpdateConnectedPlayers(_connectedPlayers);
        }

        private void OnAgentError(string error)
        {
            Debug.Log(error);
        }

        private void OnShutdown()
        {
            Debug.Log("Server is shutting down");
            foreach(var conn in VoxelBashNetworkManager.Instance.Connections)
            {
                conn.Connection.Send<VoxelBashNetworkManager.ShutdownMessage>(new VoxelBashNetworkManager.ShutdownMessage());
            }
            StartCoroutine(Shutdown());
        }

        IEnumerator Shutdown()
        {
            yield return new WaitForSeconds(5f);
            Application.Quit();
        }

        private void OnMaintenance(DateTime? NextScheduledMaintenanceUtc)
        {
            Debug.LogFormat("Maintenance scheduled for: {0}", NextScheduledMaintenanceUtc.Value.ToLongDateString());
            foreach (var conn in VoxelBashNetworkManager.Instance.Connections)
            {
                conn.Connection.Send<VoxelBashNetworkManager.MaintenanceMessage>(new VoxelBashNetworkManager.MaintenanceMessage() {
                    ScheduledMaintenanceUTC = (DateTime)NextScheduledMaintenanceUtc
                });
            }
        }
    }
}