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


    private void Awake() {
        Inputs inputActions = new Inputs();
        inputActions.Player.Enable();
        inputActions.Player.EscapeMenu.performed += ToggleEscapeMenu;
    }

    public void ToggleEscapeMenu(InputAction.CallbackContext context)
    {
        escapeMenuGroup.SetActive(!escapeMenuGroup.activeSelf);
    }

    public void LeaveGame() {
        NetworkManager.singleton.StopClient();
        GotoMainMenu();
    }

    public void Exit() {
        Application.Quit();
    }

    public void GotoMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }
}
