using UnityEngine;
using UnityEngine.SceneManagement;

namespace RDPolarity.Basic
{
    /// <summary>
    /// Automatically loads a scene on start
    /// </summary>
    public class AutoLoadScene : MonoBehaviour
    {
        [SerializeField] private string sceneName = "MainMenu";
        void Start()
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
