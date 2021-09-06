using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

    [SerializeField]
    private GameObject escapeMenuGroup;

    public void ToggleEscapeMenu()
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
