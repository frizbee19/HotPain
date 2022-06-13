using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRoot : MonoBehaviour
{
    [SerializeField] private GameObject anchor;
    [SerializeField] private float followSpeed = 10.0f;
    private bool prevRagdoll = false;
    private bool currRagdoll = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        currRagdoll = transform.parent.GetComponent<RagdollController>().isRagdoll;
        // transform.Translate(anchor.transform.position - transform.position);
        if(prevRagdoll && !currRagdoll)
        {
            transform.position = anchor.transform.position;
        }
        else {
            transform.position = Vector3.Lerp(transform.position, anchor.transform.position, (followSpeed + anchor.GetComponent<Rigidbody>().velocity.magnitude) * Time.deltaTime);
        }
        prevRagdoll = currRagdoll;
    }

    

    void FixedUpdate() {
        // transform.Translate(anchor.transform.position - transform.position);
    }
}
