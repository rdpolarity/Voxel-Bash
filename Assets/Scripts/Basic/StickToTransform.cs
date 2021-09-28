using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RDPolarity
{
    public class StickToTransform : MonoBehaviour
    {
        [SerializeField] public Transform target;

        void FixedUpdate()
        {
            if (target != false)
            {
                transform.position = target.position;
            }
        }
    }
}
