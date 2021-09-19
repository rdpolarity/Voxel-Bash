using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : Singleton<LobbyManager>
{
    private int selectedMap = 0;

    public int SelectedMap { get => selectedMap; set => selectedMap = value; }
}
