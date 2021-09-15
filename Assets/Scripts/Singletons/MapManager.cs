using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Mirror;
using UnityEngine.SceneManagement;

public class MapManager : NetworkBehaviour
{
    [SerializeField]
    private List<GameObject> levels = new List<GameObject>();

    [SerializeField]
    TextMeshProUGUI levelText;

    private GameObject currentMap;

    [SyncVar(hook = nameof(OnSelectionChanged))]
    private int selected;

    private Inputs inputActions;

    private void Awake()
    {
        inputActions = new Inputs();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.ChangeMapForward.performed += Forward;
        inputActions.Player.ChangeMapBackwards.performed += Back;
        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.Player.ChangeMapForward.performed -= Forward;
        inputActions.Player.ChangeMapBackwards.performed -= Back;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitMaps();
    }


    private void InitMaps()
    {
        if (NetworkServer.active)
        {
            if (SceneManager.GetActiveScene().name == "Arena") {
                SpawnMap(LobbyManager.Instance.SelectedMap);
                NetworkClient.localPlayer.gameObject.transform.position = NetworkManager.singleton.GetStartPosition().position;
            }
            if (SceneManager.GetActiveScene().name == "Lobby") {
                LobbyManager.Instance.SelectedMap = 0;
                SpawnPreview(selected);
            }
        }
    }

    private void Back(InputAction.CallbackContext context)
    {
        if (NetworkServer.active)
        {
            if (SceneManager.GetActiveScene().name != "Lobby") return;
            if (selected <= 0) { selected = levels.Count - 1; }
            else { selected--; }
        }
    }

    private void Forward(InputAction.CallbackContext context)
    {
        if (NetworkServer.active)
        {
            if (SceneManager.GetActiveScene().name != "Lobby") return;
            if (selected >= levels.Count - 1) { selected = 0; }
            else { selected++; }
        }
    }

    private void OnSelectionChanged(int oldSelection, int newSelection)
    {
        if (NetworkServer.active)
        {
            LobbyManager.Instance.SelectedMap = newSelection;
            SpawnPreview(newSelection);
        }
    }

    [Server]
    public void RemoveStartPositions(GameObject map)
    {
        var startPositions = map.GetComponentsInChildren<NetworkStartPosition>();
        foreach (var obj in startPositions)
        {
            Destroy(obj.gameObject);
        }
    }

    [Server]
    public void SpawnMap(int selectedMap)
    {
        if (NetworkServer.active)
        {
            currentMap = Instantiate(levels[selectedMap], transform.position, Quaternion.identity);
            NetworkServer.Spawn(currentMap);
        }
    }

    [Server]
    public void SpawnPreview(int currentSelection)
    {
        // Destroys the last preview
        if (currentMap != null) NetworkServer.Destroy(currentMap);

        // Spawns map and remove spawn locations
        currentMap = Instantiate(levels[currentSelection], transform.position, Quaternion.identity);
        RemoveStartPositions(currentMap);

        // Set Preview Scale and Position
        currentMap.transform.localScale = Vector3.one / 1.5f;
        currentMap.transform.position = transform.position; // Needs to be set like this due to network identity

        NetworkServer.Spawn(currentMap);
    }

    // SINGLETON AREA (NetworkBehavour can't inherit with generics... :< )

    [SerializeField]
    private static MapManager _instance;

    public static MapManager Instance { get { return _instance; } }

    private void Start()
    {
        InitMaps();

        if (_instance != null && _instance != this)
        {
            if (NetworkServer.active)
            {
                NetworkServer.Destroy(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            _instance = this;
        }
    }
}