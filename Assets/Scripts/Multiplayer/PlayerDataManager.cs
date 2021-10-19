using System;
using Mirror;
using UnityEngine;

namespace RDPolarity.Multiplayer
{
    public class PlayerDataManager : NetworkBehaviour
    {
        [SyncVar] public int skinID;

        public void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}