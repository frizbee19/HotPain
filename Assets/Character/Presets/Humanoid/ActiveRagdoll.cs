using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveRagdoll : MonoBehaviour
{
    [SerializeField] private Transform targetLimb;
    private ConfigurableJoint joint;

    Quaternion targetInitialRotation;
    // Start is called before the first frame update
    void Start()
    {
        joint = gameObject.GetComponent<ConfigurableJoint>();
        targetInitialRotation = targetLimb.transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        joint.targetRotation = MoveRotation();
    }

    private Quaternion MoveRotation() {
        return Quaternion.Inverse(targetLimb.localRotation) * targetInitialRotation;
    }
}
