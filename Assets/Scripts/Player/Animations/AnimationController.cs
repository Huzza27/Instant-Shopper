using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private TwoBoneIKConstraint rightHandIK, leftHandIK;

    public void StartWalkAnim()
    {
        bool isWalking = anim.GetBool("isWalking");
        if (!isWalking)
        {
            anim.SetBool("isWalking", true);
        }
    }

    public void StopWalkAnim()
    {
        anim.SetBool("isWalking", false);
    }

    public void PlaceHandsOnHandle()
    {
        rightHandIK.weight = 1;
        leftHandIK.weight = 1;
    }
}
