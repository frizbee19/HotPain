using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    // Start is called before the first frame update
	// private StarterAssetsInputs _input;
	// private PlayerInput _playerInput;
    // private bool IsCurrentDeviceMouse => _playerInput.currentControlScheme == "KeyboardMouse";

    // public Collider mainCollider;
    [Header("Animator")]
    [Tooltip("The animator for the active ragdoll")]
    [SerializeField] private Animator animator;
    [Tooltip("The animator for the character while in control")]
    [SerializeField] private Animator uprightAnimator;
    // private Collider[] limbColliders;
    // private Rigidbody[] limbRigidbodies;
    private ConfigurableJoint[] limbJoints;
    [Header("Ragdoll control")]
    [Tooltip("Threshold between freefall and bracing for impact")]
    [SerializeField] private float tuckStartDistance = 3.0f;
    [Tooltip("Threshold between bracing and getting ready to get up")]
    [SerializeField] private float tuckStartVelocity = 2.0f;
    [Tooltip("Time to get up after going below the tucking velocity threshold")]
    [SerializeField] private float tuckEndDelay = 1.25f;
    private float tuckTimeElapsed = 0.0f;
    [Tooltip("Main rigidbody for the ragdoll")]
    [SerializeField] private GameObject _ragdollAnchor;
    [Tooltip("For testing purposes")]
    [SerializeField] public bool isRagdoll;
    [Tooltip("Height at which the character will start ragdolling")]
    [SerializeField] private float _ragdollStartDistance = 7.5f;
    [Tooltip("The time it takes to start ragdolling after character has stayed in the air for the ragdollStartDistance")]
    [SerializeField] private float _ragdollStartTime = 1f;
    [SerializeField] private float fallDeathVelocity = -15f;
    private float FallDamageTimer = 0f;
    private float fallTimer = 0.0f;
    private bool dead = false;

    // LayerMask mask = LayerMask.GetMask("Environment");
    // int layerMask = 1 << 8;
    [Tooltip("hips")]
    [SerializeField] private GameObject hips;
    private Vector3 prevPosition;
    private Vector3 currPosition;
    private Vector3 velocity;
    private Vector3 velocityPrev;
    private float velocityMagnitude;
    private float velocityMagnitudePrev;
    [Header("Health")]
    [SerializeField] private float bleedOut = 1000.0f;
    [SerializeField] private float maxBleedOut = 1000.0f;
    [SerializeField] private float bleedOutThreshold = 1000.0f;
    [SerializeField] private float baseBleedOutRate = 100.0f;
    [SerializeField] private float bleedOutMultiplier = 1.0f;
    // private float startHealth;

    void Start()
    {
        limbJoints = gameObject.GetComponentsInChildren<ConfigurableJoint>();
        currPosition = _ragdollAnchor.transform.position;
        prevPosition = _ragdollAnchor.transform.position;
    }

    
    RaycastHit[] hits;
    // Update is called once per frame
    void FixedUpdate()
    {
        currPosition = _ragdollAnchor.transform.position;
        velocity = currPosition - prevPosition;
        velocityMagnitude = velocity.magnitude / Time.fixedDeltaTime;
        IsBleeding();
        float minHeight = HeightFromGround();
        
        IsDead();
        if(isRagdoll) {
            // Debug.Log("Ragdoll");
            animator.SetFloat("FallVelocity", velocityMagnitude);
            if(minHeight > tuckStartDistance + Mathf.Max((velocityMagnitude / (velocityMagnitude - tuckStartDistance)), 0)) {
                animator.SetBool("FreeFall", true);
            }
            else {
                animator.SetBool("FreeFall", false);
                if(velocityMagnitude > tuckStartVelocity) {
                    tuckTimeElapsed = 0.0f;
                    animator.SetBool("FastTumble", true);
                }
                else {
                    tuckTimeElapsed += Time.deltaTime;
                    if(tuckTimeElapsed > tuckEndDelay) {
                        animator.SetBool("FastTumble", false);
                        tuckTimeElapsed = 0.0f;
                        GetUp();
                    }
                }
            }
            FallDamage();
        }
        else {
            if(minHeight > _ragdollStartDistance ) {
                fallTimer += Time.deltaTime;
            }
            else if(minHeight < 0.5f) {
                fallTimer = 0.0f;
            }
            if(fallTimer > _ragdollStartTime) {
                fallTimer = 0.0f;
                Debug.Log("ragdoll start");
                isRagdoll = true;
                animator.SetBool("isRagdoll", true);
                uprightAnimator.enabled = false;
                _ragdollAnchor.GetComponent<Rigidbody>().AddForce(velocity, ForceMode.VelocityChange);
            }
        }
        prevPosition = currPosition;
        velocityPrev = velocity;
        velocityMagnitudePrev = velocityMagnitude;
    }
    

    private float HeightFromGround() {
        hits = Physics.RaycastAll(_ragdollAnchor.transform.position, _ragdollAnchor.transform.TransformDirection(Vector3.down), Mathf.Infinity);
        float minHeight = Mathf.Infinity;
        foreach(RaycastHit hit in hits) {
            if(hit.collider.gameObject.layer == 8) {
                if(hit.distance < minHeight) {
                    minHeight = hit.distance;
                }
            }
        }
        return minHeight;
    }
    private void IsBleeding() {
        if(bleedOut < bleedOutThreshold && bleedOut >= 0.0f) {
            bleedOut -= baseBleedOutRate * bleedOutMultiplier * Time.deltaTime;
            Ragdoll();
        }
        else if(bleedOut > bleedOutThreshold && bleedOut <= maxBleedOut) {
            bleedOut += 0.5f * baseBleedOutRate * Time.deltaTime;
        }
    }

    private void FallDamage() {
        if(isRagdoll) {
            float minHeight = HeightFromGround();
            FallDamageTimer += Time.deltaTime;
            if(minHeight < 0.6f && FallDamageTimer > 0.75f) {
                
                Debug.Log("minHeight: " + minHeight);
                if(Mathf.Abs((velocity.y/Time.fixedDeltaTime)/fallDeathVelocity) > 0.3f) {
                    TakeDamage(Mathf.Abs((velocity.y/Time.fixedDeltaTime)/fallDeathVelocity) - 0.3f);
                    FallDamageTimer = 0.0f;
                }
            }
        }
    }

    private void Ragdoll() {
        isRagdoll = true;
        animator.SetBool("isRagdoll", true);
        uprightAnimator.enabled = false;
    }
    private void GetUp() {
        if(!dead && bleedOut >= bleedOutThreshold) {
            isRagdoll = false;
            animator.SetBool("isRagdoll", false);
            Vector3 temp = hips.transform.position;
            transform.position = hips.transform.position = temp;
            uprightAnimator.enabled = true;
        }
    }


    private void IsDead() {
        if(bleedOut <= 0.0f) {
            foreach(ConfigurableJoint joint in limbJoints) {
                JointDrive xDrive = joint.angularXDrive;
                JointDrive yzDrive = joint.angularYZDrive;
                xDrive.positionSpring = 2.0f;
                yzDrive.positionSpring = 2.0f;
                joint.angularXDrive = xDrive;
                joint.angularYZDrive = yzDrive;
            }
            dead = true;
        }
    }

    public void TakeDamage(float calibre) {
        if(calibre > 0.0f) {
            bleedOut -= bleedOutThreshold * calibre;
        }
        else {
            bleedOut -= baseBleedOutRate;
        }
        bleedOutMultiplier = bleedOutMultiplier * (1 + (0.5f * calibre));
    }

    
    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.layer == 8) {
            TakeDamage(Mathf.Abs(velocityMagnitude/fallDeathVelocity));
            Debug.Log("collision");
        }
    }
}





