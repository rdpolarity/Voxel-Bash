using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace RDPolarity.Arena
{
    /// <summary>
    /// Provides logic for each block
    /// </summary>
    public class Block : MonoBehaviour
    {
        [SerializeField] private GameObject explosionEffect;
        [SerializeField] private GameObject spawnOnDestroyed;
        [SerializeField] private GameObject nestedBlock;
        
        public Vector2 coordinates;
        public List<Block> neighbours;
    
        //IMPORTANT!!!! this method must be used to destroy a tile to prevent null fields in other lists
        public void Delete()
        {
            Debug.Log("Destroyed");
            if (explosionEffect != null) Instantiate(explosionEffect, transform.position, transform.rotation);
            if (spawnOnDestroyed != null) Instantiate(spawnOnDestroyed, transform.position, transform.rotation);
            foreach (Block t in neighbours)
            {
                t.neighbours.Remove(this);
            }
            if (nestedBlock != null)
            {
                nestedBlock.transform.localPosition = new Vector3(0, 20, 0);
            }
            gameObject.transform.position = gameObject.transform.position - new Vector3(0, 20, 0);
        }
    }
}
