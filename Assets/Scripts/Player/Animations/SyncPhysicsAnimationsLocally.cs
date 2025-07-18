using UnityEngine;

public class SyncPhysicsAnimationsLocally : MonoBehaviour
{
    Rigidbody rb;
    ConfigurableJoint joint;

    [SerializeField]
    Rigidbody animatedRb;

    [SerializeField]
    bool synAnimation = false;


    Quaternion startLocalRotation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        joint = GetComponent<ConfigurableJoint>();

        startLocalRotation = transform.localRotation;
    }

    public void UpdateFromJointAnimation()
    {
        if (!synAnimation)
            return;
        ConfigurableJointExtensions.SetTargetRotationLocal(joint, animatedRb.transform.localRotation, startLocalRotation);

    }
}
