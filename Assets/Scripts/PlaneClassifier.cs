using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[Serializable]
public class ClassifierMaterialInfo
{
    public PlaneClassification classification;

    public Material material;
}

public class PlaneClassifier : MonoBehaviour
{
    [SerializeField] private TextMeshPro planeInfo;

    [SerializeField] private ClassifierMaterialInfo[] planeMaterials;

    private void Start()
    {
        ColorClassify();
    }

    private void ColorClassify()
    {
        var plane = GetComponent<ARPlane>();
        switch (plane.classification)
        {
            case PlaneClassification.Floor:
                planeInfo.text = "Floor";
                break;
            case PlaneClassification.Ceiling:
                planeInfo.text = "Ceiling";
                break;
            case PlaneClassification.Wall:
                planeInfo.text = "Wall";
                break;
            case PlaneClassification.Table:
                planeInfo.text = "Table";
                break;
            case PlaneClassification.Door:
                planeInfo.text = "Door";
                break;
            case PlaneClassification.Seat:
                planeInfo.text = "Seat";
                break;
            case PlaneClassification.Window:
                planeInfo.text = "Window";
                break;
            default:
                planeInfo.text = "Other";
                break;
        }

        var classifierInfo = planeMaterials.FirstOrDefault(m => m.classification
                                                                == plane.classification);
        var meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = classifierInfo?.material ?? meshRenderer.material;
        LearnXR.Core.Logger.Instance.LogInfo($"({plane.transform.name} classified as {plane.classification}");
    }
}
