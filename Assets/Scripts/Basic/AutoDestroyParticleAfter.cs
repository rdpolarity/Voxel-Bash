using UnityEngine;

namespace RDPolarity.Basic
{
    /// <summary>
    /// Automatically destroys itself after specified time, ideal for instantiated particle prefabs (eg. explosions)
    /// </summary>
    public class AutoDestroyParticleAfter : MonoBehaviour
    {
        [SerializeField] private float lifetime = 2;

        public void Start()
        {
            Destroy(gameObject, lifetime);
        }
    }
}