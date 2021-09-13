using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Mirror;

public class MapSelector : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> levels = new List<GameObject>();

    [SerializeField]
    TextMeshProUGUI levelText;

    private int selected = 0;

    private void Awake()
    {
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
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        var currentMap = Instantiate(levels[selected], transform.position, Quaternion.identity);
        var startPositions = currentMap.gameObject.GetComponentsInChildren<NetworkStartPosition>();
        foreach (var obj in startPositions)
        {
            Destroy(obj.gameObject);
        }
        
        currentMap.transform.localScale = Vector3.one / 1.5f;
        currentMap.transform.parent = transform;
        if (levelText != null)
        {
            levelText.text = levels[selected].name;
        }
        GameSettings.Instance.SelectedMap = levels[selected];
    }



}
