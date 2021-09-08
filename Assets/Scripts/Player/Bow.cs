using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    private float charge;
    
    [SerializeField]
    private GameObject arrow;

    [SerializeField]
    private float power;
    [SerializeField]
    private float chargeRate;
    [SerializeField]
    private float minForce;
    [SerializeField]
    private float maxForce;

    private Vector3 dir = Vector3.zero;

    private float yHeight;

    private AimIndicator indicator;

    // Start is called before the first frame update
    void Start()
    {
        indicator = GetComponentInChildren<AimIndicator>();
        yHeight = transform.position.y;
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
            indicator.DrawIndicator(Mathf.Clamp(charge * power, minForce, maxForce) * (dir + new Vector3(0, 0.1f, 0)), arrow.GetComponent<Rigidbody>(), transform.position + dir);
        }
        else
        {
            indicator.gameObject.SetActive(false);
        }
    }

    public void Shoot(Vector3 pos)
    {
        var projectile = Instantiate(arrow);
        projectile.transform.position = new Vector3(transform.position.x + dir.x, yHeight, transform.position.z + dir.z);
        projectile.GetComponent<Rigidbody>().velocity = Mathf.Clamp(charge * power, minForce, maxForce) * (dir);

        indicator.Clear();
        charge = 0;
        Charging = false;
    }
}
