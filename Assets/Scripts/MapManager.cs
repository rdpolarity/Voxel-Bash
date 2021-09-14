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

    public static MapManager Instance
    {
        private set;
        get;
    }

    [SerializeField]
    private List<GameObject> levels = new List<GameObject>();

    [SerializeField]
    TextMeshProUGUI levelText;

    private GameObject currentMap;

    private int selected = 0;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
        Inputs inputActions = new Inputs();
        inputActions.Player.Enable();
        inputActions.Player.ChangeMapForward.performed += Forward;
        inputActions.Player.ChangeMapBackwards.performed += Back;
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "Lobby") return;
        OnMapChange();
    }

    [Button]
    private void Back(InputAction.CallbackContext context)
    {
        if (SceneManager.GetActiveScene().name != "Lobby") return;
        if (selected <= 0) { selected = levels.Count - 1; }
        else { selected--; }
        OnMapChange();
    }

    [Button]
    private void Forward(InputAction.CallbackContext context)
    {
        if (SceneManager.GetActiveScene().name != "Lobby") return;
        if (selected >= levels.Count - 1) { selected = 0; }
        else { selected++; }
        OnMapChange();

    }

    private void OnMapChange()
    {
        if (NetworkServer.active)
        {
            SpawnPreview();
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
    public void SpawnMap(Vector3 loc)
    {
        currentMap = Instantiate(levels[selected], loc, Quaternion.identity);
        NetworkServer.Spawn(currentMap);
    }

    [Server]
    public void SpawnPreview()
    {
        // Destroys the last preview
        if (currentMap != null) NetworkServer.Destroy(currentMap);

        // Spawns map and remove spawn locations
        currentMap = Instantiate(levels[selected], transform.position, Quaternion.identity);
        RemoveStartPositions(currentMap);

        // Set Preview Scale and Position
        currentMap.transform.localScale = Vector3.one / 1.5f;
        currentMap.transform.position = transform.position; // Needs to be set like this due to network identity
        NetworkServer.Spawn(currentMap);

        // Sets the text of map leve (needs to be refactored to sync var)
        if (levelText != null)
        {
            levelText.text = levels[selected].name;
        }
        GameSettings.Instance.SelectedMap = levels[selected];
    }

    [Server]
    public void DestroyMapBlock(GameObject block)
    {
        if (currentMap != null)
        {
            RpcDestroyBlock(currentMap.GetComponent<NetworkIdentity>(), block.transform.position);
        }
    }



    [ClientRpc]
    private void RpcDestroyBlock(NetworkIdentity map, Vector3 pos)
    {
        Debug.Log(map.gameObject.name);
        var tiles = map.GetComponentsInChildren<Tile>();
        float closest = 0;
        Tile tile = null;

        foreach (var t in tiles)
        {
            var d = Vector3.Distance(t.transform.position, pos);
            if (tile == null || d < closest)
            {
                tile = t;
                closest = d;
            }
        }

        if (closest < 1 && tile != null)
        {
            tile.Delete();
        }
    }


}
