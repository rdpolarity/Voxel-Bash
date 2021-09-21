using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AutoDestroyParicleOnFinish : MonoBehaviour
{ 
     public void Start() 
     {
         Destroy(gameObject, 2);
     }
}
