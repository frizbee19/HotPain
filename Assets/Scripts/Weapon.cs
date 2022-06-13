using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //fire rate
    public float fireRate = 0.5f;
    private float nextFire = 0.0f;
    [SerializeField] bool isProjectile;
    [SerializeField] bool isMelee;
    [SerializeField] int maxAmmo;
    [SerializeField] int currentAmmo;

    [SerializeField] LayerMask aimCollisionMask = new LayerMask();

    private Vector3 firePoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire() {
        firePoint = gameObject.transform.position;
        if (Time.time > nextFire) {
            nextFire = Time.time + fireRate;
            if(!isProjectile) {
                Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
                Ray ray = Camera.main.ScreenPointToRay(screenCenter);
                if(Physics.Raycast(ray, out RaycastHit hit, 999f, aimCollisionMask)) {
                    Debug.DrawLine(firePoint, hit.point, Color.red);
                    Debug.Log("shoot");
                }
            }
        }
    }
}
