using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RDPolarity
{
    public class AutoSceneSwitch : MonoBehaviour
    {
        void Start()
        {
            SceneManager.LoadScene(Constants.MAIN_MENU_SCENE);
        }
    }
}
