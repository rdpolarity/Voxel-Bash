using Mirror;
using RDPolarity.Controllers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.Collections;

namespace RDPolarity.Player
{
    public class Bow : NetworkBehaviour
    {
        private float charge;

        [FormerlySerializedAs("player")] [SerializeField]
        private PlayerController playerController;

        [SerializeField] private Image cooldownImage;
        private Canvas cooldownCanvas;

        [SerializeField] private float power;
        [SerializeField] private float chargeRate;
        [SerializeField] private float minForce;
        [SerializeField] private float maxForce;
        [SerializeField] private float cooldown;
        [SerializeField] private GameObject arrow;

        private float cooldownTimer;

        private Vector3 dir = Vector3.zero;

        private AimIndicator indicator;


        public bool Charging { get; set; }

        public Vector3 Dir
        {
            get { return dir; }
            set { dir = value; }
        }

        public float CooldownTimer
        {
            get => cooldownTimer;
        }

        // Start is called before the first frame update
        void Start()
        {
            indicator = GetComponentInChildren<AimIndicator>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            cooldownTimer -= Time.fixedDeltaTime;
            if (cooldownTimer < 0)
            {
                cooldownImage.enabled = false;
                if (Charging)
                {
                    charge += Time.deltaTime * chargeRate;
                    indicator.gameObject.SetActive(true);
                    indicator.DrawIndicator(
                        Mathf.Clamp(charge * power, minForce, maxForce) * dir,
                        transform.position + dir);
                }
                else
                {
                    
                    
                }
            }
            else
            {
                cooldownImage.enabled = true;
                indicator.gameObject.SetActive(false);
            }

            cooldownImage.transform.LookAt(Camera.main.transform);
            cooldownImage.fillAmount = Mathf.Clamp(1 - cooldownTimer / cooldown, 0, 1);
        }

        public void Shoot(Vector3 pos)
        {
            if (!hasAuthority) return;
            
            var pow = Mathf.Clamp(charge * power, minForce, maxForce) * dir;
            indicator.Clear();
            charge = 0;
            Debug.Log("Setting false");
            Charging = false;
            
            if (cooldownTimer < 0)
            {
                if (!isServer)
                {
                    var projectile = Instantiate(arrow, transform.position, Quaternion.LookRotation(pow.normalized));
                    projectile.GetComponent<Arrow>().Launch(pow);
                }

                CmdShoot(pos, dir, pow, NetworkTime.time);
                cooldownTimer = cooldown;
            }
        }
        

        [Command]
        private void CmdShoot(Vector3 pos, Vector3 dir, Vector3 pow, double networkTime)
        {
            double timePassed = NetworkTime.time - networkTime;
            
            var projectile = Instantiate(arrow);
            projectile.transform.position = transform.position + dir / 2;
            projectile.GetComponent<Arrow>().Launch(pow);
            
            RpcShoot(pos, dir, pow, networkTime);
        }

        [ClientRpc]
        private void RpcShoot(Vector3 pos, Vector3 dir, Vector3 pow, double networkTime)
        {
            if (hasAuthority) return;
            if (isServer) return;
            var projectile = Instantiate(arrow);
            projectile.transform.position = transform.position + dir / 2;
            projectile.GetComponent<Arrow>().Launch(pow);
        }
    }
}