using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

    [SerializeField]
    private GameObject escapeMenuGroup;

    [SerializeField]
    private GameObject lobbyButton;

    private void Awake()
    {
        if (lobbyButton != null)
        {
            if (SceneManager.GetActiveScene().name != "Lobby")
            {
                if (NetworkServer.active && NetworkClient.isHostClient)
                {
                    lobbyButton.SetActive(true);
                }
            }
        }
        Inputs inputActions = new Inputs();
        inputActions.Player.Enable();
        inputActions.Player.EscapeMenu.performed += ToggleEscapeMenu;
    }

    public void ToggleEscapeMenu(InputAction.CallbackContext context)
    {
        if (escapeMenuGroup != null)
        {
            escapeMenuGroup.SetActive(!escapeMenuGroup.activeSelf);
        }
    }

    public void LeaveGameOnClick()
    {
        var nm = NetworkManager.singleton as VoxelBashNetworkManager;
        nm.Leave();
    }

    public void JoinGameOnClick()
    {
        var nm = NetworkManager.singleton as VoxelBashNetworkManager;
        nm.Join();
    }

    public void HostGameOnClick()
    {
        var nm = NetworkManager.singleton as VoxelBashNetworkManager;
        nm.Host();
    }

    public void GotoLobby()
    {
        if (NetworkServer.active && NetworkClient.isHostClient) {
            NetworkManager.singleton.ServerChangeScene("Lobby");
        }
    }

    /// <summary>
    /// Will go to the main menu offline
    /// </summary>
    public void GotoOfflineMainMenu()
    {
        NetworkManager.singleton.ServerChangeScene("MainMenu");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
