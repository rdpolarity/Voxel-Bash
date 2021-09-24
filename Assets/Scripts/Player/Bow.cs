using RDPolarity.Controllers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace RDPolarity.Player
{
    public class Bow : MonoBehaviour
    {
        private float charge;

        [FormerlySerializedAs("player")] [SerializeField]
        private PlayerController playerController;
        [SerializeField]
        private Image cooldownImage;
        private Canvas cooldownCanvas;

        [SerializeField]
        private float power;
        [SerializeField]
        private float chargeRate;
        [SerializeField]
        private float minForce;
        [SerializeField]
        private float maxForce;
        [SerializeField]
        private float cooldown;

        private float cooldownTimer;

        private Vector3 dir = Vector3.zero;

        private AimIndicator indicator;


        public bool Charging { get; set; }
        public Vector3 Dir { get { return dir; } set { dir = value; } }

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
                    indicator.DrawIndicator(Mathf.Clamp(charge * power, minForce, maxForce) * (dir + new Vector3(0, 0.1f, 0)), transform.position + dir);
                }
                else
                {
                    indicator.gameObject.SetActive(false);
                }
            } else {
                cooldownImage.enabled = true;
            }
            cooldownImage.transform.LookAt(Camera.main.transform);
            cooldownImage.fillAmount = Mathf.Clamp(1 - cooldownTimer / cooldown, 0 , 1);
        }

        public void shoot(Vector3 pos)
        {
            var pow = Mathf.Clamp(charge * power, minForce, maxForce) * (dir + new Vector3(0, 0.1f,0));
            indicator.Clear();
            charge = 0;
            Charging = false;
            if (cooldownTimer < 0) {
                playerController.CmdShoot(pos, dir, pow);
                cooldownTimer = cooldown;
            }
        }
    }
}
