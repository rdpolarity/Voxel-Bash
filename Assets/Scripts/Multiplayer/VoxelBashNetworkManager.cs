using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public struct SpawnPlayerMessage : NetworkMessage
{
    String username;
}

public class VoxelBashNetworkManager : NetworkManager
{

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
        if (!NetworkClient.isHostClient) {
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
    public void OnCreateClient(NetworkConnection conn, SpawnPlayerMessage msg)
    {
        if (NetworkServer.active)
        {
            Transform startPos = GetStartPosition();
            GameObject player = startPos != null
                ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
                : Instantiate(playerPrefab);
            NetworkServer.AddPlayerForConnection(conn, player);
        }
    }
}
