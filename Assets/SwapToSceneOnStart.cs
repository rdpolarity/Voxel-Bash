using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RDPolarity
{
    public class SwapToSceneOnStart : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Start()
        {
            SceneManager.LoadScene(Constants.MAIN_MENU_SCENE);
        }
    }
}
