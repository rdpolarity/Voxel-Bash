using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Bow : MonoBehaviour
{
    private float charge;

    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private float power;
    [SerializeField]
    private float chargeRate;
    [SerializeField]
    private float minForce;
    [SerializeField]
    private float maxForce;

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

    public void shoot(Vector3 pos)
    {
        var pow = Mathf.Clamp(charge * power, minForce, maxForce) * (dir + new Vector3(0, 0.1f,0));
        indicator.Clear();
        charge = 0;
        Charging = false;

        player.CmdShoot(pos, dir, pow);     
    }
}
