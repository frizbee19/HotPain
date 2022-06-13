using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePhysics : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] GameObject playerArmature;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enviroment"))
        {
            if(playerArmature.GetComponent<RagdollController>().isRagdoll)
            {
                playerArmature.GetComponent<RagdollController>().TakeDamage(1);
                Debug.Log("Damage");
            }
        
        }
    }
}
