using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollAnchor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //rotate the anchor upright in the worldspace
        transform.rotation = Quaternion.Euler(transform.TransformDirection(Vector3.up));
    }
}
