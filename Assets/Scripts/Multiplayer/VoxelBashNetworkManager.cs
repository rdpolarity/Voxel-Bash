using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public struct CreateClientMessage : NetworkMessage
{

}

public class VoxelBashNetworkManager : NetworkManager
{
    [SerializeField]
    private GameObject playerSpawner = null;

    public override void OnStartServer()
    {
        base.OnStartServer();
        ServerChangeScene("Lobby");
        NetworkServer.RegisterHandler<CreateClientMessage>(OnCreateClient);
    }

    public override void OnStopServer()
    {
        ServerChangeScene("MainMenu");
        base.OnStopServer();
        NetworkServer.UnregisterHandler<CreateClientMessage>();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
    }

    public override void OnStartClient()
    {
        base.OnStopClient();
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);
        Debug.Log("New player has connected!");
    }

    [Server]
    public void OnCreateClient(NetworkConnection conn, CreateClientMessage msg)
    {
        Debug.Log("ClientScene Changes");
        Transform startPos = GetStartPosition();
        GameObject player = startPos != null
            ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
            : Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);
        NetworkClient.connection.Send(new CreateClientMessage());
    }
}
