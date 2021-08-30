using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelList : MonoBehaviour
{
    [SerializeField]
    private GameObject listItem;

    [SerializeField]
    private List<GameObject> levels;

    void Start()
    {
        levels.ForEach(level => {
            var item = Instantiate(listItem);
            item.name = level.name;
            item.GetComponentInChildren<Text>().text = level.name;
            item.transform.parent = gameObject.transform;
        });
    }
}
