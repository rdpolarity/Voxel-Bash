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


    private void Awake()
    {
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
