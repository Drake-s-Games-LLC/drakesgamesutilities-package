using UnityEngine;

public class AddThisCameraToStack : MonoBehaviour
{
    [SerializeField] private int cameraStackPriority = 999;
    private Camera cameraComponentToAdd;

    private void Awake()
    {
        cameraComponentToAdd = GetComponent<Camera>();
        if (!cameraComponentToAdd)
            Debug.LogWarning("No camera component found!");
    }

    private void OnEnable()
    {
        CameraManager.Instance.AddCameraToStack(cameraComponentToAdd, cameraStackPriority);
    }

    private void OnDisable()
    {
        if (!CameraManager.Instance)
            return;
        CameraManager.Instance.RemoveCameraFromStack(cameraComponentToAdd);
    }
}
