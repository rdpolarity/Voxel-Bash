using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class Bow : MonoBehaviour
{
    private float charge;

    [SerializeField]
    private PlayerController player;
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

    // Start is called before the first frame update
    void Start()
    {
        indicator = GetComponentInChildren<AimIndicator>();
    }

    public bool Charging { get; set; }
    public Vector3 Dir { get { return dir; } set { dir = value; } }

    // Update is called once per frame
    void FixedUpdate()
    {
        cooldownTimer -= Time.fixedDeltaTime;
        if (cooldownTimer < 0)
        {
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
        }
        Debug.Log(1 - cooldownTimer / cooldown);
        cooldownImage.transform.LookAt(Camera.main.transform);
        cooldownImage.fillAmount = Mathf.Clamp(1 - cooldownTimer / cooldown, 0 , 1);
    }

    public void shoot(Vector3 pos)
    {
        var pow = Mathf.Clamp(charge * power, minForce, maxForce) * (dir + new Vector3(0, 0.1f,0));
        indicator.Clear();
        charge = 0;
        Charging = false;

        player.CmdShoot(pos, dir, pow);
        cooldownTimer = cooldown;
    }
}
