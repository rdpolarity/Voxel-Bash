using System.Collections.Generic;
using FMODUnity;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        [SerializeField] private EventReference blockSound;
        
        public Vector2 coordinates;
        public List<Block> neighbours;
    
        //IMPORTANT!!!! this method must be used to destroy a tile to prevent null fields in other lists
        public void Delete()
        {
            RuntimeManager.PlayOneShot(blockSound);
            if (explosionEffect != null) Instantiate(explosionEffect, transform.position, transform.rotation);
            if (spawnOnDestroyed != null) Instantiate(spawnOnDestroyed, transform.position, transform.rotation);
            foreach (Block t in neighbours)
            {
                t.neighbours.Remove(this);
            }
            if (nestedBlock != null && SceneManager.GetActiveScene().name == "Arena")
            {
                var basicBlock = Instantiate(nestedBlock, transform.position, transform.rotation);
                // basicBlock.transform.parent = transform;
                NetworkServer.Spawn(basicBlock);
            }
            gameObject.transform.position = gameObject.transform.position - new Vector3(0, 1000, 0);
        }
    }
}
