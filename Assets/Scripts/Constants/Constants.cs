using System;
using UnityEngine;

namespace RDPolarity
{
    public static class Constants
    {
        public static readonly string SCENE_PATH = "Assets/Scenes";
        public static readonly string PREFABS_PATH = "Assets/Prefabs";
        
        public static readonly string PERSISTENT_SCENE = "Persistent";
        public static readonly string MAIN_MENU_SCENE = "MainMenu";
        public static readonly string ARENA_SCENE = "Arena";
        public static readonly string LOBBY_SCENE = "Lobby";
        
        public static readonly string PERSISTENT_SCENE_PATH = $"{SCENE_PATH}/{PERSISTENT_SCENE}.unity";
        public static readonly string MAIN_MENU_SCENE_PATH = $"{SCENE_PATH}/{MAIN_MENU_SCENE}.unity";
        public static readonly string ARENA_SCENE_PATH = $"{SCENE_PATH}/{ARENA_SCENE}.unity";
        public static readonly string LOBBY_SCENE_PATH = $"{SCENE_PATH}/{LOBBY_SCENE}.unity";
        
        public static readonly string PLAYER_PREFAB_PATH = $"{PREFABS_PATH}/Players/Player.prefab";
    }
}