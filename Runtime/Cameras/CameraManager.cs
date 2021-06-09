using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Linq;

public class CameraManager : MonoBehaviour
{
    private Camera baseCamera;
    private UniversalAdditionalCameraData baseCameraAdditionalCameraData;
    private List<CameraStackMember> cameraStackMembers;
    
    #region Singleton
    private static CameraManager _instance;

    public static CameraManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<CameraManager>();
            return _instance;
        }
    }
    #endregion


    private void Awake()
    {
        baseCamera = GetComponent<Camera>();
        baseCameraAdditionalCameraData = baseCamera.GetComponent<UniversalAdditionalCameraData>();
        _instance = this;
        cameraStackMembers = new List<CameraStackMember>();
    }

    public void AddCameraToStack(Camera camera, int stackPriority)
    {
        if (baseCameraAdditionalCameraData.cameraStack.Contains(camera))
            return;

        CameraStackMember cameraStackMember = new CameraStackMember(camera, stackPriority);
        cameraStackMembers.Add(cameraStackMember);
        SortCameraStack();
    }



    public void RemoveCameraFromStack(Camera camera)
    {
        if (!baseCameraAdditionalCameraData.cameraStack.Contains(camera))
            return;

        int removeCamIndex = cameraStackMembers.FindIndex(c => c.Camera == camera);
        cameraStackMembers.RemoveAt(removeCamIndex);
        SortCameraStack();
    }

    private void SortCameraStack()
    {
        Camera[] sortedStackMembers = cameraStackMembers
            .OrderBy(c => c.StackPriority)
            .Select(c => c.Camera)
            .ToArray();

        baseCameraAdditionalCameraData.cameraStack.Clear();
        baseCameraAdditionalCameraData.cameraStack.AddRange(sortedStackMembers);
    }
    
    internal struct CameraStackMember
    {
        public Camera Camera;
        public int StackPriority;

        public CameraStackMember(Camera cam, int stackPriority)
        {
            Camera = cam;
            StackPriority = stackPriority;
        }
    }
}
