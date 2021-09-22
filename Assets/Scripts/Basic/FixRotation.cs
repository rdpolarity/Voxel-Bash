using UnityEngine;

namespace RDPolarity.Basic
{
    /// <summary>
    /// Prevents children from rotating with parent
    /// </summary>
    public class FixRotation : MonoBehaviour
    {
        private Quaternion _rotation;
        void Awake()
        {
            _rotation = transform.rotation;
        }
        void LateUpdate()
        {
            transform.rotation = _rotation;
        }
    }
}
