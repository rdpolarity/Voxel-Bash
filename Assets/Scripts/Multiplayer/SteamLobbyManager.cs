using UnityEngine;
// using Steamworks;

namespace RDPolarity.Multiplayer
{
    /// <summary>
    /// Manages all steam related logic
    /// </summary>
    public class SteamLobbyManager : MonoBehaviour
    {
//     [SerializeField]
//     private NetworkManager networkManager;
// // 
//     protected Callback<LobbyCreated_t> lobbyCreated;
//     protected Callback<GameLobbyJoinRequested_t> joinRequested;
//     protected Callback<LobbyEnter_t> lobbyJoin;

//     private const string hostAddressKey = "hostAddressKey";

//     public void Start() {
//         if (!SteamManager.Initialized) { return; }
//         lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
//         joinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnRequestedJoin);
//         lobbyJoin = Callback<LobbyEnter_t>.Create(OnLobbyJoined);

//     } 

//     public void Join() {

//     }

//     public void Leave() {
//         networkManager.ServerChangeScene("MainMenu");
//         networkManager.StopClient();
//     }

//     /// <summary>
//     /// Hosts a steam lobby
//     /// </summary>
//     public void Host() {
//         SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
//         networkManager.ServerChangeScene("Lobby");
//     }

//     /// <summary>
//     /// Triggers when a player has successfully joined the steam lobby
//     /// </summary>
//     /// <param name="lobbyDetails"></param>
//     private void OnLobbyJoined(LobbyEnter_t lobbyDetails) {
//         // Don't do anything if you're hosting the lobby
//         if (NetworkServer.active) { return; }

//         // If you're not the host then connect to the steam lobby relay
//         string hostAddress = SteamMatchmaking.GetLobbyData(
//             new CSteamID(lobbyDetails.m_ulSteamIDLobby),
//             hostAddressKey
//         );

//         networkManager.networkAddress = hostAddress;
//         networkManager.StartClient();
//     }

//     /// <summary>
//     /// Triggers when a user requests to join server
//     /// </summary>
//     private void OnRequestedJoin(GameLobbyJoinRequested_t joinRequestDetails) {
//         networkManager.ServerChangeScene("Lobby");
//         SteamMatchmaking.JoinLobby(joinRequestDetails.m_steamIDLobby);
//     }

//     /// <summary>
//     /// Triggers when a lobby is created
//     /// </summary>
//     /// <param name="lobbyDetails">Data received back on the connection details</param>
//     private void OnLobbyCreated(LobbyCreated_t lobbyDetails) {
//         if (lobbyDetails.m_eResult != EResult.k_EResultOK) {
//             return;
//         }

//         networkManager.StartHost();
//         SteamMatchmaking.SetLobbyData(new CSteamID(lobbyDetails.m_ulSteamIDLobby), hostAddressKey, SteamUser.GetSteamID().ToString());
//     }
    }
}
