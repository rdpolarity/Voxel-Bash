using UnityEngine;

namespace RDPolarity.Editor
{
    /// <summary>
    /// Auto loads any prefabs into the scene on game load found under "Resources/Preload"
    /// Great for any singletons or persistent managers
    /// </summary>
    public class PreloadPrefabs : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnLoad() {
            var prefabs = Resources.LoadAll<GameObject>("Preload"); // Located in Assets/Resources/Preload
            if (prefabs != null) {
                Debug.Log(prefabs.Length + " Preload Assets Found!");
                foreach (var prefab in prefabs) {
                    Instantiate(prefab);
                    Debug.Log("Preloaded (Local): " + prefab.name);
                }
            } else {
                Debug.Log("No Preload Assets Found!");
            }
        }
    }
}
