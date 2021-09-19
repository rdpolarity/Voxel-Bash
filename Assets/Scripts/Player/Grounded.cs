using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grounded : MonoBehaviour
{
    public bool active;
    [SerializeField]
    private int tilecount;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Tile") || collision.gameObject.CompareTag("Indestructable"))
        {
            Debug.Log("added tile");
            tilecount += 1;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        Debug.Log("removed tile");
        if(collision.gameObject.CompareTag("Tile") || collision.gameObject.CompareTag("Indestructable"))
        {
            tilecount -= 1;
        }
    }

    private void Update()
    {
        if (tilecount > 0)
        {
            active = true;
        }
        else
        {
            active = false;
        }
    }
}
