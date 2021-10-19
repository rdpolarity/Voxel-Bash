using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RDPolarity.Editor
{
    public class SceneSelector : MonoBehaviour
    {
        [MenuItem("Voxel Bash/Player Prefab", priority = 1)]
        public static void LoadPlayerPrefab()
        {
            PrefabUtility.LoadPrefabContents(Constants.PLAYER_PREFAB_PATH);
        }
        
        [MenuItem("Voxel Bash/Main Menu Scene", priority = 10)]
        public static void LoadMainMenu()
        {
            EditorSceneManager.OpenScene(Constants.MAIN_MENU_SCENE_PATH, OpenSceneMode.Single);
            EditorSceneManager.OpenScene(Constants.PERSISTENT_SCENE_PATH, OpenSceneMode.Additive);
        }

        [MenuItem("Voxel Bash/Lobby Scene", priority = 20)]
        public static void LoadLobby()
        {
            EditorSceneManager.OpenScene(Constants.LOBBY_SCENE_PATH, OpenSceneMode.Single);
            EditorSceneManager.OpenScene(Constants.PERSISTENT_SCENE_PATH, OpenSceneMode.Additive);
        }

        [MenuItem("Voxel Bash/Arena Scene", priority = 30)]
        public static void LoadArena()
        {
            EditorSceneManager.OpenScene(Constants.ARENA_SCENE_PATH, OpenSceneMode.Single);
            EditorSceneManager.OpenScene(Constants.PERSISTENT_SCENE_PATH, OpenSceneMode.Additive);
        }
    }
}
