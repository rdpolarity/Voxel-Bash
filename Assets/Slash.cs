using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

namespace RDPolarity
{
    public class Slash : MonoBehaviour
    {
        [SerializeField] private GameObject onHitParticles;
        [SerializeField] private EventReference slashHitSound;
        [SerializeField] private float knockback = 7;
        private bool _canTrigger = true;
        private void OnTriggerEnter(Collider other)
        {
            if (_canTrigger)
            {
                if (other.gameObject.CompareTag("Player"))
                {
                    Debug.Log("Hit player");
                    RuntimeManager.PlayOneShot(slashHitSound);
                    Instantiate(onHitParticles, transform.position, transform.rotation);
                    other.GetComponentInParent<Rigidbody>().AddForce(transform.parent.forward * knockback, ForceMode.Impulse);
                    StartCoroutine(TriggerCoolDown());
                }
            }
        }

        private IEnumerator TriggerCoolDown()
        {
            _canTrigger = false;
            yield return new WaitForSeconds(0.5f);
            _canTrigger = true;
        }
    }
}
