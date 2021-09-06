using System.Collections;
using System.Collections.Generic;
using Mirror;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SteamLobby : MonoBehaviour
{
    [SerializeField]
    private NetworkManager networkManager;
// 
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> joinRequested;
    protected Callback<LobbyEnter_t> lobbyJoin;

    private const string hostAddressKey = "hostAddressKey";

    public void Start() {
        if (!SteamManager.Initialized) { return; }
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        joinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnRequestedJoin);
        lobbyJoin = Callback<LobbyEnter_t>.Create(OnLobbyJoined);

    } 

    public void Join() {

    }

    public void Leave() {
        networkManager.StopClient();
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Hosts a steam lobby
    /// </summary>
    public void Host() {
        SceneManager.LoadScene("Lobby");
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
    }

    /// <summary>
    /// Triggers when a player has successfully joined the steam lobby
    /// </summary>
    /// <param name="lobbyDetails"></param>
    private void OnLobbyJoined(LobbyEnter_t lobbyDetails) {
        // Don't do anything if you're hosting the lobby
        if (NetworkServer.active) { return; }

        // If you're not the host then connect to the steam lobby relay
        string hostAddress = SteamMatchmaking.GetLobbyData(
            new CSteamID(lobbyDetails.m_ulSteamIDLobby),
            hostAddressKey
        );

        networkManager.networkAddress = hostAddress;
        networkManager.StartClient();
    }

    /// <summary>
    /// Triggers when a user requests to join server
    /// </summary>
    private void OnRequestedJoin(GameLobbyJoinRequested_t joinRequestDetails) {
        SceneManager.LoadScene("Lobby");
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

        networkManager.StartHost();
        SteamMatchmaking.SetLobbyData(new CSteamID(lobbyDetails.m_ulSteamIDLobby), hostAddressKey, SteamUser.GetSteamID().ToString());
    }
}
