using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport;

public class PlaneConfigurationManager : MonoBehaviour
{
    [SerializeField] private uint maxResults = 100;

    [SerializeField] private float minPlaneArea = 0.25f;

    private readonly MLPermissions.Callbacks permissionsCallbacks = new();

    private ARPlaneManager planeManager;
    
    private bool permissionGranted = false;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
        planeManager = FindObjectOfType<ARPlaneManager>();
        permissionsCallbacks.OnPermissionGranted += OnPermissionGranted;
        permissionsCallbacks.OnPermissionDenied += OnPermissionDenied;
        permissionsCallbacks.OnPermissionDeniedAndDontAskAgain += OnPermissionDenied;
    }

    private void OnDestroy()
    {
        permissionsCallbacks.OnPermissionGranted -= OnPermissionGranted;
        permissionsCallbacks.OnPermissionDenied -= OnPermissionDenied;
        permissionsCallbacks.OnPermissionDeniedAndDontAskAgain -= OnPermissionDenied;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(AreSubsystemsLoaded);
        MLPermissions.RequestPermission(MLPermission.SpatialMapping, permissionsCallbacks);
    }

    private bool AreSubsystemsLoaded()
    {
        if (XRGeneralSettings.Instance == null && XRGeneralSettings.Instance.Manager == null) return false;
        var activeLoader = XRGeneralSettings.Instance.Manager.activeLoader;
        if (activeLoader == null) return false;
        return activeLoader.GetLoadedSubsystem<XRPlaneSubsystem>() != null;
    }

    private void OnPermissionGranted(string permission)
    {
        LearnXR.Core.Logger.Instance.LogInfo($"Permission {permission} was granted");
        planeManager.enabled = true;
        permissionGranted = true;
    }

    private void OnPermissionDenied(string permission)
    {
        LearnXR.Core.Logger.Instance.LogInfo($"Permission {permission} was denied");
        planeManager.enabled = false;
    }

    private void Update() => QueryPlanes();

    private void QueryPlanes()
    {
        if (planeManager != null && planeManager.enabled && permissionGranted)
        {
            MLXrPlaneSubsystem.Query = new MLXrPlaneSubsystem.PlanesQuery
            {
                Flags = planeManager.requestedDetectionMode.ToMLXrQueryFlags() | MLXrPlaneSubsystem.MLPlanesQueryFlags.SemanticAll,
                BoundsCenter = mainCamera.transform.position,
                BoundsRotation = mainCamera.transform.rotation,
                BoundsExtents = Vector3.one * 20.0f,
                MaxResults = maxResults,
                MinPlaneArea = minPlaneArea
            };
        }
    }
}
