using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class CameraTransitionManager : MonoBehaviour
{
    public static CameraTransitionManager Instance { get; private set; }
    public GameObject cameraObject;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AnimateCameraToPosition(Transform targetPosition, float duration, System.Action onComplete = null)
    {
        LeanTween.move(cameraObject, targetPosition.position, duration)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() =>
            {
                onComplete?.Invoke();
            });
    }
}
