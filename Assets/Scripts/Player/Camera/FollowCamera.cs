using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Rigidbody playerRB;
    [SerializeField] private Vector3 offset = new Vector3(0, 0.6f, 0);
    [SerializeField] private float followSpeed = 10f;
    [SerializeField] private float bobAmount = 0.05f;
    [SerializeField] private float bobSpeed = 10f;
    private float bobTimer = 0f;

    void LateUpdate()
    {
        HandleCameraFollow();
    }

    public void HandleCameraFollow()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        // Slight head bob effect
        if (playerRB.linearVelocity.magnitude > 0.1f)
        {
            bobTimer += Time.deltaTime * bobSpeed;
            desiredPosition.y += Mathf.Sin(bobTimer) * bobAmount;
        }

        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * followSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, Time.deltaTime * followSpeed);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public Transform GetTarget()
    {
        return target;
    }

}
