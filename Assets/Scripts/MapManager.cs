using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Mirror;

public class MapManager : NetworkBehaviour
{

    public static MapManager Instance {
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
        Inputs inputActions = new Inputs();
        inputActions.Player.Enable();
        inputActions.Player.ChangeMapForward.performed += Forward;
        inputActions.Player.ChangeMapBackwards.performed += Back;
    }

    private void Start()
    {
        OnMapChange();
    }

    [Button]
    private void Back(InputAction.CallbackContext context)
    {
        if (selected <= 0) { selected = levels.Count - 1; }
        else { selected--; }
        OnMapChange();
    }

    [Button]
    private void Forward(InputAction.CallbackContext context)
    {
        if (selected >= levels.Count - 1) { selected = 0; }
        else { selected++; }
        OnMapChange();
        
    }


    private void OnMapChange()
    {   
        if (NetworkServer.active) {
            if (currentMap != null) {
                NetworkServer.Destroy(currentMap);
            }

            currentMap = Instantiate(levels[selected], transform.position, Quaternion.identity);

            var startPositions = currentMap.GetComponentsInChildren<NetworkStartPosition>();
            foreach (var obj in startPositions)
            {
                Destroy(obj.gameObject);
            }
            
            currentMap.transform.localScale = Vector3.one / 1.5f;
            currentMap.transform.position = transform.position;

            NetworkServer.Spawn(currentMap);

            if (levelText != null)
            {
                levelText.text = levels[selected].name;
            }
            GameSettings.Instance.SelectedMap = levels[selected];
        }
        
    }

    [Server]
    public void DestroyMapBlock(GameObject block) {
        if (currentMap != null) {
            RpcDestroyBlock(currentMap.GetComponent<NetworkIdentity>(),block.transform.position);
        }
    }



    [ClientRpc]
    private void RpcDestroyBlock(NetworkIdentity map,Vector3 pos) {
        Debug.Log(map.gameObject.name);
        var tiles = map.GetComponentsInChildren<Tile>();
        float closest = 0;
        Tile tile = null;

        foreach (var t in tiles) {
            var d = Vector3.Distance(t.transform.position, pos);
            if (tile == null || d < closest) {
                tile = t;
                closest = d;
            }
        }

        if (closest < 1 && tile != null) {
            tile.Delete();
        }
    }


}
