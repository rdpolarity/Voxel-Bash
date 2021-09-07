using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : Singleton<GameSettings>
{
    private GameObject selectedMap;

    public GameObject SelectedMap { get => selectedMap; set => selectedMap = value; }
}
