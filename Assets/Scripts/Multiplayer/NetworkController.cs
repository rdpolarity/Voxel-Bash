using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkController : Singleton<NetworkController>
{
    public void Leave()
    {
        var nm = NetworkManager.singleton;
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            nm.StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            nm.StopClient();
        }
        else if (NetworkServer.active)
        {
            nm.StopServer();
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void Host()
    {
        var nm = NetworkManager.singleton;
        if (!NetworkClient.active)
        {
            if (Application.platform != RuntimePlatform.WebGLPlayer) {
                nm.StartHost();
            }
        }
    }

    public void Join()
    {
        var nm = NetworkManager.singleton;
        nm.StartClient();
    }
}
