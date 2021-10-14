using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RDPolarity
{
    public class LookAtCamera : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            transform.rotation = Camera.main.transform.rotation;
            
            Vector3 objectNormal = transform.rotation * Vector3.forward;
            Vector3 cameraToText = transform.position - Camera.main.transform.position;
            float f = Vector3.Dot (objectNormal, cameraToText);
            if (f < 0f) 
            {
                transform.Rotate (0f, 180f, 0f);
            }
        }
    }
}
