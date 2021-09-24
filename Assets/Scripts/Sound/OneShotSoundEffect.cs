using UnityEngine;
using FMODUnity;

namespace RDPolarity.Sound
{
    public class OneShotSoundEffect : MonoBehaviour
    {
        [SerializeField] private EventReference dashSound;
        [SerializeField] private EventReference swingSound;
        [SerializeField] private EventReference shootSound;
        [SerializeField] private EventReference chargeSound;
        [SerializeField] private EventReference hitSound;
        [SerializeField] private EventReference deathSound;
        [SerializeField] private EventReference blockSound;
        public void PlaySwingSound() {RuntimeManager.PlayOneShot(swingSound);}
        public void PlayShootSound() {RuntimeManager.PlayOneShot(shootSound);}
        public void PlayChargeSound() {RuntimeManager.PlayOneShot(chargeSound);}
        public void PlayHitSound() {RuntimeManager.PlayOneShot(hitSound);}
        public void PlayDeathSound() {RuntimeManager.PlayOneShot(deathSound);}
        public void PlayDashSound() {RuntimeManager.PlayOneShot(dashSound);}
        public void PlayBlockSound() {RuntimeManager.PlayOneShot(blockSound);}
    }
}