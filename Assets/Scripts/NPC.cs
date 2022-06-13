using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Animator>().enabled = true;
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Player") {
            gameObject.GetComponent<Animator>().enabled = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
